using System;
using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Messaging.Messages
{
    public class MessageDeviceStatusResponse : MessageBase
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly DeviceStatusValue m_deviceStatus;
		private UInt16 m_revision;

		public override Api Api {
			get {
				return Protocol.Api.DEVICE_STATUS_RESPONSE;
			}
		}

        public DeviceStatusValue DeviceStatus
        {
            get { return m_deviceStatus; }
        }

		public UInt16 Revision
		{
			get { return m_revision; }
		}

		public MessageDeviceStatusResponse(
			Node p_sourceNode,
			DeviceStatusValue p_deviceStatus,
			UInt16 p_revision) :
			base(p_sourceNode, null)
		{
			m_deviceStatus = p_deviceStatus;
			m_revision = p_revision;
		}

        public MessageDeviceStatusResponse(
            Node p_sourceNode,
            Node p_destinationNode,
            byte[] p_data,
            int p_dataOffset) :
                base(p_sourceNode, p_destinationNode)
        {

            m_deviceStatus = (DeviceStatusValue) (p_data[p_dataOffset + 2]);

            log.Trace("status: {0} length: {1}", m_deviceStatus, p_data.Length);


            // Parse out any additional info if necessary
            if (m_deviceStatus == DeviceStatusValue.INFO)
            {
                if (p_data.Length > (p_dataOffset + 9))
                {
//                    m_synetID = EBitConverter.toUInt16(p_data, p_dataOffset + 3);
//                    m_manufacturerID = EBitConverter.toUInt16(p_data, p_dataOffset + 5);
//                    m_profile = EBitConverter.toUInt16(p_data, p_dataOffset + 7);
                    m_revision = EBitConverter.ToUInt16(p_data, p_dataOffset + 9);
                }
                else
                {
					log.Warn("Received poorly formed info message from {0}", p_sourceNode.Identifier);
                }

				log.Debug ("Status:{0} Revision:{1}", p_sourceNode.Identifier, m_revision);
            }
        }
    }
}
