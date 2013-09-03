using System;
using System.Collections.Generic;
using BitHome.Helpers;
using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Messaging.Messages
{
    public class MessageParameterResponse : MessageBase
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private readonly Dictionary<String, int> m_enumValueByName = new Dictionary<String, int> ();
		private int m_nValueWidthInBytes;
		private String m_name;
		private Int64 m_maxValue;
		private Int64 m_minValue;

        public override Api Api {
			get {
				return Protocol.Api.PARAMETER_RESPONSE;
			}
		}

		public String Name {
			get {
				return m_name;
			}
		}

		public int ActionIndex { get; private set; }
		public int ParameterIndex { get; private set; }
		public DataType DataType { get; private set; }
		public byte Options { get; private set; }

		public Int64 MaxValue {
			get {
				return m_maxValue;
			}
		}

		public Int64 MinValue {
			get {
				return m_minValue;
			}
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
			Int64 minValue,
			Int64 maxValue,
			Dictionary<String, int> enumValues ):
			base(p_sourceNode, null)
		{
			ActionIndex = actionIndex;
			ParameterIndex = paramIndex;
			m_name = name;
			DataType = dataType;
			m_minValue = minValue;
			m_maxValue = maxValue;
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

			ActionIndex = p_data[nByteIdx++];
			ParameterIndex = p_data[nByteIdx++];
			DataType = (DataType)p_data[nByteIdx++];
			Options = p_data[nByteIdx++];
			
			// get parameter name
			nByteIdx += DataHelpers.GatherZeroTermString(p_data, nByteIdx, out m_name);

			// Get the minium and maximum values
			switch (DataType)
            {
                case DataType.BOOL:
                    m_nValueWidthInBytes = 1;
                    break;
                case DataType.INT8:
                    m_nValueWidthInBytes = 1;
                    break;
				case DataType.INT16:
					m_nValueWidthInBytes = 2;
					break;
				case DataType.INT32:
					m_nValueWidthInBytes = 4;
					break;
				case DataType.INT64:
					m_nValueWidthInBytes = 8;
					break;
				case DataType.UINT8:
					m_nValueWidthInBytes = 1;
					break;
				case DataType.UINT16:
					m_nValueWidthInBytes = 2;
					break;
				case DataType.UINT32:
					m_nValueWidthInBytes = 4;
					break;
				case DataType.UINT64:
					m_nValueWidthInBytes = 8;
					break;
                case DataType.VOID:
                    m_nValueWidthInBytes = 0;
                    break;
                case DataType.STRING:
                    m_nValueWidthInBytes = 1;
                    break;
                default:
					log.Warn("Unrecognized Parameter Data Type: {0}", DataType);
                    return;
            }

			if (m_nValueWidthInBytes > 0) {
				nByteIdx += DataHelpers.LoadValueGivenWidth (p_data, nByteIdx, m_nValueWidthInBytes, out m_minValue);
				nByteIdx += DataHelpers.LoadValueGivenWidth (p_data, nByteIdx, m_nValueWidthInBytes, out m_maxValue);
			}
        }

		public override string ToString ()
		{
			return string.Format ("[MessageParameterResponse: Name={1}, ActionIndex={2}, ParameterIndex={3}, DataType={4}, Options={5}, MaxValue={6}, MinValue={7}]", Api, Name, ActionIndex, ParameterIndex, DataType, Options, MaxValue, MinValue);
		}
    }
}
