using System;
using BitHomeProtocol = BitHome.Messaging.Protocol;

namespace BitHome.Messaging.Messages
{
	public class MessageDeviceStatusRequest : MessageTxBase
	{
		private BitHomeProtocol.DeviceStatusRequest m_requestType;

		public override BitHomeProtocol.Api Api {
			get {
				return BitHomeProtocol.Api.DEVICE_STATUS_REQUEST;
			}
		}

		public BitHomeProtocol.DeviceStatusRequest RequestType {
			get { return m_requestType; }
		}

		public MessageDeviceStatusRequest (BitHomeProtocol.DeviceStatusRequest p_requestType) 
		{
			m_requestType = p_requestType;
		}

		public override byte[] GetBytes ()
		{
			byte[] bytes = new byte[3];
			bytes [0] = (byte)BitHomeProtocol.PacketValues.PACKET_START;
			bytes [1] = (byte)BitHomeProtocol.Api.DEVICE_STATUS_REQUEST;
			bytes [2] = (byte)m_requestType;

			return bytes;
		}
	}
}

