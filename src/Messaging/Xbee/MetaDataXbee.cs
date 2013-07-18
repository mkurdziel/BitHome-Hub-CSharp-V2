using System;

namespace BitHome
{
	[Serializable]
	public class MetaDataXbee
	{
		private UInt16 Address16 { get; set; }
		private UInt64 Address64 { get; set; }

		public MetaDataXbee() {
		}

		public MetaDataXbee (UInt16 p_address16, UInt64 p_address64)
		{
			Address16 = p_address16;
			Address64 = p_address64;
		}
	}
}

