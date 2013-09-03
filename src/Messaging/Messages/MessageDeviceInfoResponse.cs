using System;
using BitHome.Messaging.Protocol;
using NLog;
using System.Collections.Generic;

namespace BitHome.Messaging.Messages
{
	public class MessageDeviceInfoResponse : MessageBase
	{
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public override Api Api {
			get {
				return Protocol.Api.DEVICE_INFO_RESPONSE;
			}
		}

		public UInt16 ManufacturerId { get; private set; }
		public byte ActionCount { get; private set; }
		public byte InterfaceCount { get; private set; }
		public List<UInt16> Interfaces { get; private set; }

		public MessageDeviceInfoResponse(
			Node p_sourceNode,
			UInt16 manufacturerId,
			byte functionCount,
			byte interfaceCount,
			List<UInt16> interfaces) :
			base (p_sourceNode, null) 
		{
			this.ManufacturerId = manufacturerId;
			this.ActionCount = functionCount;
			this.InterfaceCount = interfaceCount;
			this.Interfaces = interfaces;
		}

		public MessageDeviceInfoResponse(
			Node p_sourceNode,
			Node p_destinationNode,
			byte[] p_data,
			int p_dataOffset) :
			base(p_sourceNode, p_destinationNode)
		{
			ManufacturerId = EBitConverter.ToUInt16 (p_data, p_dataOffset+2);
			ActionCount = p_data [p_dataOffset+4];
			InterfaceCount = p_data [p_dataOffset+5];

			Interfaces = new List<UInt16> ();
			for (int i=0; i<InterfaceCount; i++) {
				Interfaces.Add (EBitConverter.ToUInt16 (p_data, p_dataOffset+ 6 + (i * 2)));
			}
		}
	}
}
