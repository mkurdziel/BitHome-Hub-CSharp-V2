using System;
using System.Collections.Generic;
using System.Linq;
using BitHome.Actions;
using BitHomeProtocol = BitHome.Messaging.Protocol;

namespace BitHome.Messaging.Messages
{
	public class MessageFunctionTransmit : MessageTxBase
	{
    //private byte[] m_bytes;
    //private Object m_responseLockObject = new Object();
    //private boolean m_isResponded = false;
    //private MsgFunctionReceive m_response = null

        public int ActionIndex { get; private set; }
        public INodeParameter[] Parameters { get; private set; }
        public BitHomeProtocol.DataType ReturnType { get; private set; }

		public override BitHomeProtocol.Api Api {
			get {
				return BitHomeProtocol.Api.FUNCTION_TRANSMIT;
			}
		}


        public MessageFunctionTransmit(
            int actionIndex,
            INodeParameter[] parameters,
            BitHomeProtocol.DataType returnType )
        {
            this.ActionIndex = actionIndex;
            this.Parameters = parameters;
            this.ReturnType = returnType;
        }

		public override byte[] GetBytes ()
		{
		    List<byte> bytes = new List<byte>();

            bytes.Add((byte)BitHomeProtocol.PacketValues.PACKET_START);
            bytes.Add((byte)BitHomeProtocol.Api.FUNCTION_TRANSMIT);
		    bytes.Add((byte) ActionIndex);
		    bytes.Add((byte) 0); // TODO: Options

            byte[] intBytes;
		    foreach(INodeParameter parameter in Parameters)
            {
                switch(parameter.DataType) {
                    case BitHomeProtocol.DataType.BOOL:
                    case BitHomeProtocol.DataType.UINT8:
                    case BitHomeProtocol.DataType.INT8:
                        intBytes = EBitConverter.GetBytes(parameter.IntValue);
                        bytes.Add(intBytes[7]);
                        break;
                    case BitHomeProtocol.DataType.INT16:
                    case BitHomeProtocol.DataType.UINT16:
                        intBytes = EBitConverter.GetBytes(parameter.IntValue);
                        bytes.Add(intBytes[6]);
                        bytes.Add(intBytes[7]);
                        break;
                    case BitHomeProtocol.DataType.INT32:
                    case BitHomeProtocol.DataType.UINT32:
                        intBytes = EBitConverter.GetBytes(parameter.IntValue);
                        bytes.Add(intBytes[4]);
                        bytes.Add(intBytes[5]);
                        bytes.Add(intBytes[6]);
                        bytes.Add(intBytes[7]);
                        break;
                    case BitHomeProtocol.DataType.INT64:
                    case BitHomeProtocol.DataType.UINT64:
                        intBytes = EBitConverter.GetBytes(parameter.IntValue);
                        bytes.Add(intBytes[0]);
                        bytes.Add(intBytes[1]);
                        bytes.Add(intBytes[2]);
                        bytes.Add(intBytes[3]);
                        bytes.Add(intBytes[4]);
                        bytes.Add(intBytes[5]);
                        bytes.Add(intBytes[6]);
                        bytes.Add(intBytes[7]);
                        break;
                    case BitHomeProtocol.DataType.STRING:
                        bytes.AddRange(parameter.Value.Select(c => (byte) c));
                        bytes.Add(0x00);
                        break;
                }
            }

		    return bytes.ToArray();
		}
	}
}



///**
// * 
// */
//package synet.controller.messaging.messages;

//import synet.controller.Protocol.EsnDataTypes;
//import synet.controller.actions.INodeParameter;
//import synet.controller.nodes.NodeBase;
//import synet.controller.utils.EBitConverter;

///**
// * @author mkurdziel
// *
// */
//public class MsgFunctionTransmit extends MsgTx
//{
//    private static final String TAG = "MsgFunctionTransmit";
//    private byte[] m_bytes;
//    private boolean m_needsReturn;
//    private int m_actionIndex;
//    private Object m_responseLockObject = new Object();
//    private boolean m_isResponded = false;
//    private MsgFunctionReceive m_response = null;

//    public MsgFunctionTransmit(
//            NodeBase p_destinationNode,
//            int p_functionId,
//            int p_numParameters,
//            INodeParameter[] p_parameters,
//            EsnDataTypes p_returnType)
//    {
//        super(p_destinationNode);

//        // Save this for use in the dispatcher
//        m_actionIndex = p_functionId;

//        // Save the return type for use in the dispatcher
//        m_needsReturn = p_returnType != EsnDataTypes.VOID;

//        // Find the size of the parameters
//        int paramBytes = 0;
//        INodeParameter param;

//        for (int i=0; i<p_numParameters; ++i)
//        {
//            param = p_parameters[i];
//            switch(param.getDataType())
//            {
//                case BOOL:
//                case BYTE:
//                    paramBytes++;
//                    break;
//                case WORD:
//                    paramBytes+=2;
//                    break;
//                case DWORD:
//                    paramBytes+=4;
//                    break;
//                case QWORD:
//                    paramBytes+=8;
//                    break;
//                case STRING:
//                    paramBytes += param.getStrValue().length()+1;
//                    break;
//                case VOID:
//                    // No param for void?
//                    break;
//            }
//        }

//        m_bytes = new byte[3 + paramBytes];

//        m_bytes[0] = (byte)MsgConstants.SYNET_START_BYTE;
//        m_bytes[1] = (byte)MsgConstants.SN_API_FUNCTION_TRANSMIT;
//        m_bytes[2] = (byte)p_functionId;

//        int byteIndex = 3;
//        long intVal;
//        byte[] bytes;

//        for (int i=0; i<p_numParameters; ++i)
//        {
//            param = p_parameters[i];
//            switch(param.getDataType())
//            {
//                case BOOL:
//                case BYTE:
//                    intVal = param.getIntValue();
//                    bytes = EBitConverter.longToBytes(intVal);
//                    m_bytes[byteIndex++] = bytes[7];
//                    break;
//                case WORD:
//                    intVal = param.getIntValue();
//                    bytes = EBitConverter.longToBytes(intVal);
//                    m_bytes[byteIndex++] = bytes[6];
//                    m_bytes[byteIndex++] = bytes[7];
//                    break;
//                case DWORD:
//                    intVal = param.getIntValue();
//                    bytes = EBitConverter.longToBytes(intVal);
//                    m_bytes[byteIndex++] = bytes[4];
//                    m_bytes[byteIndex++] = bytes[5];
//                    m_bytes[byteIndex++] = bytes[6];
//                    m_bytes[byteIndex++] = bytes[7];
//                    break;
//                case QWORD:
//                    intVal = param.getIntValue();
//                    bytes = EBitConverter.longToBytes(intVal);
//                    m_bytes[byteIndex++] = bytes[0];
//                    m_bytes[byteIndex++] = bytes[1];
//                    m_bytes[byteIndex++] = bytes[2];
//                    m_bytes[byteIndex++] = bytes[3];
//                    m_bytes[byteIndex++] = bytes[4];
//                    m_bytes[byteIndex++] = bytes[5];
//                    m_bytes[byteIndex++] = bytes[6];
//                    m_bytes[byteIndex++] = bytes[7];
//                    break;
//                case STRING:
//                    for( char c : param.getStrValue().toCharArray())
//                    {
//                        m_bytes[byteIndex++] = (byte)c;

//                    }
//                    m_bytes[byteIndex++] = 0x00;
//                    break;
//                case VOID:
//                    // No param for void?
//                    break;
//            }
//        }
//    }


//    /**
//     * Set the response status of the message to be true
//     * 
//     * @param p_response the response message
//     */
//    public void setIsResponded(MsgFunctionReceive p_response)
//    {
//        synchronized (m_responseLockObject)
//        {
//            m_isResponded = true;
//            m_response = p_response;
//            m_responseLockObject.notifyAll();
//        }
//    }

//    /**
//     * @return true if there is a response to this message
//     */
//    public boolean getIsResponded()
//    {
//        return m_isResponded;
//    }


//    /**
//     * Wait for the message to send
//     * @param p_milliseconds
//     * @return
//     */
//    public boolean waitForResponse(long p_milliseconds)
//    {
//        // If already sent, return true as a success
//        if (m_isResponded) return true;

//        synchronized (m_responseLockObject)
//        {
//            try {
//                m_responseLockObject.wait(p_milliseconds);
//            } catch (InterruptedException e) {
//                // We didn't get a send notification within the 
//                // requested timeout
//                return false;
//            }
//        }
//        return m_isResponded;
//    }

//    /**
//     * @return the response message
//     */
//    public MsgFunctionReceive getResponseMsg()
//    {
//        return m_response;
//    }

//    /* (non-Javadoc)
//     * @see synet.controller.messaging.messages.Msg#getMsgType()
//     */
//    @Override
//    public String getMsgType()
//    {
//        return TAG;
//    }

//    /* (non-Javadoc)
//     * @see synet.controller.messaging.messages.Msg#getAPI()
//     */
//    @Override
//    public byte getAPI()
//    {
//        return MsgConstants.SN_API_FUNCTION_TRANSMIT;
//    }

//    @Override
//    public byte[] getBytes()
//    {
//        return m_bytes;
//    }

//    /**
//     * @return true if this transmit has a return type other than void
//     */
//    public boolean getNeedsReturn()
//    {
//        return m_needsReturn;
//    }

//    /**
//     * @return the action index of this transmit request
//     */
//    public int getActionIndex()
//    {
//        return m_actionIndex;
//    }
//}
