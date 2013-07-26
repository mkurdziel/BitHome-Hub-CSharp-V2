using System;
using BitHomeProtocol = BitHome.Messaging.Protocol;

namespace BitHome.Messaging.Messages
{
	public class MessageBootloadTransmit : MessageTxBase
	{
        public BitHomeProtocol.BootloadTransmit State { get; private set; }
        public byte[] DataBytes { get; private set; }
        public UInt32 Address { get; private set; }
        public UInt32 Checksum { get; private set; }
        public UInt32 Blocksize { get; private set; }

		public override BitHomeProtocol.Api Api {
			get {
				return BitHomeProtocol.Api.BOOTLOAD_TRANSMIT;
			}
		}

        public MessageBootloadTransmit(BitHomeProtocol.BootloadTransmit state)
        {
            this.State = state;
        }

        public MessageBootloadTransmit(
            BitHomeProtocol.BootloadTransmit state,
            byte[] dataBytes,
            UInt32 address,
            UInt32 checksum,
            UInt32 blocksize)
        {
            this.State = state;
            this.DataBytes = dataBytes;
            this.Address = address;
            this.Checksum = checksum;
            this.Blocksize = blocksize;
        }

		public override byte[] GetBytes ()
		{
            switch (State)
            {
                case BitHomeProtocol.BootloadTransmit.REBOOT_DEVICE:
                case BitHomeProtocol.BootloadTransmit.BOOTLOAD_REQUEST:
                case BitHomeProtocol.BootloadTransmit.DATA_COMPLETE:
                    {
                        byte[] bytes = new byte[3];

                        bytes[0] = (byte)BitHomeProtocol.PacketValues.PACKET_START;
                        bytes[1] = (byte)BitHomeProtocol.Api.BOOTLOAD_TRANSMIT;
                        bytes[2] = (byte)State;

                        return bytes;
                    }
                case BitHomeProtocol.BootloadTransmit.DATA_TRANSMIT:
                    {
                        byte[] bytes = new byte[7 + DataBytes.Length];
                        bytes[0] = (byte)BitHomeProtocol.PacketValues.PACKET_START;
                        bytes[1] = (byte)BitHomeProtocol.Api.BOOTLOAD_TRANSMIT;
                        bytes[2] = (byte)State;
                        bytes[3] = (byte)Blocksize;
                        bytes[4] = (byte)(Address >> 8);
                        bytes[5] = (byte)(Address);
                        bytes[6] = (byte)Checksum;

                        int length = DataBytes.Length;
                        for (int i = 0; i < length; ++i)
                        {
                            bytes[i + 7] = DataBytes[i];
                        }

                        return bytes;
                    }
            }

            return null;
		}
	}
}

