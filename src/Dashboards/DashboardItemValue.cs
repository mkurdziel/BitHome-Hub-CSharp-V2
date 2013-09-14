using System;

namespace BitHome
{
	[Serializable]
	public class DashboardItemValue
	{
		public string Id { 
			get {
				if (ParameterId != null) {
					return ParameterId;
				} else {
					return DataStreamId;
				}
			}
		}
		public string DataStreamId { get; set; }
		public string ParameterId { get; set; }
		public string Name { get; set; }
		public String Constant { get; set; }
	}
}
