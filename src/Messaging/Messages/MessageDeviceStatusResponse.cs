using BitHome.Messaging.Protocol;
using NLog;

namespace BitHome.Messaging.Messages
{
    internal class MessageDeviceStatusResponse : MessageBase
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly DeviceStatusValue m_deviceStatus;

        public DeviceStatusValue DeviceStatus
        {
            get { return m_deviceStatus; }
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
            //if (m_deviceStatus == DeviceStatusValue.INFO)
            //{
            //    if (p_data.Length > (p_dataOffset + 9))
            //    {
            //        m_synetID = EBitConverter.toUInt16(p_data, p_dataOffset + 3);
            //        m_manufacturerID = EBitConverter.toUInt16(p_data, p_dataOffset + 5);
            //        m_profile = EBitConverter.toUInt16(p_data, p_dataOffset + 7);
            //        m_revision = EBitConverter.toUInt16(p_data, p_dataOffset + 9);
            //    }
            //    else
            //    {
            //        log.warn("Received poorly formed info message from " + p_sourceNode.getDescString());
            //    }

            //    log.debug("Status:" + p_sourceNode.getDescString()+ " - "
            //            + m_deviceStatus +  
            //            " SyNetID:"+m_synetID+" manufacID:" + m_manufacturerID +
            //            " profile:" + m_profile + " revision:" + m_revision);
            //}
        }
    }
}
