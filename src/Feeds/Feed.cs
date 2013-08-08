using System;
using System.Collections.Generic;
using ServiceStack.Text;

namespace BitHome.Feeds
{
	[Serializable]
	public class Feed
	{
		private Dictionary<long, DataStream> m_dataStreams = new Dictionary<long, DataStream> ();

		public String Id { get; set; }
		public String Name { get; set; }
		public bool IsPrivate { get; set; }
		public String Url { get; set; }
		public FeedStatus Status { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
		public Version Version { get; set; }

		public DataStream[] DataStreams {
			get {
				return m_dataStreams.Values.ToArray();
			}
		}

		public Feed() {
			IsPrivate = true;
			Status = FeedStatus.Unknown;
			Created = DateTime.Now;
			Updated = DateTime.Now;

		}

		public DataStream GetDataStream(long dataStreamId) {
			if (m_dataStreams.ContainsKey(dataStreamId)) {
				return m_dataStreams[dataStreamId];
			}
			return null;
		}

		public bool AddDataStream(DataStream stream) {
			if (stream != null && m_dataStreams.ContainsKey (stream.Id) == false) {
				m_dataStreams.Add (stream.Id, stream);
				return true;
			}
			return false;
		}

		public DataStream DeleteDataStream(long dataStreamId) {
			DataStream dataStream = GetDataStream (dataStreamId);

			if (dataStream != null) {
				m_dataStreams.Remove (dataStream.Id);
				return dataStream;
			}
			return null;
		}
	}
}

