using BitHome;
using System;
using System.Collections.Generic;
using BitHome.Helpers;
using ServiceStack.Text;
using NLog;

namespace BitHome.Feeds
{
	public class FeedService : IBitHomeService
	{
		private const string KEY_FEEDS = "feeds";
		private const int FEED_ID_LENGTH = 42;

		private static Logger log = LogManager.GetCurrentClassLogger();

		public static Version Version = new Version(0, 1);


		private HashSet<string> m_feedKeys = new HashSet<string>();
		private Dictionary<long, Feed> m_feeds = new Dictionary<long, Feed>();

		public Feed[] Feeds { 
			get {
				return m_feeds.Values.ToArray ();
			}
		}

		private String[] FeedStorageIds {
			get {
				return m_feedKeys.ToArray ();
			}
		}


		public FeedService ()
		{
			LoadData ();
		}

		private void LoadData() 
		{
			// Load data from the storage service
			if (StorageService.Store<string[]>.Exists(KEY_FEEDS)) {
				string[] feedStoreIds = StorageService.Store<string[]>.Get (KEY_FEEDS);

				foreach (string storeId in feedStoreIds) 
				{
					if (StorageService.Store<Feed>.Exists (storeId)) {
						try {
							Feed feed = StorageService.Store<Feed>.Get (storeId);
							m_feeds.Add (feed.Id, feed);
							m_feedKeys.Add (feed.StoreId);
							log.Debug ("Loading feed {0}", feed.Id);
						} catch (Exception) {
							log.Error ("Could not decode feed {0}", storeId);
							UnSaveFeed (storeId);
						}
					} else {
						log.Warn ("Feed not found for key {0}", storeId);
					}
				}

				// Reload the datastreams
				foreach (Feed feed in m_feeds.Values) {
					foreach (String dataStreamId in feed.DataStreamStorageIds) {
						if (StorageService.Store<DataStream>.Exists(dataStreamId)) {
							try {
								DataStream dataStream = StorageService.Store<DataStream>.Get(dataStreamId);
								feed.AddDataStream (dataStream);
							} catch (Exception) {
								log.Error ("Could not decode datastream {0}", dataStreamId);
							}
						} else {
							log.Warn ("DataStream not found for key {0}", dataStreamId);
						}
					}
				}
			}
		}


		#region IBitHomeService implementation

		public bool Start ()
		{
			log.Info ("Starting FeedService");
			return true;
		}

		public void Stop ()
		{
			log.Info ("Stoppign FeedService");
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
			feed.StoreId = StorageService.GenerateKey ();
		    feed.Name = name;

			log.Debug ("Creating feed {0} with name {1}", feed.Id, feed.Name);

			m_feeds.Add (feed.Id, feed);
			m_feedKeys.Add (feed.StoreId);

			SaveFeed (feed);
			SaveFeedList ();

			return feed;
		}

		public bool DeleteFeed(long feedId) {
			Feed feed = GetFeed (feedId);

			if (feed != null) {
				log.Debug ("Deleting feed {0} with name {1}", feed.Id, feed.Name);
				m_feeds.Remove (feedId);
				m_feedKeys.Remove (feed.StoreId);
				UnSaveFeed (feed.StoreId);
				SaveFeedList ();
				return true;
			}
			return false;
		}

		public DataStream CreateDataStream(long feedId, String name) {
			Feed feed = GetFeed (feedId);

			if (feed != null) {
				DataStream dataStream = new DataStream (name);
				dataStream.StoreId = StorageService.GenerateKey ();

				log.Debug ("Creating datastream {0} for feed {1} {2}", dataStream.Id, feed.Id, feed.Name);

				feed.AddDataStream (dataStream);

				SaveFeed (feed);
				SaveDataStream (dataStream);

				return dataStream;
			}
			return null;
		}

		public DataStream DeleteDataStream(long feedId, String dataStreamId) {
			Feed feed = GetFeed (feedId);

			if (feed != null) {
				DataStream dataStream = feed.GetDataStream (dataStreamId);

				return DeleteDataStream (feed, dataStream);
			}
			return null;
		}

		public DataStream DeleteDataStream(Feed feed, DataStream dataStream) {
			if (feed != null && dataStream != null) {
				log.Debug ("Deleting datastream {0} for feed {1} {2}", dataStream.Id, feed.Id, feed.Name);

				feed.DeleteDataStream (dataStream.Id);
				UnSaveDataStream (dataStream.StoreId);
				SaveFeed (feed);
				return dataStream;
			}
			return null;
		}

		public void SetDataStreamValue (Feed feed, DataStream dataStream, string value)
		{
			if (dataStream != null) {
				log.Trace ("Setting datastream value {0}:{1} {2}", feed.Name, dataStream.Id, value);
				dataStream.AddData (value);
				SaveDataStream (dataStream);
			}
		}

        public void SetMaxValue(Feed feed, DataStream dataStream, string maxValue)
        {
            if (dataStream != null)
            {
                log.Trace("Setting datastream max value {0}:{1} {2}", feed.Name, dataStream.Id, maxValue);
                dataStream.MaxValue = maxValue;
                SaveDataStream(dataStream);
            }
        }

        public void SetMinValue(Feed feed, DataStream dataStream, string minValue)
        {
            if (dataStream != null)
            {
                log.Trace("Setting datastream min value {0}:{1} {2}", feed.Name, dataStream.Id, minValue);
                dataStream.MaxValue = minValue;
                SaveDataStream(dataStream);
            }
        }

		#region Persistence Methods

		private void SaveFeedList() {
			StorageService.Store<String[]>.Insert (KEY_FEEDS, this.FeedStorageIds);
		}

		private void SaveFeed(Feed feed) {
			StorageService.Store<Feed>.Insert (feed.StoreId, feed);
		}

		private void UnSaveFeed(String feedStorageId) {
			StorageService.Store<Feed>.Remove (feedStorageId);
		}

		private void SaveDataStream(DataStream dataStream) {
			StorageService.Store<DataStream>.Insert (dataStream.StoreId, dataStream);
		}

		private void UnSaveDataStream(String dataStreamStoreId) {
			StorageService.Store<DataStream>.Remove (dataStreamStoreId);
		}

		public void WaitFinishSaving ()
		{
			StorageService.Store<string[]>.WaitForCompletion ();
			StorageService.Store<Feed>.WaitForCompletion ();
			StorageService.Store<DataStream>.WaitForCompletion ();
		}
		#endregion


	}
}

