#include <string.h>
#include <HardwareSerial.h>
#include "Arduino.h"

#include "BitHomeXBee.h"
#include "BitHomeProtocol.h"

////////////////////////////////////////////////////////////////////////////////
// Begin
//
int BitHomeXBee::begin(long speed_p)
{
  m_serial.begin(speed_p);
  m_serial.flush();
  return 0;
}

////////////////////////////////////////////////////////////////////////////////
// Available
bool BitHomeXBee::available()
{
  return m_packetReady;
}

////////////////////////////////////////////////////////////////////////////////
// Poll
bool BitHomeXBee::poll()
{
  m_packetReady = false;

	if( m_serial.available() )
	{
		// Spool in all the serial
		while (m_serial.available() > 0 )
		{
			char c = m_serial.read();
			// Check for a packet start
			if( (m_packetStarted == true && c == ZIGBEE_START) || 
				 	 m_packetStarted == false )
			{

				m_packetIndex = 0;
			}
			// load the char into the packet
			if( m_packetIndex < MAX_IN )
			{
				m_in[m_packetIndex] = c;
				m_packetIndex++;
			}
			m_packetStarted = true;
		}

		// If we have serial, check it
		if (m_packetIndex > 0)
		{

			// Check zigbee start
			if( m_in[0] != ZIGBEE_START )
			{
				return false;
			}


			two_byte_union length;
			length.byte[1] = m_in[1];
			length.byte[0] = m_in[2];

			// Length is full index - 1 - 7e - length - checksum
			if( length.word != (m_packetIndex-4) )
			{
				// startNewPacket();
				// addDataByte(0xAA);
				// addDataByte(0xAA);
				// addDataByte(m_in[0]);
				// addDataByte(m_in[1]);
				// addDataByte(m_in[2]);
				// addDataByte(m_in[3]);
				// addDataByte(m_in[4]);
				// addDataByte(m_in[5]);
				// sendDataPacket();
				return false;
			}


			// Check zigbee packet
			if( m_in[FRAME_ZIGBEE_RX_API] != (char)ZIGBEE_API_RX )
			{
				// startNewPacket();
				// addDataByte(m_in[FRAME_ZIGBEE_RX_API]);
				// addDataByte(ZIGBEE_API_RX);
				// sendDataPacket();
				return false;
			}


			// Check synet start
		if( m_in[FRAME_SYNET_START] != (char)SYNET_START )
			{
				// startNewPacket();
				// addDataByte(m_in[FRAME_SYNET_START]);
				// addDataByte(SYNET_START);
				// sendDataPacket();
				return false;
			}

			// We made it this far
			m_packetStarted = false;
			m_packetIndex = FRAME_SYNET_DATA;
			m_packetReady = true;

		}
	}
	return m_packetReady;
}

////////////////////////////////////////////////////////////////////////////////
//  Send a data packet
void BitHomeXBee::sendDataPacket() {
  char checksum = 0;
	char zeroChar = 0x00;

	m_serial.write(ZIGBEE_START); // START
	two_byte_union frame_size;
  	frame_size.word = 14 + m_sendPacketIndex;

	m_serial.write( frame_size.byte[1] ); // Length MSB
	m_serial.write( frame_size.byte[0] ); // Length LSB

	m_serial.write(ZIGBEE_API_TX); // API
  checksum += ZIGBEE_API_TX;

  int i;
  for( i=0; i<13; i++) {
	  m_serial.write(zeroChar); 
  }


  for( i=0; i<m_sendPacketIndex; i++)
  {
    m_serial.write(m_out[i]);
    checksum+= m_out[i];
  }
	m_serial.write(0xff-checksum); // Checksum
}


uint8_t BitHomeXBee::getAPI()
{
  if (m_packetReady)
  {
    return m_in[FRAME_SYNET_API];
  }
  return 0;
}

////////////////////////////////////////////////////////////////////////////////
// GetNextDataByte
uint8_t BitHomeXBee::getNextDataByte()
{
  if (m_packetReady)
  {
    return m_in[m_packetIndex++];
  }
  return 0;
}

////////////////////////////////////////////////////////////////////////////////
// GetNextDataWord
uint16_t BitHomeXBee::getNextDataWord()
{
  two_byte_union retVal;
  if (m_packetReady)
  {
    retVal.byte[1] = m_in[m_packetIndex++];
    retVal.byte[0] = m_in[m_packetIndex++];
    return retVal.word;
  }
  return 0;
}

void BitHomeXBee::startNewPacket()
{
  m_sendPacketIndex = 0;
  addDataByte(SYNET_START);
}

void BitHomeXBee::addDataByte(uint8_t byte_p)
{
  m_out[m_sendPacketIndex++] = byte_p;
}


void BitHomeXBee::addDataBytes(uint8_t* byte_p, int length)
{
	for (int i=0; i<length; i++) {
  		m_out[m_sendPacketIndex++] = byte_p[i];
	}
}

void BitHomeXBee::addDataWord(uint16_t word_p)
{
  two_byte_union var;
  var.word = word_p;
  m_out[m_sendPacketIndex++] = var.byte[1];
  m_out[m_sendPacketIndex++] = var.byte[0];
}

void BitHomeXBee::addDataUnion(two_byte_union tbu_p)
{
  m_out[m_sendPacketIndex++] = tbu_p.byte[1];
  m_out[m_sendPacketIndex++] = tbu_p.byte[0];
}

void BitHomeXBee::addDataString(const char* str_p)
{
  int len = strlen(str_p);
  for( int i=0; i<len; i++ )
  {
    m_out[m_sendPacketIndex++] = str_p[i];
  }
  // Null char
  m_out[m_sendPacketIndex++] = 0x00;
}

// void BitHomeXBee::blinkLED(int p_num)
// {
//   for( int i=0; i<p_num; i++) {
//     digitalWrite(4, HIGH);
//     delay(100);
//     digitalWrite(4, LOW);
//     delay(100);
//   }
//     delay(400);
// }


