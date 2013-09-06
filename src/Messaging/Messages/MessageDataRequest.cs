using System;
using BitHomeProtocol = BitHome.Messaging.Protocol;

namespace BitHome.Messaging.Messages
{
	public class MessageDataRequest : MessageTxBase
	{
		public byte ActionIndex { get; private set; }
		public byte ParameterIndex { get; private set; }

		public MessageDataRequest (byte actionIndex, byte parameterIndex)
		{
			ActionIndex = actionIndex;
			ParameterIndex = parameterIndex;
		}

		#region implemented abstract members of MessageTxBase

		public override byte[] GetBytes ()
		{
			byte[] bytes = new byte[4];
			bytes [0] = (byte)BitHomeProtocol.PacketValues.PACKET_START;
			bytes [1] = (byte)Api;
			bytes [2] = ActionIndex;
			bytes [3] = ParameterIndex;

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
			return string.Format ("[MessageDataRequest: ActionIndex={0}, ParameterIndex={1}]", 
			                      ActionIndex, ParameterIndex);
		}
	}
}
