using System;
using BitHome.Messaging;

namespace BitHome.Messaging.Messages
{
	public class MessageParameterRequest : MessageTxBase
	{
		public byte ActionIndex { private set; get; }
		public byte ParameterIndex { private set; get; }

		public MessageParameterRequest (byte p_functionIndex, byte p_parameterIndex)
		{
			ActionIndex = p_functionIndex;
			ParameterIndex = p_parameterIndex;
		}

		#region implemented abstract members of MessageTxBase

		public override byte[] GetBytes ()
		{
			byte[] bytes = new byte[4];
			bytes [0] = (byte)Protocol.PacketValues.PACKET_START;
			bytes [1] = (byte)Api;
			bytes [2] = ActionIndex;
			bytes [3] = ParameterIndex;

			return bytes;	
		}

		#endregion

		#region implemented abstract members of MessageBase

		public override Protocol.Api Api {
			get { return Protocol.Api.PARAMETER_REQUEST; }
		}

		#endregion
	}
}

