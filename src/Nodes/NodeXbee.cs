using System;
using NLog;

namespace BitHome
{
	[Serializable]
	public class NodeXbee : Node
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public UInt64 Address64 { get; set; }
		public UInt16 Address16 { get; set; }

		public String Address16String { 
            get { return String.Format("0x{0:X4}", Address16); }
        }

		public String Address64String { 
            get { return String.Format("0x{0:X16}", Address64); }
        }

		public NodeXbee ()
		{
			log.Trace ("()");
		}

		public NodeXbee(UInt64 p_address64, UInt16 p_address16)
		{
			log.Trace ("(64=0x{0:X} 16=0x{1:X})", p_address64, p_address16);

			this.Address16 = p_address16;
			this.Address64 = p_address64;
		}

	}
}

