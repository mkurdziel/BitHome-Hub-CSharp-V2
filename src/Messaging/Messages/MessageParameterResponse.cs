using System;
using System.Collections.Generic;
using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Messaging.Messages
{
    public class MessageParameterResponse : MessageBase
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly int m_functionId;
        private readonly int m_paramId;
        private readonly String m_paramName;
        private readonly DataType m_paramDataType;
        private readonly ParamValidationType m_validationType;
        private readonly int m_nMaxStringLength;
        private readonly int m_nValueWidthInBytes;
        private readonly bool m_bIsSigned;
		private readonly Dictionary<String, int> m_enumValueByName = new Dictionary<String, int> ();
        private long m_maxValue;
        private long m_minValue;

        public override Api Api {
			get {
				return Protocol.Api.PARAMETER_RESPONSE;
			}
		}

        public int FunctionId
        {
            get { return m_functionId; }
        }

        public int ParamId
        {
            get { return m_paramId; }
        }

        public String ParamName
        {
            get { return m_paramName; }
        }

		public Int64 MaxValue
		{
		    get { return m_maxValue; }
		    private set { m_maxValue = value; }
		}

        public Int64 MinValue
        {
            get { return m_minValue; }
            private set { m_minValue = value; }
        }

        public DataType DataType
        {
            get { return m_paramDataType; }
        }

        public ParamValidationType ValidationType
        {
            get { return m_validationType; }
        }

        public int MaxStringLength
        {
            get { return m_nMaxStringLength; }
        }

        public bool IsSigned
        {
            get { return m_bIsSigned; }
        }

        public Dictionary<String, int> EnumValues
        {
            get { return m_enumValueByName; }
        }

        public MessageParameterResponse (
			Node p_sourceNode,
			int actionIndex,
			int paramIndex,
			String name,
			DataType dataType,
			ParamValidationType validationType,
			Int64 minValue,
			Int64 maxValue,
			Dictionary<String, int> enumValues ):
			base(p_sourceNode, null)
		{
			m_functionId = actionIndex;
			m_paramId = paramIndex;
			m_paramName = name;
			m_paramDataType = dataType;
			m_validationType = validationType;
			this.MinValue = minValue;
			this.MaxValue = maxValue;
			m_enumValueByName = enumValues;
		}

        public MessageParameterResponse (
            Node p_sourceNode,
            Node p_destinationNode,
            byte[] p_data,
            int p_dataOffset) :
            base(p_sourceNode, p_destinationNode)
        {

            int nByteIdx = p_dataOffset + 2;

            // get function ID
            m_functionId = p_data[nByteIdx++];
            // get parameter ID
            m_paramId = p_data[nByteIdx++];
            // get parameter data type
            m_paramDataType = (DataType)p_data[nByteIdx++];
            switch (m_paramDataType)
            {
                case DataType.BOOL:
                    m_nValueWidthInBytes = 1;
                    break;
                case DataType.BYTE:
                    m_nValueWidthInBytes = 1;
                    break;
                case DataType.WORD:
                    m_nValueWidthInBytes = 2;
                    break;
                case DataType.VOID:
                    m_nValueWidthInBytes = 0;
                    break;
                case DataType.STRING:
                    m_nValueWidthInBytes = 0;
                    break;
                case DataType.DWORD:
                    m_nValueWidthInBytes = 4;
                    break;
                default:
                    log.Warn("Unrecognized Parameter Data Type");
                    return;
            }

            // get parameter validation type
			byte validationByte = p_data [nByteIdx++];
			if (validationByte == 0) { // Unsigned full
				// TODO: min and max values
				m_validationType = ParamValidationType.UNSIGNED_RANGE;
			} else if (validationByte == 10) {
				m_validationType = ParamValidationType.SIGNED_RANGE;
			} else {
				m_validationType = (ParamValidationType)validationByte;
			}

            // get validation values
            switch (m_validationType)
            {
                case ParamValidationType.UNSIGNED_RANGE:
                    // load min and max type-width values
                    nByteIdx += DataHelpers.LoadValueGivenWidth(p_data, nByteIdx, m_nValueWidthInBytes, out m_minValue);
                    nByteIdx += DataHelpers.LoadValueGivenWidth(p_data, nByteIdx, m_nValueWidthInBytes, out m_maxValue);
                    m_bIsSigned = false;
                    break;
                case ParamValidationType.SIGNED_RANGE:
                    m_bIsSigned = true;
                    // load min and max type-width values
                    nByteIdx += DataHelpers.LoadValueGivenWidth(p_data, nByteIdx, m_nValueWidthInBytes, out m_minValue);
                    nByteIdx += DataHelpers.LoadValueGivenWidth(p_data, nByteIdx, m_nValueWidthInBytes, out m_maxValue);
                    break;
                case ParamValidationType.ENUMERATED:
                    // load count, then value-name pairs count times
                    int nNbrEnumValues = p_data[nByteIdx++];
                    int nEnumValue;
                    string strEnumValueName;
                    for (int nEntryIdx = 0; nEntryIdx < nNbrEnumValues; nEntryIdx++)
                    {
                        nByteIdx += DataHelpers.LoadValueGivenWidth(p_data, nByteIdx, m_nValueWidthInBytes, out nEnumValue);
                        nByteIdx += DataHelpers.GatherZeroTermString(p_data, nByteIdx, out strEnumValueName);
                        m_enumValueByName[strEnumValueName] = nEnumValue;
                    }
                    break;
                case ParamValidationType.STRING:
                    // load single byte max string length
                    m_nMaxStringLength = p_data[nByteIdx++];
                    break;
            }

            // get parameter name
            nByteIdx += DataHelpers.GatherZeroTermString(p_data, nByteIdx, out m_paramName);
            //}
        }
    }
}
