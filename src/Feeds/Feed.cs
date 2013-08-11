using System;
using System.Collections.Generic;
using ServiceStack.Text;
using System.Runtime.Serialization;

namespace BitHome.Feeds
{
	[Serializable]
	public class Feed
	{
		private HashSet<String> m_dataStreamKeys = new HashSet<string>();

		[NonSerialized]
		private Dictionary<String, DataStream> m_dataStreams = new Dictionary<String, DataStream> ();

	    private Version m_version;

	    public long Id { get; set; }
		public String StoreId { get; set; }
		public String Name { get; set; }
		public bool IsPrivate { get; set; }
		public String Url { get; set; }
		public FeedStatus Status { get; set; }
		public DateTime Created { get; set; }
		public long CreatedMilliseconds
		{
		    get { return Created.ToUnixTimeMs(); }
            set { Created = new DateTime(value); }
		}

	    public DateTime Updated { get; set; }
	    public long UpdatedMilliseconds
	    {
            get { return Updated.ToUnixTimeMs(); }
            set { Updated = new DateTime(value); }
	    }

	    public Version Version
		{
		    get { return new Version(0, 1); }
		    set { m_version = value; }
		}

		public String[] DataStreamStorageIds {
			get {
				return m_dataStreamKeys.ToArray ();
			}
			set {
				m_dataStreamKeys = new HashSet<string> (value);
			}
		}

//	    public DataStream[] DataStreams {
//			get {
//				return m_dataStreams.Values.ToArray();
//			}
//		}



		public Feed() {
			m_dataStreams = new Dictionary<string, DataStream> ();

			Created = DateTime.Now;
			Updated = DateTime.Now;

			IsPrivate = true;
			Status = FeedStatus.Unknown;
			Created = DateTime.Now;
			Updated = DateTime.Now;

		}

		public DataStream[] GetDataStreams() {
			return m_dataStreams.Values.ToArray ();
		}

		public DataStream GetDataStream(String dataStreamId) {
			if (m_dataStreams.ContainsKey(dataStreamId)) {
				return m_dataStreams[dataStreamId];
			}
			return null;
		}

		public bool AddDataStream(DataStream stream) {
			if (stream != null && m_dataStreams.ContainsKey (stream.Id) == false) {
				m_dataStreams.Add (stream.Id, stream);
				if (!m_dataStreamKeys.Contains (stream.StoreId)) {
					m_dataStreamKeys.Add (stream.StoreId);
				}
				return true;
			}
			return false;
		}

		public DataStream DeleteDataStream(String dataStreamId) {
			DataStream dataStream = GetDataStream (dataStreamId);

			if (dataStream != null) {
				m_dataStreams.Remove (dataStream.Id);
				m_dataStreamKeys.Remove (dataStream.StoreId);
				return dataStream;
			}
			return null;
		}

		[OnDeserialized]
		private void SetOnDeserialized(StreamingContext context)
		{
			m_dataStreams = new Dictionary<string, DataStream> ();
		}
	}
}

