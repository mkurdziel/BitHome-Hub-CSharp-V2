using System;
using NLog;

namespace BitHome.Messaging.Messages
{
	public abstract class MessageBase
	{
		private Logger log = LogManager.GetCurrentClassLogger();
		private String m_sourceNodeKey;
		private String m_destNodeKey;

		private DateTime m_timeStamp;

		public DateTime TimeStamp {
			get { return m_timeStamp; }
		}

		public MessageBase () : this(new DateTime())
		{
		}

		public MessageBase(DateTime p_timeStamp) {
			log.Debug ("Constructor: {0}", p_timeStamp);

			m_timeStamp = p_timeStamp;
		}
	}
}
