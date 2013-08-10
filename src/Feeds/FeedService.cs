using BitHome;
using System;
using System.Collections.Generic;
using BitHome.Helpers;
using ServiceStack.Text;

namespace BitHome.Feeds
{
	public class FeedService : IBitHomeService
	{
		private const int FEED_ID_LENGTH = 42;

		public static Version Version = new Version(0, 1);


		private Dictionary<long, Feed> m_feeds = new Dictionary<long, Feed>();

		public Feed[] Feeds { 
			get {
				return m_feeds.Values.ToArray ();
			}
		}

		public FeedService ()
		{
		}

		#region IBitHomeService implementation

		public bool Start ()
		{
			return true;
		}

		public void Stop ()
		{
			throw new NotImplementedException ();
		}

		public void WaitFinishSaving ()
		{
		}

		#endregion

		public Feed GetFeed(long feedId) {
			if (m_feeds.ContainsKey (feedId)) {
				return m_feeds [feedId];
			}
			return null;
		}

		public Feed CreateFeed(string name) {
			Feed feed = new Feed ();
			feed.Id = DataHelpers.RandomLongPositive ();
		    feed.Name = name;

			m_feeds.Add (feed.Id, feed);

			return feed;
		}

		public bool DeleteFeed(long feedId) {
			Feed feed = GetFeed (feedId);

			if (feed != null) {
				m_feeds.Remove (feedId);
				return true;
			}
			return false;
		}

		public DataStream CreateDataStream(long feedId, String name) {
			Feed feed = GetFeed (feedId);

			if (feed != null) {
				DataStream dataStream = new DataStream (name);
				feed.AddDataStream (dataStream);

				return dataStream;
			}
			return null;
		}
	}
}

