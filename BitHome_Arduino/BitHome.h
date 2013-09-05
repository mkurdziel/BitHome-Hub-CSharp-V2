#ifndef BITHOME_H
#define BITHOME_H

#define BITHOME_DATA_VOID		0x00
#define BITHOME_DATA_UINT8		0x01
#define BITHOME_DATA_UINT16		0x02
#define BITHOME_DATA_UINT32		0x03
#define BITHOME_DATA_UINT64		0x04
#define BITHOME_DATA_INT8		0x05
#define BITHOME_DATA_INT16		0x06
#define BITHOME_DATA_INT32		0x07
#define BITHOME_DATA_INT64		0x08
#define BITHOME_DATA_STRING		0x09
#define BITHOME_DATA_BOOL		0x0A
#define BITHOME_DATA_FLOAT		0x0B
#define BITHOME_DATA_ENUM		0x0C
#define BITHOME_DATA_DATETIME	0x0D

#endif

#include <BitHomeParameter.h>
#include <BitHomeAction.h>
#include <BitHomeXBee.h>
#include <BitHomeClient.h>