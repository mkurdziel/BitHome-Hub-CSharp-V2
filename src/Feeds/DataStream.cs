using System;
using System.Collections.Generic;
using BitHome;
using BitHome.Messaging.Protocol;

namespace BitHome.Feeds
{
	[Serializable]
	public class DataStream
	{
		private List<DataPoint> m_dataList = new List<DataPoint>();

		public String Id { get; set; }
		public String StoreId { get; set; }
		public DataType DataType { get; set; }
		public String MinValue { get; set; }
		public String MaxValue { get; set; }
		private DataPoint MostRecent { get; set; }
		private Version Version { get; set; }

		public DateTime LastModified { 
			get {
				if (MostRecent != null) {
					return MostRecent.TimeStamp;
				}
				return DateTime.MinValue;
			}
		}

		public String Value {
			get {
				if (MostRecent == null && m_dataList.Count > 0) {
					MostRecent = m_dataList [m_dataList.Count - 1];
				}

				if (MostRecent != null) {
					return MostRecent.Value;
				}
				return null;	
			}
		}

		public DataStream() {
			m_dataList = new List<DataPoint> ();
		}

		public DataStream( String id ) : this()
		{
			Id = id;
			DataType = DataType.STRING;

		}

		public void AddData ( String data ) {
			DataPoint datapoint = new DataPoint { TimeStamp = DateTime.Now, Value = data };
			m_dataList.Add (datapoint);
			MostRecent = datapoint;
		}
	}
}

