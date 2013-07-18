using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Messaging.Messages
{
    internal class MessageBootloadResponse : MessageBase
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly BootloadResponse m_bootloadResponse;
        private readonly int m_memoryAddress;

        public BootloadResponse BootloadResponse
        {
            get { return m_bootloadResponse; }
        }

        public int MemoryAddress 
        {
            get { return m_memoryAddress; }
        }

        public MessageBootloadResponse(
            Node p_sourceNode,
            Node p_destinationNode,
            byte[] p_data,
            int p_dataOffset) :
                base(p_sourceNode, p_destinationNode)
        {

            m_bootloadResponse = (BootloadResponse)p_data[p_dataOffset + 2];

            // Memory address if only valid for data success command
            if (m_bootloadResponse == BootloadResponse.DATA_SUCCESS)
            {
                m_memoryAddress = EBitConverter.ToUInt16(p_data, p_dataOffset + 3);
            }

            log.Trace("Status: {0} Mem Address: {1}", m_bootloadResponse, m_memoryAddress);
        }
    }
}
