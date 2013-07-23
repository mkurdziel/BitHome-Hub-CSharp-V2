using System;
using BitHome.Messaging;

namespace BitHome.Messaging.Messages
{
	public abstract class MessageTxBase : MessageBase
	{

		public MessageTxBase () : base (null, null)
		{
		}

		public abstract byte[] GetBytes ();
	}
}
