using System;
using NLog;
using BitHome.Messaging.Messages;


namespace BitHome.Messaging.Xbee
{
	public static class MessageFactoryXbee
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		private static NodeService m_nodeService = null;

		private static NodeService NodeService {
			get {
				if (m_nodeService == null) 
				{
					m_nodeService = ServiceManager.NodeService;
				}

				return m_nodeService;
			}
		}
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
					// Extract the 64 bit address
					UInt64 address64 = EBitConverter.ToUInt64(p_data, (int)Protocol.Rx.ADDR64_OFFSET);
					// Extract the 16 bit address
					UInt16 address16 = EBitConverter.ToUInt16(p_data, (int)Protocol.Rx.ADDR16_OFFSET);
	
					log.Trace ("Received Zigbee RX message 64:0x{0:X} 16:0x{1:X}", address64, address16);
	
					// Look up the node
					Node node = NodeService.GetNodeXbee (address64, address16, true);

					return MessageFactory.CreateMessage(p_data, (int)Protocol.Rx.DATA_OFFSET);
				} 

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

