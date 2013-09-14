using System;
using BitHomeProtocol = BitHome.Messaging.Protocol;
using BitHome.Messaging.Protocol;
using System.Collections.Generic;

namespace BitHome.Messaging.Messages
{
	public class MessageDataRequest : MessageTxBase
	{
		public HashSet<Tuple<byte, byte>> Parameters { get; private set; }
		public DataRequestType RequestType { get; private set; }
		public UInt16 Time { get; private set; }

		public MessageDataRequest (byte actionIndex, byte parameterIndex) : 
			this(actionIndex, parameterIndex, DataRequestType.POLL_REQUEST, 0)
		{
		}

		public MessageDataRequest (byte actionIndex, byte parameterIndex, DataRequestType requestType, UInt16 time)
		{
			Parameters = new HashSet<Tuple<byte, byte>> ();
			Parameters.Add (new Tuple<byte, byte> (actionIndex, parameterIndex));
			RequestType = requestType;
			Time = time;
		}

		public MessageDataRequest (HashSet<Tuple<byte, byte>> parameters, DataRequestType requestType, UInt16 time)
		{
			Parameters = parameters;
			RequestType = requestType;
			Time = time;
		}

		#region implemented abstract members of MessageTxBase

		public override byte[] GetBytes ()
		{
			byte[] bytes = new byte[7+(Parameters.Count*2)];
			byte[] timeBytes = EBitConverter.GetBytes (Time);

			bytes [0] = (byte)BitHomeProtocol.PacketValues.PACKET_START;
			bytes [1] = (byte)Api;
			bytes [2] = (byte)Parameters.Count;
			bytes [3] = (byte)RequestType;
			bytes [4] = 0x00; // options
			bytes [5] = timeBytes [0];
			bytes [6] = timeBytes [1];
			int index = 7;
			foreach (Tuple<byte, byte> pair in Parameters) {
				bytes [index++] = pair.Item1;
				bytes [index++] = pair.Item2;
			}

			return bytes;	
		}

		#endregion

		#region implemented abstract members of MessageBase

		public override Protocol.Api Api {
			get { return BitHomeProtocol.Api.DATA_REQUEST; }
		}

		#endregion
		public override string ToString ()
		{
			return string.Format ("[MessageDataRequest: RequestType={0}, Time={1}]", 
			                      RequestType, Time);
		}
	}
}
