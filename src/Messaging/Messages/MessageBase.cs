using System;
using NLog;

namespace BitHome.Messaging.Messages
{
	public abstract class MessageBase
	{
		private Node m_sourceNode;
		private Node m_destNode;

		private DateTime m_timeStamp;

		public abstract Protocol.Api Api 
		{
			get;
		}

		public Node SourceNode {
			get {
				return m_sourceNode;
			}
		}

		public Node DestinationNode {
			get {
				return m_destNode;
			}

			set {
				m_destNode = value;
			}
		}

		public DateTime TimeStamp {
			get { return m_timeStamp; }
		}

		public MessageBase (Node p_sourceNode, Node p_destNode) : this(DateTime.Now)
		{
		    m_sourceNode = p_sourceNode;
		    m_destNode = p_destNode;
		}

		public MessageBase(DateTime p_timeStamp) {
//			log.Debug ("Constructor: {0}", p_timeStamp);
			m_timeStamp = p_timeStamp;
		}
	}
}
