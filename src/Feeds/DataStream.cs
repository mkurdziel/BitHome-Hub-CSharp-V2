using System;
using System.Collections.Generic;
using BitHome;
using BitHome.Messaging.Protocol;

namespace BitHome.Feeds
{
	[Serializable]
	public class DataStream
	{
		private SortedSet<DataPoint> m_data = new SortedSet<DataPoint> ();

		public long Id { get; set; }
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
				if (MostRecent != null) {
					return MostRecent.Value;
				}
				return null;	
			}
		}

		public DataStream( long id )
		{
			Id = id;
			DataType = DataType.STRING;
		}

		public void AddData ( String data ) {
			DataPoint datapoint = new DataPoint { TimeStamp = DateTime.Now, Value = data };
			m_data.Add (datapoint);
			MostRecent = datapoint;
		}
	}
}

