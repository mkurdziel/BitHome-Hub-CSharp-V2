using System;
using BitHome.Messaging;
using BitHome.Messaging.Messages;
using NLog;
using System.Text;

namespace BitHome.Messaging
{
	public static class MessageFactory
	{
		private static Logger log = LogManager.GetCurrentClassLogger();

		public static MessageBase CreateMessage(Node p_sourceNode, Node p_destNode, byte[] p_data, int p_startIndex)
		{
			
			// Make sure the first byte is our start byte
			if (p_data[p_startIndex] != (byte)Protocol.PacketValues.PACKET_START)
			{
				log.Warn("Received message without start byte. Read: 0x{0:X}",p_data[p_startIndex]);
				return null;
			}

			// Switch on the API byte
			switch(p_data[p_startIndex+(int)Protocol.Indexes.API])
			{
			case (byte)Protocol.Api.DEVICE_STATUS_REQUEST:
				{
					log.Trace ("Device Status Request");
				}
				break;
			case (byte)Protocol.Api.DEVICE_STATUS_RESPONSE:
				{
					log.Trace ("Device Status Response");
                    return new MessageDeviceStatusResponse(p_sourceNode, p_destNode, p_data, p_startIndex);
				}
			case (byte)Protocol.Api.DEVICE_INFO_REQUEST:
				{
					log.Trace ("Device Info Request");
				}
				break;
			case (byte)Protocol.Api.DEVICE_INFO_RESPONSE:
				{
					log.Trace ("Device Info Response");
					return new MessageDeviceInfoResponse(p_sourceNode, p_destNode, p_data, p_startIndex);
				}
			case (byte)Protocol.Api.BOOTLOAD_TRANSMIT:
				{
					log.Trace ("Bootload Transmit");
				}
				break;	
			case (byte)Protocol.Api.BOOTLOAD_RESPONSE:
				{
					log.Trace ("Bootload Response");
                    return new MessageBootloadResponse(p_sourceNode, p_destNode, p_data, p_startIndex);
				}
				break;
			case (byte)Protocol.Api.CATALOG_REQUEST:
				{
					log.Trace ("Catalog Request");
				}
				break;
			case (byte)Protocol.Api.CATALOG_RESPONSE:
				{
					log.Trace ("Catalog Response");
                    return new MessageCatalogResponse(p_sourceNode, p_destNode, p_data, p_startIndex);
				}
			case (byte)Protocol.Api.PARAMETER_REQUEST:
				{
					log.Trace ("Parameter Request");
				}
				break;
			case (byte)Protocol.Api.PARAMETER_RESPONSE:
				{
					log.Trace ("Parameter Response");
                    return new MessageParameterResponse(p_sourceNode, p_destNode, p_data, p_startIndex);
				}
			case (byte)Protocol.Api.FUNCTION_RECEIVE:
				{
	//				return new MsgFunctionReceive(p_sourceNode, p_destinationNode, p_data, p_startIndex);
					log.Trace ("Function Receive");
				}
				break;
			case (byte)Protocol.Api.FUNCTION_RECEIVE_RESPONSE:
				{
					log.Trace ("Function Receive Response");
				}
				break;
			case (byte)Protocol.Api.FUNCTION_TRANSMIT:
				{
					log.Trace ("Function Transmit");
				}
				break;
			case (byte)Protocol.Api.FUNCTION_TRANSMIT_RESPONSE:
				{
					log.Trace ("Function Transmit Response");
				}
				break;
			case (byte)Protocol.Api.DATA_REQUEST:
				{
					log.Trace ("Data Request");
				}
				break;
			case (byte)Protocol.Api.DATA_RESPONSE:
				{
					log.Trace ("Data Response");
					return new MessageDataResponse (p_sourceNode, p_destNode, p_data, p_startIndex);
				}
				break;
			default:
				{
				log.Warn ("Received function with invalid API: 0x{0:X}", p_data[p_startIndex+(int)Protocol.Indexes.API]);
					System.Text.StringBuilder sb = new StringBuilder ();
					foreach (byte data in p_data) {
						sb.Append(String.Format("{0:X2}", data));
					}
					log.Debug("Message Data: {0}", sb.ToString()); 
				}
				break;
			}
			return null;

		}
	}
}

