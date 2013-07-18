﻿using System;
using System.Collections.Generic;
using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Messaging.Messages
{
    internal class MessageParameterResponse : MessageBase
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly int m_functionId;
        private readonly int m_paramId;
        private readonly String m_paramName;
        private readonly DataType m_paramDataType;
        private readonly ParamValidationType m_validationType;
        private readonly int m_nMaxStringLength;
        private readonly int m_nMinumumValue;
        private readonly int m_nMaximumValue;
        private readonly int m_nValueWidthInBytes;
        private readonly bool m_bIsSigned;
        private readonly Dictionary<string, int> m_enumValueByName = new Dictionary<string, int>();

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

        public Dictionary<string, int> EnumValues
        {
            get { return m_enumValueByName; }
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
            m_validationType = (ParamValidationType)p_data[nByteIdx++];

            // get validation values
            switch (m_validationType)
            {
                case ParamValidationType.UNSIGNED_FULL:
                    // full-range unsigned value
                    m_bIsSigned = false;
                    break;
                case ParamValidationType.UNSIGNED_RANGE:
                    // load min and max type-width values
                    nByteIdx += DataHelpers.LoadValueGivenWidth(p_data, nByteIdx, m_nValueWidthInBytes, out m_nMinumumValue);
                    nByteIdx += DataHelpers.LoadValueGivenWidth(p_data, nByteIdx, m_nValueWidthInBytes, out m_nMaximumValue);
                    m_bIsSigned = false;
                    break;
                case ParamValidationType.SIGNED_FULL:
                    // full-range signed value
                    m_bIsSigned = true;
                    break;
                case ParamValidationType.SIGNED_RANGE:
                    m_bIsSigned = true;
                    // load min and max type-width values
                    nByteIdx += DataHelpers.LoadValueGivenWidth(p_data, nByteIdx, m_nValueWidthInBytes, out m_nMinumumValue);
                    nByteIdx += DataHelpers.LoadValueGivenWidth(p_data, nByteIdx, m_nValueWidthInBytes, out m_nMaximumValue);
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
                case ParamValidationType.MAX_STRING_LEN:
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
