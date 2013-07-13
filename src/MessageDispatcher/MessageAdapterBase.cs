using System;
using NLog;
using BitHome.Messaging.Messages;

namespace BitHome.Messaging
{
	public abstract class MessageAdapterBase
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public MessageAdapterBase ()
		{
			log.Trace ("()");
		}

		public bool Start() {
			log.Info ("Starting MessageAdapterBase");

			StartAdapter ();

			return true;
		}

		public void Stop() {
			log.Info ("Stopping MessageAdapterBase");

			StopAdapter ();
		}

//		public abstract class getNodeClass();
		protected abstract void StartAdapter();

		protected abstract void StopAdapter();

		public abstract String NodeTypeIdentifierString {
			get;
		}

		public abstract void SendMessage(MessageBase p_msg);

		public abstract void BroadcastMessage(MessageBase p_msg);
	}
}

