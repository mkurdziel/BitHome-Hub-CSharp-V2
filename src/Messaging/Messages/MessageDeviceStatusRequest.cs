using System;
using BitHomeProtocol = BitHome.Messaging.Protocol;

namespace BitHome.Messaging.Messages
{
	public class MessageDeviceStatusRequest : MessageTxBase
	{
		public override BitHomeProtocol.Api Api {
			get {
				return BitHomeProtocol.Api.DEVICE_STATUS_REQUEST;
			}
		}

		public MessageDeviceStatusRequest () 
		{
		}

		public override byte[] GetBytes ()
		{
			byte[] bytes = new byte[2];
			bytes [0] = (byte)BitHomeProtocol.PacketValues.PACKET_START;
			bytes [1] = (byte)Api;

			return bytes;
		}

		public override string ToString ()
		{
			return string.Format ("[MessageDeviceStatusRequest: Api={0}]", Api);
		}
	}
}

