using System;

namespace BitHome
{
	public class NodeUpdateStatus
	{
		private NodeUpdateFile m_dataFile;
		private Node m_node;
		private int m_nextAddress = 0;
		private NodeBootloadStatus m_statusEnum = NodeBootloadStatus.UNKNOWN;
		private DateTime m_timeNextInvestigation;
		private int m_numInvestigationRetries = 0;

		public NodeUpdateStatus(Node p_node, NodeUpdateFile p_dataFile)
		{
			m_dataFile = p_dataFile;
			m_node = p_node;
		}

	
		public void setNumRetries(int p_retries)
		{
			m_numInvestigationRetries = p_retries;
		}

		public int getNumRetries()
		{
			return m_numInvestigationRetries;
		}

		public void setTimeNextUpdate(DateTime p_time)
		{
			m_timeNextInvestigation = p_time;
		}

		public DateTime getTimeNextUpdate()
		{
			return m_timeNextInvestigation;
		}

		public NodeBootloadStatus getStatus()
		{
			return m_statusEnum;
		}

		public void setStatus(NodeBootloadStatus p_status)
		{
			m_statusEnum = p_status;
		}

		public Node getNode()
		{
			return m_node;
		}

		public int getNextAddress()
		{
			return m_nextAddress;
		}

		public void markAsSent(int p_memoryAddress)
		{
//			m_nextAddress = p_memoryAddress + m_node.getCodeUpdatePageSize(); 
		}

		public bool IsComplete()
		{
//			return m_nextAddress > m_dataFile.getMaxAddress();
			return true;
		}

		public byte getDataByte(int p_address)
		{
//				return m_dataFile.getDataByte(p_address); 
			return 0;
		}
	}
}

