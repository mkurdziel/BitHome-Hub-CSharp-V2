using System;

namespace BitHome.Messaging.Xbee.Protocol
{
	public enum Api : byte 
	{
		START = 0x7E,
		INVALID = 0x00,
		MODEM_STATUS = 0xBA,
		AT_CMD = 0x08,
		AT_CMD_PARAM = 0x09,
		AT_CMD_RESPONSE = 0x88,
		REMOTE_CMD = 0x17,
		CMD_RESPONSE = 0x97,
		ZIGBEE_TX_REQ = 0x10,
		ZIGBEE_CMD_FRAME = 0x11,
		ZIGBEE_TX_STATUS = 0x8B,
		ZIGBEE_RX = 0x90,
		ZIGBEE_IO = 0x92,
		RX_SENSOR = 0x94,
		NODE_IDENT = 0x95
	}

	public enum Rx
	{
		DATA_OFFSET = 11,
		ADDR64_OFFSET = 0,
		ADDR16_OFFSET = 8,
	}

	
	public enum Tx
	{
		FRAME_OFFSET = 0,
		ADDR16_OFFSET = 1,
		RETRY_OFFSET = 3,
		STATUS_OFFSET = 4,
		DISCOVERY_OFFSET = 5,
	}
}

