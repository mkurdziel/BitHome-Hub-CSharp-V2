using System;

namespace BitHome.Messaging.Protocol
{
	/// </summary>
	public enum PacketValues : byte
	{
		/// <summary>
		///   SyNet packet start byte
		/// </summary>
		PACKET_START = 0xA5
	}

	/// <summary>
	///   Enumeration for parameter validation types
	/// </summary>
	public enum ParamValidationTypes
	{
		/// <summary>
		///   Unsigned full range
		/// </summary>
		UNSIGNED_FULL = 0,
		/// <summary>
		///   Unsigned range
		/// </summary>
		UNSIGNED_RANGE = 1,
		/// <summary>
		///   Enumerated values
		/// </summary>
		ENUMERATED = 2,
		/// <summary>
		///   String with maximum length
		/// </summary>
		MAX_STRING_LEN = 3,
		/// <summary>
		///   Boolean
		/// </summary>
		BOOL = 4,
		/// <summary>
		///   Signed full range
		/// </summary>
		SIGNED_FULL = 10,
		/// <summary>
		///   Signed range
		/// </summary>
		SIGNED_RANGE = 11,
		/// <summary>
		///   DateTime value (always underlying QWORD Type!)
		/// </summary>
		DATE_TIME = 20,
		/// <summary>
		///   Unknown
		/// </summary>
		UNKNOWN
	}

	/// <summary>
	///   Enumeration for the SyNet API
	/// </summary>
	public enum Api
	{
		/// <summary>
		///   Device status request
		/// </summary>
		DEVICE_STATUS_REQUEST = 0x02,
		/// <summary>
		///   Device status response
		/// </summary>
		DEVICE_STATUS = 0x03,
		/// <summary>
		///   Bootload transmit
		/// </summary>
		BOOTLOAD_TRANSMIT = 0x04,
		/// <summary>
		///   Bootload receive
		/// </summary>
		BOOTLOAD_RESPONSE = 0x05,
		/// <summary>
		///   Set device info
		/// </summary>
		SETINFO = 0x0A,
		/// <summary>
		///   Set device info response
		/// </summary>
		SETINFORESPONSE = 0x0B,
		/// <summary>
		///   Catalog request
		/// </summary>
		CATALOG_REQUEST = 0x10,
		/// <summary>
		///   Catalog response
		/// </summary>
		CATALOG_RESPONSE = 0x11,
		/// <summary>
		///   Parameter request
		/// </summary>
		PARAMETER_REQUEST = 0x12,
		/// <summary>
		///   Parameter response
		/// </summary>
		PARAMETER_RESPONSE = 0x13,
		/// <summary>
		///   Function transmit
		/// </summary>
		FUNCTION_TRANSMIT = 0x40,
		/// <summary>
		///   Function transmit response
		/// </summary>
		FUNCTION_TRANSMIT_RESPONSE = 0x80,
		/// <summary>
		///   Function receive
		/// </summary>
		FUNCTION_RECEIVE = 0x60,
		/// <summary>
		///   Function receive response
		/// </summary>
		FUNCTION_RECEIVE_RESPONSE = 0x61
	}

	/// <summary>
	///   Device status values
	/// </summary>
	public enum DeviceStatusValues
	{
		/// <summary>
		///   Active
		/// </summary>
		ACTIVE = 0x00,
		/// <summary>
		///   Hardware Reset
		/// </summary>
		HW_RESET = 0x01,
		/// <summary>
		///   Info
		/// </summary>
		INFO = 0x02
	}

	/// <summary>
	///   Enumeration for device status request
	/// </summary>
	public enum DeviceStatusRequest
	{
		/// <summary>
		///   Status request
		/// </summary>
		STATUS_REQUEST = 0x00,
		/// <summary>
		///   Information request
		/// </summary>
		INFO_REQUEST = 0x01
	}

	/// <summary>
	///   Enumeration for SyNet bootload transmit API
	/// </summary>
	public enum BootloadTransmit
	{
		/// <summary>
		///   Reboot the device
		/// </summary>
		REBOOT_DEVICE = 0x00,
		/// <summary>
		///   Request a bootload
		/// </summary>
		BOOTLOAD_REQUEST = 0x01,
		/// <summary>
		///   Data transmit
		/// </summary>
		DATA_TRANSMIT = 0x03,
		/// <summary>
		///   Data complete
		/// </summary>
		DATA_COMPLETE = 0x04
	}

	/// <summary>
	///   Enumeration for bootload response
	/// </summary>
	public enum BootloadResponse
	{
		/// <summary>
		///   Device is ready to bootload
		/// </summary>
		BOOTLOAD_READY = 0x00,
		/// <summary>
		///   Data chunk was successful
		/// </summary>
		DATA_SUCCESS = 0x01,
		/// <summary>
		///   Bootload completed
		/// </summary>
		BOOTLOAD_COMPLETE = 0x02,
	}

	/// <summary>
	///   Enumeration for bootload error values
	/// </summary>
	public enum BootloadError
	{
		/// <summary>
		///   Start bit
		/// </summary>
		START_BIT = 0x01,
		/// <summary>
		///   Size
		/// </summary>
		SIZE = 0x02,
		/// <summary>
		///   Api
		/// </summary>
		API = 0x03,
		/// <summary>
		///   Network address
		/// </summary>
		MY16_ADDR = 0x04,
		/// <summary>
		///   Bootload API
		/// </summary>
		BOOTLOADAPI = 0x05,
		/// <summary>
		///   Bootload start
		/// </summary>
		BOOTLOADSTART = 0x06,
		/// <summary>
		///   Page Length
		/// </summary>
		PAGELENGTH = 0x07,
		/// <summary>
		///   Page address
		/// </summary>
		ADDRESS = 0x08,
		/// <summary>
		///   Checksum
		/// </summary>
		CHECKSUM = 0x09,
		/// <summary>
		///   SyNet API
		/// </summary>
		SYNETAPI = 0x0A
	}

	/// <summary>
	///   Enumeration for data types
	/// </summary>
	public enum DataTypes
	{
		/// <summary>
		///   Void
		/// </summary>
		VOID = 0x00,
		/// <summary>
		///   Byte - 8 bits
		/// </summary>
		BYTE = 0x01,
		/// <summary>
		///   Word - 16 bits
		/// </summary>
		WORD = 0x02,
		/// <summary>
		///   String (len bytes plus len value)
		/// </summary>
		STRING = 0x03,
		/// <summary>
		///   Doubleword - 32 bits
		/// </summary>
		DWORD = 0x04,
		/// <summary>
		///   Bool - 8 bits
		/// </summary>
		BOOL = 0x05,
		/// <summary>
		///   QWORD - Quad-word 64-bits
		/// </summary>
		QWORD = 0x06
	}

	/// <summary>
	///   Enumeration for SyNet Device Info Values
	/// </summary>
	public enum InfoValues
	{
		/// <summary>
		///   Device ID
		/// </summary>
		ID = 0x01,
		/// <summary>
		///   Manufacturer ID
		/// </summary>
		MANUFAC = 0x02,
		/// <summary>
		///   Profile ID
		/// </summary>
		PROFILE = 0x03,
		/// <summary>
		///   Revision ID
		/// </summary>
		REVISION = 0x04,
		/// <summary>
		///   TODO: Temporary, remove
		/// </summary>
		REMOTE = 0x0F
	}
}

