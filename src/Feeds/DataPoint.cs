using System;

namespace BitHome
{
	[Serializable]
	public class DataPoint
	{
		public String Value { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}

