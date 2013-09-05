#ifndef H_SYNET_XBEE
#define H_SYNET_XBEE

#include <HardwareSerial.h>
#include "BitHomeProtocol.h"

#define MAX_IN 100
#define MAX_OUT 80
////////////////////////////////////////////////////////////////////////////////
// XBee Reader Object
class BitHomeXBee {
  private: // Member variables
    // hardware serial object for communicating with hardware
    HardwareSerial m_serial;

    // Is there a full frame ready
    bool m_packetReady;

    bool m_packetStarted;

    uint16_t m_packetIndex;

    char m_in[MAX_IN];
    char m_out[MAX_OUT];
    
    uint8_t m_sendPacketIndex;

  private: // Methods
  BitHomeXBee();

    
  public:
    // Constructor
    BitHomeXBee(HardwareSerial serial_p) : 
      m_serial(serial_p), 
      m_packetReady(false),
      m_packetStarted(false),
      m_packetIndex(0),
      m_sendPacketIndex(0)
    { 
    }

    // Start the XBee module
    int begin(long speed = 19200);
    
    // Return the availability of data
    bool available();

    // Operate the data pump
    bool poll();

    void startNewPacket();
    void addDataBytes(uint8_t* byte_p, int length);
    void addDataByte(uint8_t);
    void addDataWord(uint16_t);
    void addDataUnion(two_byte_union tbu_p);
    void addDataString(const char* str_p);

    void sendDataPacket();

    uint8_t getAPI();

    // Get the next data byte
    uint8_t getNextDataByte();
    
    // Get the next data word
    uint16_t getNextDataWord();

    // void blinkLED(int p_num);
};

#endif 
