using System;
using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Messaging.Messages
{
    public class MessageDeviceStatusResponse : MessageBase
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public override Api Api {
			get {
				return Protocol.Api.DEVICE_STATUS_RESPONSE;
			}
		}

		public DeviceStatusValue DeviceStatus { get; private set; }

		public byte ProtocolVersion { get; private set; }
		public Version Revision { get; private set; }

		public MessageDeviceStatusResponse(
			Node p_sourceNode,
			DeviceStatusValue p_deviceStatus,
			byte protocolVersion,
			Version revision) :
			base(p_sourceNode, null)
		{
			Revision = new Version ();

			DeviceStatus = p_deviceStatus;
			ProtocolVersion = protocolVersion;
			Revision = revision;
		}

        public MessageDeviceStatusResponse(
            Node p_sourceNode,
            Node p_destinationNode,
            byte[] p_data,
            int p_dataOffset) :
                base(p_sourceNode, p_destinationNode)
        {
			Revision = new Version ();

			ProtocolVersion = p_data[p_dataOffset + 2];
			Revision.MajorVersion = p_data[p_dataOffset + 3];
			Revision.MinorVersion = p_data[p_dataOffset + 4];
			DeviceStatus = (DeviceStatusValue) (p_data[p_dataOffset + 5]);

			log.Trace (this.ToString ());
        }

		public override string ToString ()
		{
			return string.Format ("[MessageDeviceStatusResponse: Api={0}, Status={1}, Protocol={2}, Version={3}]", 
			                      Api, DeviceStatus, ProtocolVersion, Revision);
		}
    }
}
