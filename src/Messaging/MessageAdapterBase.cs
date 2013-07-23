using System;
using NLog;
using BitHome.Messaging.Messages;

namespace BitHome.Messaging
{
	public abstract class MessageAdapterBase
	{
		private static Logger log = LogManager.GetCurrentClassLogger();
		
		#region Custom Events

		public event EventHandler<MessageRecievedEventArgs> MessageRecieved;

		#endregion

		public abstract Node BroadcastNode {
			get;
		}

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

		public abstract NodeType GetNodeType (); 

		protected abstract void StartAdapter();

		protected abstract void StopAdapter();

		public abstract void SendMessage(MessageBase p_msg, Node p_destinationNode);

		public abstract void BroadcastMessage(MessageBase p_msg);

		
		protected void OnMessageRecieved(MessageBase p_msg)
		{
			// Make a temporary copy of the event to avoid possibility of 
			// a race condition if the last subscriber unsubscribes 
			// immediately after the null check and before the event is raised.
			EventHandler<MessageRecievedEventArgs> handler = MessageRecieved;
			if (handler != null && p_msg != null)
			{
				handler(this, new MessageRecievedEventArgs(p_msg));
			}
		}
	}
}

