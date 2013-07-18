using System.Collections.Generic;
using System.Text;
using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Messaging.Messages
{
    internal class MessageCatalogResponse : MessageBase
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly int m_totalEntries;
        private readonly int m_entryNumber;
        private readonly int m_numParams;
        private readonly DataType m_returnType;
        private readonly Dictionary<int, DataType> m_paramTypes = new Dictionary<int, DataType>();
        private readonly string m_functionName;

        public int TotalEntries
        {
            get { return m_totalEntries; }
        }

        public int EntryNumber
        {
            get { return m_entryNumber; }
        }

        public int NumberParams
        {
            get { return m_numParams; }
        }

        public DataType ReturnType
        {
            get { return m_returnType; }
        }

        public Dictionary<int, DataType> ParamTypes
        {
            get { return m_paramTypes; }
        }

        public string FunctionName 
        {
            get { return m_functionName; }
        }

        public MessageCatalogResponse(
            Node p_sourceNode,
            Node p_destinationNode,
            byte[] p_data,
            int p_dataOffset) :
            base(p_sourceNode, p_destinationNode)
        {

            m_totalEntries = p_data[p_dataOffset + 2];
            m_entryNumber = p_data[p_dataOffset + 3];
            m_numParams = p_data[p_dataOffset + 4];

            log.Trace("entry number: {0}", m_entryNumber);

            if (m_entryNumber != 0)
            {
                m_returnType = (DataType)p_data[p_dataOffset + 5];
                for (int i = 0; i < m_totalEntries; ++i)
                {
                    m_paramTypes[i] = (DataType)p_data[p_dataOffset + 6 + i];
                }

                StringBuilder sbname = new StringBuilder();
                char c;
                int stringIndex = 0;
                while ((c = (char)p_data[p_dataOffset + 6 + m_numParams + (stringIndex++)]) != 0x00)
                {
                    sbname.Append(c);
                    if (stringIndex >= p_data.Length)
                    {
                        log.Warn("Name out of bounds");
                    }
                }
                m_functionName = sbname.ToString();
            }
        }
    }
}
