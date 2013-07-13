using System;
using NLog;
using BitHome.Messaging.Messages;


namespace BitHome.Messaging.Xbee
{
	public static class MessageFactoryXbee
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

//
//		private NodeManager m_nodeManager;
//
//		/**
//	 * Default constructor
//	 */
//		public MsgFactoryXbee()
//		{
//			m_nodeManager = NodeManager.getInstance();
//		}
//
		public static MessageBase createMessage(byte p_api, byte[] p_data)
		{
			switch(p_api)
			{
			case (byte)Protocol.Api.ZIGBEE_RX:
				{
					log.Debug ("Creating {0}", Protocol.Api.ZIGBEE_RX);
	//					// Extract the 64 bit address
	//					long address64 = EBitConverter.toUInt64(p_data, C_XBEE_RX_ADDR64_OFFSET);
	//					// Extract the 16 bit address
	//					int address16 = EBitConverter.toUInt16(p_data, C_XBEE_RX_ADDR16_OFFSET);
	//
	//					Logger.v(TAG, "received Zigbee RX message 64:"+ String.format("0x%x", address64)
	//					         + " 16:" + String.format("0x%x", address16));
	//
	//					// Look up the node
	//					NodeBase node = m_nodeManager.getNode(address64);
	//
	//					if (node == null)
	//					{
	//						node = new NodeZigbee(address64, address16);
	//						m_nodeManager.addNewNode(address64, node);
	//					}
	//
	//					return MsgFactory.createMessage(node, null, p_data, C_XBEE_RX_DATA_OFFSET);
				} 
				break;

			case (byte)Protocol.Api.ZIGBEE_TX_STATUS:
				{
						//			Logger.v(TAG, "TXStatus - Frame:"+p_data[C_XBEE_TXS_FRAME_OFFSET]+" Addr16:"+String.format("0x%x", EBitConverter.toUInt16(p_data, C_XBEE_TXS_ADDR16_OFFSET)) +
						//					" retry:" + p_data[C_XBEE_TXS_RETRY_OFFSET] +
						//					" status:" + String.format("0x%x", p_data[C_XBEE_TXS_STATUS_OFFSET])+
						//					" discovery:" + String.format("0x%x", p_data[C_XBEE_TXS_DISCOVERY_OFFSET]));
	//					return new MsgXbeeTxStatus(
	//						p_data[C_XBEE_TXS_FRAME_OFFSET], 
	//						p_data[C_XBEE_TXS_RETRY_OFFSET], 
	//						p_data[C_XBEE_TXS_STATUS_OFFSET], 
	//						p_data[C_XBEE_TXS_DISCOVERY_OFFSET]);
				}
				break;

			default:
				{
					log.Debug ("Received unknown message api: {0:2X}", p_api);
				}
				break;
			}
			return null;
		}

	}
}

