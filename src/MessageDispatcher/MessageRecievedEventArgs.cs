using System;
using BitHome.Messaging.Messages;

namespace BitHome.Messaging
{
	public class MessageRecievedEventArgs : EventArgs
	{
		private MessageBase m_msg;

		public MessageBase Message 
		{
			get { return m_msg; }
		}

		public MessageRecievedEventArgs (MessageBase p_msg)
		{
			m_msg = p_msg;
		}
	}
}

