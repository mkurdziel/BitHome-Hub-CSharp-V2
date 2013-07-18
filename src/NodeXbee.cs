using System;

namespace BitHome
{
	public class NodeXbee : Node
	{
		UInt64 m_address64;
		UInt16 m_address16;

		public NodeXbee () {
		}

		public NodeXbee (UInt64 p_address64, UInt16 p_address16)
		{
			m_address64 = p_address64;
			m_address16 = p_address16;
		}
	}
}

