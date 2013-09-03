using System;
using BitHomeProtocol = BitHome.Messaging.Protocol;

namespace BitHome.Messaging.Messages
{
	public class MessageDeviceInfoRequest : MessageTxBase
	{
		public override BitHomeProtocol.Api Api {
			get {
				return BitHomeProtocol.Api.DEVICE_INFO_REQUEST;
			}
		}

		public MessageDeviceInfoRequest () 
		{
		}

		public override byte[] GetBytes ()
		{
			byte[] bytes = new byte[2];
			bytes [0] = (byte)BitHomeProtocol.PacketValues.PACKET_START;
			bytes [1] = (byte)BitHomeProtocol.Api.DEVICE_INFO_REQUEST;

			return bytes;
		}

		public override string ToString ()
		{
			return string.Format ("[MessageDeviceInfoRequest: Api={0}]", Api);
		}
	}
}

