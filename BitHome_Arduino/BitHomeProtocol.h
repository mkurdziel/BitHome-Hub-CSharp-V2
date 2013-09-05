#ifndef BITHOME_PROTOCOL_H
#define BITHOME_PROTOCOL_H

#include <stdint.h>

#define SYNET_START 0xA5

#define BITHOME_VERSION 0x01

////////////////////////////////////////////////////////////////////////////////
// SyNet API Defines
////////////////////////////////////////////////////////////////////////////////


//
// Status
// 
#define BITHOME_API_STATUSREQUEST      0x02
#define BITHOME_API_STATUSRESPONSE       0x03

//
// Info
//
#define BITHOME_API_INFOREQUEST 0x04
#define BITHOME_API_INFORESPONSE  0x05

//
// Catalog
//
#define BITHOME_API_CATALOGREQUEST     0x10
#define BITHOME_API_CATALOGRESPONSE    0x11

//
// Parameters
//
#define BITHOME_API_PARAMREQUEST       0x12
#define BITHOME_API_PARAMRESPONSE      0x13

//
// Functions
//
#define BITHOME_API_FUNCTIONTRANSMIT   0x40
#define SYNET_API_FUNCTIONRECEIVE    0x41

//
// Error
//
#define SYNET_API_ERROR              0xEE

////////////////////////////////////////////////////////////////////////////////
// SyNet API
////////////////////////////////////////////////////////////////////////////////
#define BITHOME_STATUS_HWRESET 0x00
#define BITHOME_STATUS_ACTIVE 0x01

// Bootload messages from controller
#define SYNET_API_BOOTLOADTX_REBOOT  0x00
#define SYNET_API_BOOTLOADTX_BLREQUEST     0x01
#define SYNET_API_BOOTLOADTX_DATATRANSMIT  0x03
#define SYNET_API_BOOTLOADTX_DATACOMPLETE  0x04

// Bootload messages to controller
#define SYNET_API_BOOTLOADRX_BOOTLOADREADY     0x00
#define SYNET_API_BOOTLOADRX_DATASUCCESS       0x01
#define SYNET_API_BOOTLOADRX_BOOTLOADCOMPLETE  0x02
#define SYNET_API_BOOTLOADRX_ERROR_STARTBIT    0x03
#define SYNET_API_BOOTLOADRX_ERROR_FRAMESIZE   0x04
#define SYNET_API_BOOTLOADRX_ERROR_API         0x05
#define SYNET_API_BOOTLOADRX_ERROR_16ADDR      0x06
#define SYNET_API_BOOTLOADRX_ERROR_BLAPI       0x07
#define SYNET_API_BOOTLOADRX_ERROR_BLSTART     0x08
#define SYNET_API_BOOTLOADRX_ERROR_PAGELENGTH  0x09
#define SYNET_API_BOOTLOADRX_ERROR_ADDRESS     0x0A
#define SYNET_API_BOOTLOADRX_ERROR_CHECKSUM    0x0B
#define SYNET_API_BOOTLOADRX_ERROR_SYNETSTART  0x0C
#define SYNET_API_BOOTLOADRX_ERROR_SYNETAPI    0x0D

#define SYNET_ID       0xABCD
#define SYNET_MANUFAC  0x1234
#define SYNET_PROFILE  0xAABB
#define SYNET_REVISION 0x0001

#define ZIGBEE_START 0x7E
#define ZIGBEE_API_RX 0x90
#define ZIGBEE_API_TX 0x10

// Define Frame Layout
#define FRAME_ZIGBEE_START 0
#define FRAME_LENGTH_MSB 1
#define FRAME_LENGTH_LSB 2
#define FRAME_ZIGBEE_RX_API 3
#define FRAME_ZIGBEE_64_ADDR 4
#define FRAME_ZIGBEE_16_ADDR 12

#define FRAME_SYNET_START 15
#define FRAME_SYNET_API 16
#define FRAME_SYNET_DATA 17
#define FRAME_SYNET_API_BOOTLOAD 17
#define FRAME_SYNET_API_BOOTLOAD_PAGE_LENGTH 18
#define FRAME_SYNET_API_BOOTLOAD_ADDRESS_MSB 19
#define FRAME_SYNET_API_BOOTLOAD_ADDRESS_LSB 20 
#define FRAME_SYNET_API_BOOTLOAD_CHECKSUM 21
#define FRAME_SYNET_API_BOOTLOAD_DATA 22

typedef union {
	uint16_t word;
	uint8_t  byte[2];
} two_byte_union;

enum DATA_TYPES 
{ 
  DT_VOID = 0, 
  DT_BYTE = 1, 
  DT_WORD = 2, 
  DT_STRING = 3, 
  DT_DWORD = 4
};

enum VALIDATION_TYPES
{
  PVT_UNSIGNED_FULL = 0,
  PVT_UNSIGNED_RANGE = 1,
  PVT_ENUMERATED = 2,
  PVT_MAX_STRING_LEN = 3,
  PVT_SIGNED_FULL = 10,
  PVT_SIGNED_RANGE = 11,
  PVT_UNKNOWN
};

#endif
