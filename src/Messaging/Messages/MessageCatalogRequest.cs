using System;
using BitHomeProtocol = BitHome.Messaging.Protocol;

namespace BitHome.Messaging.Messages
{
	public class MessageCatalogRequest : MessageTxBase
	{
		public byte ActionIndex { get; private set; }

		public MessageCatalogRequest (byte p_functionNum) 
		{
			ActionIndex = p_functionNum;
		}

		#region implemented abstract members of MessageTxBase

		public override byte[] GetBytes ()
		{
			byte[] bytes = new byte[3];
			bytes [0] = (byte)BitHomeProtocol.PacketValues.PACKET_START;
			bytes [1] = (byte)Api;
			bytes [2] = ActionIndex;

			return bytes;	
		}

		#endregion

		#region implemented abstract members of MessageBase

		public override Protocol.Api Api {
			get { return BitHomeProtocol.Api.CATALOG_REQUEST; }
		}

		#endregion

		public override string ToString ()
		{
			return string.Format ("[MessageCatalogRequest: ActionIndex={0}]", ActionIndex);
		}
	}
}

