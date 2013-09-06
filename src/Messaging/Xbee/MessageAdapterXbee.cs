using System;
using System.IO.Ports;
using System.Threading;
using NLog;
using BitHome.Messaging.Messages;
using BitHomeProtocol = BitHome.Messaging.Protocol;
using System.Text;
using System.Collections.Generic;

namespace BitHome.Messaging.Xbee
{
	public class MessageAdapterXbee : MessageAdapterBase, IDisposable
	{
		private enum PacketState {
			PacketStart, // Waiting for Initial packet byte 
			SizeMsb, // Waiting for MSB of frame size
			SizeLsb, // Waiting for LSB of frame size 
			Api, // Waiting for the API code 
			Data, // Waiting for the frame data 
			Checksum // Waiting for the frame checksum
		}

		// Packet decoding
        private byte m_currentFrameId = 1;
		private PacketState m_packetState = PacketState.PacketStart;
		private int m_packetSize = 0;
		private int m_packetDataIndex = 0;
		private byte m_packetChecksum = 0; // short for unsigned byte
        private byte m_packetApi = 0;
        private byte[] m_packetData = null;
        private byte m_sendChecksum;
		private object m_sendLock = new object ();
		private NodeXbee m_broadcastNode = new NodeXbee(0xFFFF, 0xFFFE);


		private readonly Logger log = LogManager.GetCurrentClassLogger();

		#region Defines
		private string _port;
		private int _baudRate;
		private SerialPort _serialPort;
		private Thread serThread;
		private double _PacketsRate;
		private DateTime _lastReceive;
		/*The Critical Frequency of Communication to Avoid Any Lag*/
		private const int freqCriticalLimit = 20;
		#endregion

		public override Node BroadcastNode {
			get { return m_broadcastNode; }
		}

		private byte NextFrameId 
		{
			get {
				return ++m_currentFrameId;
			}
		}

		#region Constructors

		public MessageAdapterXbee()
		{
			log.Trace ("()");

			_port = null;
			_baudRate = 57600;
			_lastReceive = DateTime.MinValue;

			serThread = new Thread(new ThreadStart(SerialReceiving)) {Priority = ThreadPriority.Normal};

			serThread.Name = "SerialHandle" + serThread.ManagedThreadId;
		}


		public MessageAdapterXbee(string p_port)
		{
			log.Trace ("({0})", p_port);

			_port = p_port;
			_baudRate = 57600;
			_lastReceive = DateTime.MinValue;

			serThread = new Thread(new ThreadStart(SerialReceiving)) {Priority = ThreadPriority.Normal};

		    serThread.Name = "SerialHandle" + serThread.ManagedThreadId;
		}

		public MessageAdapterXbee(string Port, int baudRate)
			: this(Port)
		{
			_baudRate = baudRate;
		}
		#endregion


		#region Properties
		public string Port
		{
			get { return _port; }
		}

		public int BaudRate
		{
			get { return _baudRate; }
		}
		public string ConnectionString
		{
			get
			{
				return String.Format("[Serial] Port: {0} | Baudrate: {1}",
				                     _serialPort.PortName, _serialPort.BaudRate.ToString());
			}
		}
		#endregion

		#region Methods

		#region Adepter Methods

		public override NodeType GetNodeType()
		{
			return NodeType.Xbee;
		}

		protected override void StartAdapter() 
		{
			log.Info ("Starting on {0} at {1}", _port, _baudRate);
			OpenConn ();
		}

		protected override void StopAdapter() 
		{
			log.Info ("Stopping");
			CloseConn ();
		}

		public override void SendMessage(MessageBase p_msg, Node p_destinationNode) {
			log.Trace ("Sending message: {0} to:{1}", p_msg.Api, p_destinationNode.Identifier);
			log.Trace (p_msg.ToString);

			switch(p_msg.Api)
			{
				case BitHomeProtocol.Api.DEVICE_STATUS_REQUEST:
				case BitHomeProtocol.Api.DEVICE_INFO_REQUEST:
				case BitHomeProtocol.Api.CATALOG_REQUEST:
				case BitHomeProtocol.Api.PARAMETER_REQUEST:
				case BitHomeProtocol.Api.FUNCTION_TRANSMIT:
				case BitHomeProtocol.Api.DATA_REQUEST:
				case BitHomeProtocol.Api.BOOTLOAD_TRANSMIT:
				{
					SendTxMessage((MessageTxBase)p_msg, p_destinationNode);
				}
				break;
			default:
				{
					log.Trace ("Sending unhandled message:{0} to {1}", p_msg.Api, p_destinationNode.Identifier);
				}
				break;
			}
		}

		private void SendTxMessage(MessageTxBase p_msg, Node p_destinationNode)
		{
			if (p_destinationNode.NodeType != NodeType.Xbee) {
				log.Warn ("Trying to send message to wrong device type {0} type {1}", 
				          p_destinationNode.Identifier, p_destinationNode.NodeType);
			}
			NodeXbee node = (NodeXbee)p_destinationNode;

			byte[] messageBytes = p_msg.GetBytes ();
			byte[] address16bytes = EBitConverter.GetBytes (node.Address16);
			byte[] address64bytes = EBitConverter.GetBytes (node.Address64);
			byte frameId = NextFrameId;

			// Size = 1 API + 
			// frame ID + 2 16addr + 8 64addr + radius +
			// options + n packetdata
			int size = 14 + messageBytes.Length;

			log.Debug ("Sending TX message: {0}", p_msg.Api);

			byte[] serialBytes = new byte[size+4];

			lock (m_sendLock) {
				int bi = 0; // byte index
				serialBytes[bi++] = (byte)Protocol.Api.START;
				serialBytes[bi++] = (byte)((size >> 8) & 0xff);
				serialBytes[bi++] = (byte)(size & 0xff);

				// Start the checksum here
				m_sendChecksum = 0;

				m_sendChecksum += serialBytes[bi++] = (byte)Protocol.Api.ZIGBEE_TX_REQ; // API
				m_sendChecksum += serialBytes[bi++] = frameId; // Frame ID

				// Addresses
				foreach (byte b in address64bytes) {
					m_sendChecksum += serialBytes [bi++] = b;
				}
				foreach (byte b in address16bytes) {
					m_sendChecksum += serialBytes [bi++] = b;
				}

				m_sendChecksum += serialBytes[bi++] = 0x00; // Broadcast Radius
				m_sendChecksum += serialBytes[bi++] = 0x00; // Options

				// Packet bytes
				foreach (byte b in messageBytes) {
					m_sendChecksum += serialBytes [bi++] = b;
				}

				serialBytes[bi++] = (byte)(0xFF - m_sendChecksum);

				// Send it off to the serial
				Transmit (serialBytes);
			}

//			
//
//				// Hash the message so we can correlate the send notification
//				synchronized(m_msgHashMap)
//				{
//					m_msgHashMap.put(frameId, p_msg);
//				}
//			}
//			catch (IOException e)
//			{
//				Logger.e(TAG, "error sending tx message", e);
//			}
		}


		public override void BroadcastMessage(MessageBase p_msg) {
		}

		private bool DecodePacketByte(byte data)
		{
			bool retVal = true;
//			log.Trace("Decode- byte: {0:X2}", data);

			// Decode based on the current packet state
			switch(m_packetState)
			{
				case PacketState.PacketStart:
				{
					// Check for the zigbee packet start (7e)
				if(data == (byte)Protocol.Api.START)
					{
//						log.Trace ("Decode- read packet start: {0:X2}", data);
						m_packetState = PacketState.SizeMsb;
					}
					else
					{
						// Drop it on the floor
					}
				}
				break;

				case PacketState.SizeMsb:
				{
//					log.Trace ("Decode- size MSB: {0:X2}", data);
					m_packetSize |= (data << 8);
					m_packetState = PacketState.SizeLsb;
				}
				break;

				case PacketState.SizeLsb:
				{
//					log.Trace ("Decode- size LSB: {0:X2}", data);
					m_packetSize |= data;

					// Initialize a new data buffer for this packet
					m_packetData = new byte[m_packetSize];

					//			Logger.v(TAG, "packet size:" + m_packetSize);
					m_packetState = PacketState.Api;
				}
				break;

				case PacketState.Api:
				{
//					log.Trace ("Decode- API: {0:X2}", data);
					// add to the checksum
					m_packetApi = data;
					m_packetChecksum += data;
					m_packetSize--; // API is part of packet size. We just want data size
					m_packetState = PacketState.Data;
				}
				break;

				case PacketState.Data:
				{
//					log.Trace ("Decode- data: {0:X2}", data);
					m_packetData[m_packetDataIndex++] = data;
					m_packetChecksum += data;

					// If the packet data index has overrun our packet size, move on
					if (m_packetDataIndex == m_packetSize)
					{
						m_packetState = PacketState.Checksum;
					}
				}
				break;

				case PacketState.Checksum:
				{
//					log.Trace ("Decode- checksum: {0:X2}", data);
					int checksum = 255 - m_packetChecksum;
					if (data == checksum)
					{
						MessageBase msgRx = MessageFactoryXbee.CreateMessage(m_packetApi, m_packetData);

						OnMessageRecieved (msgRx);
	//
	//					// If it is a TX response, process it. Otherwise fire a notification
	//					if (msgRx instanceof MsgXbeeTxStatus)
	//					{
	//						processTxStatus((MsgXbeeTxStatus)msgRx);
	//					}
	//					else
	//					{
	//						fireMsgReceivedEvent(msgRx);
	//					}
					}
					else
					{
						// If we are here, we are in trouble
						retVal = false;
						log.Error("Decoding failed. Checksum expected: {0:X} got {1:X}", data, checksum);
					}
					ResetDecode();
				}
				break;
			}
			return retVal;
		}

		private void ResetDecode()
		{
			m_packetState = PacketState.PacketStart;
			m_packetApi = 0;
			m_packetChecksum = 0;
			m_packetData = null;
			m_packetDataIndex = 0;
			m_packetSize = 0;
		}


		#endregion

		#region Port Control
		private bool OpenConn() 
		{
			// Get a list of serial port names. 
			string[] ports = SerialPort.GetPortNames();

			List<string> portList = new List<String> (ports);
			// Move this to the front of the list
			if (portList.Contains ("/dev/ttyUSB0")) {
				portList.Insert (0, "/dev/ttyUSB0");
			}

			// Display each port name to the console. 
			foreach(string port in portList)
			{
				if (port.Contains ("Bluetooth")) {
					continue;
				}

				if (port.Contains ("modem")) {
					continue;
				}

				log.Trace("Trying serial port at {0}", port);

				if (OpenConn (port)) {
					_port = port;

//					log.Trace("Found serial port at {0}", port);

					break;
				}

			}
			if (_serialPort != null && _serialPort.IsOpen) {
				return true;
			}
			return false;
		}

		private bool OpenConn(String port)
		{
			try
			{
				if (_serialPort == null)
					_serialPort = new SerialPort(port, _baudRate, Parity.None);

				if (_serialPort != null && !_serialPort.IsOpen)
				{
					_serialPort.ReadTimeout = -1;
					_serialPort.WriteTimeout = -1;

					_serialPort.Open();

					if (_serialPort.IsOpen)
						serThread.Start(); /*Start The Communication Thread*/
				}

				if (_serialPort == null) {
					return false;
				}
			}
			catch (Exception ex)
			{
				log.Error ("Error opening comm port {1}: {0}", ex.Message, port);

				_serialPort = null;

				return false;
			}

			return true;
		}
		private bool OpenConn(string port, int baudRate)
		{
			_port = port;
			_baudRate = baudRate;

			return OpenConn(_port);
		}
		private void CloseConn()
		{
			if (_serialPort != null && _serialPort.IsOpen)
			{
				serThread.Abort();

				_serialPort.Close();
			}
		}
		public bool ResetConn()
		{
			CloseConn();
			return OpenConn();
		}
		#endregion
		#region Transmit/Receive
		public void Transmit(byte[] packet)
		{
			StringBuilder sb = new StringBuilder ();
			foreach (byte b in packet) {
				sb.Append (String.Format("{0:X2}", b));
			}
			log.Trace ("Sending 0x{0}", sb.ToString ());

			if (_serialPort != null && _serialPort.IsOpen) {
				_serialPort.Write(packet, 0, packet.Length);
			}
		}
		public int Receive(byte[] bytes, int offset, int count)
		{
			int readBytes = 0;

			if (count > 0 && _serialPort != null && _serialPort.IsOpen)
			{
				readBytes = _serialPort.Read(bytes, offset, count);
			}

			return readBytes;
		}
		#endregion
		#region IDisposable Methods
		public void Dispose()
		{
			CloseConn();

			if (_serialPort != null)
			{
				_serialPort.Dispose();
				_serialPort = null;
			}
		}
		#endregion
		#endregion

		#region Threading Loops
		private void SerialReceiving()
		{
			if (_serialPort != null && _serialPort.IsOpen) {
				while (_serialPort.IsOpen)
				{
					int count = _serialPort.BytesToRead;

					/*Get Sleep Inteval*/
					TimeSpan tmpInterval = (DateTime.Now - _lastReceive);

					/*Form The Packet in The Buffer*/
					byte[] buf = new byte[count];
					int readBytes = Receive(buf, 0, count);
					bool success;

					if (readBytes > 0)
					{
	//					OnSerialReceiving(buf);
						foreach( byte data in buf ) {
							if (!DecodePacketByte (data)) {
								// If it returned false, decoding broke and we should move on
								_serialPort.DiscardInBuffer ();
								continue;
							}
						}
					}

					#region Frequency Control
					_PacketsRate = ((_PacketsRate + readBytes) / 2);

					_lastReceive = DateTime.Now;

					if ((double)(readBytes + _serialPort.BytesToRead) / 2 <= _PacketsRate)
					{
						if (tmpInterval.Milliseconds > 0)
							Thread.Sleep(tmpInterval.Milliseconds > freqCriticalLimit ? freqCriticalLimit : tmpInterval.Milliseconds);

						/*Testing Threading Model*/
	//					Diagnostics.Debug.Write(tmpInterval.Milliseconds.ToString());
	//					Diagnostics.Debug.Write(" - ");
	//					Diagnostics.Debug.Write(readBytes.ToString());
	//					Diagnostics.Debug.Write("\r\n");
					}
					#endregion
				}
			}

		}
		#endregion

	}
}

