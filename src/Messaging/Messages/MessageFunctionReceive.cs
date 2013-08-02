using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Messaging.Messages
{
    public class MessageFunctionRecieve : MessageBase
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly byte m_nFunctionID;
        private readonly DataType m_nDataType;
        private readonly int m_nValue;
        private readonly string m_strValue;
        //private readonly bool m_bValue;
        //private readonly bool m_bIsInteger;
        //private readonly string m_strTypeName;

		public override Api Api {
			get {
				return Protocol.Api.FUNCTION_RECEIVE;
			}
		}


        public int FunctionId
        {
            get { return m_nFunctionID; }
        }

        public DataType DataType
        {
            get { return m_nDataType; }
        }



        public MessageFunctionRecieve (
            Node p_sourceNode,
            Node p_destinationNode,
            byte[] p_data,
            int p_dataOffset) :
            base(p_sourceNode, p_destinationNode)
        {
            int nPayloadIdx = p_dataOffset + 5;
            m_nFunctionID = p_data[nPayloadIdx++];
            m_nDataType = (DataType)p_data[nPayloadIdx++];
            m_nValue = 0;
            m_strValue = string.Empty;
            //m_bIsInteger = false;

            int nConversionLengthInBytes;

            switch (m_nDataType)
            {
                case DataType.BOOL:
                    //m_bValue = (p_data[nPayloadIdx] == 0) ? false : true;
                    //m_strTypeName = "BOOL";
                    break;
                case DataType.BYTE:
                    nConversionLengthInBytes = 1;
                    DataHelpers.LoadValueGivenWidth(p_data, nPayloadIdx, nConversionLengthInBytes, out m_nValue);
                    //m_bIsInteger = true;
                    //m_strTypeName = "BYTE";
                    break;
                case DataType.STRING:
                    DataHelpers.GatherZeroTermString(p_data, nPayloadIdx, out m_strValue);
                    //m_strTypeName = "STRING";
                    break;
                case DataType.DWORD:
                    nConversionLengthInBytes = 3;
                    DataHelpers.LoadValueGivenWidth(p_data, nPayloadIdx, nConversionLengthInBytes, out m_nValue);
                    //m_bIsInteger = true;
                    //m_strTypeName = "DWORD";
                    break;
                case DataType.VOID:
                    //m_strTypeName = "VOID";
                    break;
                case DataType.WORD:
                    nConversionLengthInBytes = 2;
                    DataHelpers.LoadValueGivenWidth(p_data, nPayloadIdx, nConversionLengthInBytes, out m_nValue);
                    //m_bIsInteger = true;
                    //m_strTypeName = "WORD";
                    break;
                default:
                    log.Warn("Unexpected DATA_TYPE value parsed from SyNetFunctionReceive message");
                    //m_strTypeName = "{unknown}";
                    break;
            }
        }
    }
}
