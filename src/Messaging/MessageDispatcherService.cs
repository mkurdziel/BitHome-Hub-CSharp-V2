using System;
using NLog;

using BitHome.Messaging.Messages;
using BitHome.Messaging.Xbee;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace BitHome.Messaging
{
	public class MessageDispatcherService
	{
		private const int MESSAGE_QUEUE_SIZE = 100;

		private static Logger log = LogManager.GetCurrentClassLogger();

		private List<MessageAdapterBase> m_adapters = new List<MessageAdapterBase>();

		public event EventHandler<MessageRecievedEventArgs>  MessageRecieved;

		private MessageAdapterBase m_xbeeAdapter;
		private bool m_isTesting = false;

		// Create blocking queues for the sender threads
		private BlockingCollection<MessageBase> m_messageQueueIn = new BlockingCollection<MessageBase>(new ConcurrentQueue<MessageBase>(), MESSAGE_QUEUE_SIZE );
		private BlockingCollection<MessageBase> m_messageQueueOut = new BlockingCollection<MessageBase>(new ConcurrentQueue<MessageBase>(), MESSAGE_QUEUE_SIZE );

		// Message handling threads
		private Thread m_messageThreadIn;
		private Thread m_messageThreadOut;

		private bool m_isRunning = false;

		public MessageDispatcherService ()
		{
			log.Trace ("()");

			Init ();
		}

		public MessageDispatcherService (bool p_isTesting)
		{
			m_isTesting = p_isTesting;

			log.Trace ("() Testing:{0}", p_isTesting);

			Init ();
		}

		private void Init()
		{
			m_messageThreadIn = new Thread (MessageQueueInThread);
			m_messageThreadIn.Name = "MessageDispatcherService Input Thread";
			m_messageThreadIn.IsBackground = true;

			m_messageThreadOut = new Thread (MessageQueueOutThread);
			m_messageThreadOut.Name = "MessageDispatcherService Output Thread";
			m_messageThreadOut.IsBackground = true;
		}

		public bool Start() {
			log.Info ("Starting MessageDispatcherService");

			m_isRunning = true;

			if (m_isTesting == false) {
//				m_xbeeAdapter = new MessageAdapterXbee ("COM3");
                m_xbeeAdapter = new MessageAdapterXbee ("/dev/tty.usbserial-A601D9KI");
//				m_xbeeAdapter = new MessageAdapterXbee ("/dev/tty.usbserial-A6007WWJ");
//				m_xbeeAdapter = new MessageAdapterXbee ("/dev/tty.usbserial-AH0015BR");

				m_adapters.Add (m_xbeeAdapter);

				// Start adapters
				foreach (MessageAdapterBase adapter in m_adapters) {
					adapter.MessageRecieved += OnAdapterMessageRecieved; 
					adapter.Start ();

				}

				m_messageThreadOut.Start ();
			}
			
			m_messageThreadIn.Start ();


			return true;
		}

		public void Stop() {
			log.Info ("Stopping MessageDispatcherService");

			m_isRunning = false;

			if (m_isTesting == false) {
				m_messageThreadIn.Join ();
				m_messageThreadOut.Join ();
			}
		}

		public void BroadcastMessage (MessageTxBase p_message)
		{
			p_message.DestinationNode = null;

			m_messageQueueOut.Add (p_message);
		}

		public void SendMessage (MessageTxBase p_message, Node p_destinationNode)
		{
			p_message.DestinationNode = p_destinationNode;

			m_messageQueueOut.Add (p_message);
		}


        public void SendMessage(MessageTxBase message, string destinationNodeId)
        {
            Node node = ServiceManager.NodeService.GetNode(destinationNodeId);
            if (node != null)
            {
                SendMessage(message, node);
            }
            else
            {
                log.Warn("Trying to send {0} message to unknown node id {1}", message.Api, destinationNodeId);
            }
        }

	    public void ReceiveMessage(MessageBase p_msg) 
		{
			m_messageQueueIn.Add (p_msg);
		}

		public MessageBase TakeNextMessageIn()
		{
			MessageBase msg;
			m_messageQueueIn.TryTake(out msg, TimeSpan.FromSeconds(5));
			return msg;
		}

		public int MessageInQueueCount {
			get {
				return m_messageQueueIn.Count;
			}
		}
		
		public MessageBase TakeNextMessageOut()
		{
			MessageBase msg;
			m_messageQueueOut.TryTake(out msg, TimeSpan.FromSeconds(5));
			return msg;
		}

		public int MessageOutQueueCount {
			get {
				return m_messageQueueOut.Count;
			}
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
			m_messageQueueIn.Add (e.Message);
		}

		private void MessageQueueInThread()
		{
			log.Info ("Starting message input thread");
			try {
				MessageBase msg;
				while (m_isRunning) {
					msg = m_messageQueueIn.Take ();

					OnMessageRecieved(msg);
				}
			} catch (ThreadInterruptedException) {
				log.Info ("Message input thread interrupted");
			}
			log.Info ("Stopping message input thread");
		}

		private void MessageQueueOutThread()
		{
			log.Info ("Starting message output thread");
			try {
				MessageBase msg;
				while (m_isRunning) {
					msg = m_messageQueueOut.Take ();

					if (msg.DestinationNode == null ) {
						foreach( MessageAdapterBase adapter in m_adapters )
						{
							adapter.SendMessage (msg, adapter.BroadcastNode);
						}
					} else {
						foreach( MessageAdapterBase adapter in m_adapters )
						{
							if (adapter.GetNodeType() == msg.DestinationNode.NodeType) {
								adapter.SendMessage (msg, msg.DestinationNode);
							}
						}
					}
				}
			} catch (ThreadInterruptedException) {
				log.Info ("Message output thread interrupted");
			}
			log.Info ("Stopping message output thread");
		}

	}
}
