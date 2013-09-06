using System;

namespace BitHome.Messaging.Protocol
{
	public enum Indexes : int {
		API = 1
	}

	public enum PacketValues : byte
	{
		/// <summary>
		///   SyNet packet start byte
		/// </summary>
		PACKET_START = 0xA5
	}

	/// <summary>
	///   Enumeration for the SyNet API
	/// </summary>
	public enum Api
	{
		DEVICE_STATUS_REQUEST = 0x02,
		DEVICE_STATUS_RESPONSE = 0x03,
		DEVICE_INFO_REQUEST = 0x04,
		DEVICE_INFO_RESPONSE = 0x05,
		CATALOG_REQUEST = 0x10,
		CATALOG_RESPONSE = 0x11,
		PARAMETER_REQUEST = 0x12,
		PARAMETER_RESPONSE = 0x13,
		BOOTLOAD_TRANSMIT = 0x20,
		BOOTLOAD_RESPONSE = 0x21,
		FUNCTION_TRANSMIT = 0x40,
		FUNCTION_TRANSMIT_RESPONSE = 0x80,
		FUNCTION_RECEIVE = 0x60,
		FUNCTION_RECEIVE_RESPONSE = 0x61,
		DATA_REQUEST = 0x50,
		DATA_RESPONSE = 0x51
	}

	/// <summary>
	///   Device status values
	/// </summary>
	public enum DeviceStatusValue
	{
		/// <summary>
		///   Hardware Reset
		/// </summary>
		HW_RESET = 0x00,
		/// <summary>
		///   Active
		/// </summary>
		ACTIVE = 0x01
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
	[Serializable]
	public enum DataType
	{
		VOID	= 0x00,
		UINT8	= 0x01,
		UINT16	= 0x02,
		UINT32	= 0x03,
		UINT64	= 0x04,
		INT8	= 0x05,
		INT16	= 0x06,
		INT32	= 0x07,
		INT64	= 0x08,
		STRING	= 0x09,
		BOOL	= 0x0A,
		FLOAT	= 0x0B,
		ENUM	= 0x0C,
		DATETIME= 0x0D,
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

