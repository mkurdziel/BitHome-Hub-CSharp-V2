using System;
using BitHome.Feeds;

namespace BitHome.WebApi
{
	public class WebFeed
	{
		private Feed m_feed;

		public WebFeed (Feed feed)
		{
			m_feed = feed;
		}

		public long Id { get { return m_feed.Id; } }
		public String Name { get { return m_feed.Name; } }
		public bool IsPrivate { get { return m_feed.IsPrivate; } }
		public String Url { get { return m_feed.Url; } }
		public FeedStatus Status { get { return m_feed.Status; } }
		public String Created { get { return m_feed.Created.ToString(); } }
		public String Updated { get { return m_feed.Updated.ToString (); } }
		public Version Version { get { return m_feed.Version; } }
	}
}

