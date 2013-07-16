using System;
using NLog;

using BitHome.Messaging.Messages;
using BitHome.Messaging.Xbee;
using System.Collections.Generic;

namespace BitHome.Messaging
{
	public class MessageDispatcherService
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		private List<MessageAdapterBase> m_adapters = new List<MessageAdapterBase>();

		public event EventHandler<MessageRecievedEventArgs>  MessageRecieved;

		public MessageDispatcherService ()
		{
			log.Trace ("()");
		}

		public bool Start() {
			log.Info ("Starting MessageDispatcherService");

			MessageAdapterXbee xbeeAdapter = new MessageAdapterXbee ("/dev/tty.usbserial-AH0015BR");
			m_adapters.Add (xbeeAdapter);


			// Start adapters
			foreach (MessageAdapterBase adapter in m_adapters) 
			{
				adapter.MessageRecieved += OnAdapterMessageRecieved; 
				adapter.Start ();
			}

			return true;
		}

		public void Stop() {
			log.Info ("Stopping MessageDispatcherService");
		}

		private void OnMessageRecieved(MessageBase p_msg) {
			// Make a temporary copy of the event to avoid possibility of 
			// a race condition if the last subscriber unsubscribes 
			// immediately after the null check and before the event is raised.
			EventHandler<MessageRecievedEventArgs> handler = MessageRecieved;
			if (handler != null)
			{
				handler(this, new MessageRecievedEventArgs(p_msg));
			}
		}

		private void OnAdapterMessageRecieved (object sender, MessageRecievedEventArgs e)
		{
			log.Debug ("Message recieved from adapter {0}", sender);
		}
	}
}

