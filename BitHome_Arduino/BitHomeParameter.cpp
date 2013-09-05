#include <BitHome.h>
#include <BitHomeParameter.h>
#include <stdint.h>

BitHomeParameter::BitHomeParameter() {};
BitHomeParameter::BitHomeParameter(char* name, int datatype, uint8_t min, uint8_t max) 
{
	_name = name;
	_dataType = datatype;
	_value = new uint8_t[1];
	_minimum = new uint8_t[1];
	_minimum[0] = min;
	_maximum = new uint8_t[1];
	_maximum[0] = max;
};
BitHomeParameter::BitHomeParameter(char* name, int datatype, uint16_t min, uint16_t max) 
{
	_name = name;
	_dataType = datatype;
	_value = new uint8_t[2];
	_minimum = new uint8_t[2];
	_minimum[0] = min>>8;
	_minimum[1] = min;
	_maximum = new uint8_t[2];
	_maximum[0] = max>>8;
	_maximum[1] = max;
};
BitHomeParameter::BitHomeParameter(char* name, int datatype, uint32_t min, uint32_t max)
{
	_name = name;
	_dataType = datatype;
	_value = new uint8_t[4];
	_minimum = new uint8_t[4];
	_minimum[0] = min>>24;
	_minimum[1] = min>>16;
	_minimum[2] = min>>8;
	_minimum[3] = min;
	_maximum = new uint8_t[4];
	_maximum[0] = max>>24;
	_maximum[1] = max>>16;
	_maximum[2] = max>>8;
	_maximum[3] = max;
};
BitHomeParameter::BitHomeParameter(char* name, int datatype, uint64_t min, uint64_t max)
{
	_name = name;
	_dataType = datatype;
	_value = new uint8_t[8];
	_minimum = new uint8_t[8];
	_minimum[0] = min>>56;
	_minimum[1] = min>>48;
	_minimum[2] = min>>40;
	_minimum[3] = min>>32;
	_minimum[4] = min>>24;
	_minimum[5] = min>>16;
	_minimum[6] = min>>8;
	_minimum[7] = min;
	_maximum = new uint8_t[8];
	_maximum[0] = max>>56;
	_maximum[1] = max>>48;
	_maximum[2] = max>>40;
	_maximum[3] = max>>32;
	_maximum[4] = max>>24;
	_maximum[5] = max>>16;
	_maximum[6] = max>>8;
	_maximum[7] = max;
};

BitHomeParameter::BitHomeParameter(char* name, int datatype, int8_t min, int8_t max) 
{
	_name = name;
	_dataType = datatype;
	_value = new uint8_t[1];
	_minimum = new uint8_t[1];
	_minimum[0] = min;
	_maximum = new uint8_t[1];
	_maximum[0] = max;
};
BitHomeParameter::BitHomeParameter(char* name, int datatype, int16_t min, int16_t max)
{
	_name = name;
	_dataType = datatype;
	_value = new uint8_t[2];
	_minimum = new uint8_t[2];
	_minimum[0] = min>>8;
	_minimum[1] = min;
	_maximum = new uint8_t[2];
	_maximum[0] = max>>8;
	_maximum[1] = max;
};
BitHomeParameter::BitHomeParameter(char* name, int datatype, int32_t min, int32_t max)
{
	_name = name;
	_dataType = datatype;
	_value = new uint8_t[4];
	_minimum = new uint8_t[4];
	_minimum[0] = min>>24;
	_minimum[1] = min>>16;
	_minimum[2] = min>>8;
	_minimum[3] = min;
	_maximum = new uint8_t[4];
	_maximum[0] = max>>24;
	_maximum[1] = max>>16;
	_maximum[2] = max>>8;
	_maximum[3] = max;
};
BitHomeParameter::BitHomeParameter(char* name, int datatype, int64_t min, int64_t max)
{
	_name = name;
	_dataType = datatype;
	_value = new uint8_t[8];
	_minimum = new uint8_t[8];
	_minimum[0] = min>>56;
	_minimum[1] = min>>48;
	_minimum[2] = min>>40;
	_minimum[3] = min>>32;
	_minimum[4] = min>>24;
	_minimum[5] = min>>16;
	_minimum[6] = min>>8;
	_minimum[7] = min;
	_maximum = new uint8_t[8];
	_maximum[0] = max>>56;
	_maximum[1] = max>>48;
	_maximum[2] = max>>40;
	_maximum[3] = max>>32;
	_maximum[4] = max>>24;
	_maximum[5] = max>>16;
	_maximum[6] = max>>8;
	_maximum[7] = max;
};


uint8_t BitHomeParameter::getUInt8() { 
	if (_dataType == BITHOME_DATA_UINT8) {
		return _value[0];
	}
	return 0;
};

uint16_t BitHomeParameter::getUInt16(){ 
	uint16_t value = 0;
	if (_dataType == BITHOME_DATA_UINT16) {
		value |= _value[0];
		value <<= 8;
		value |= _value[1];

		return value;
	}
	return 0;
};

uint32_t BitHomeParameter::getUInt32() { 
	uint32_t value = 0;
	if (_dataType == BITHOME_DATA_UINT32) {
		value |= _value[0];
		value <<= 24;
		value |= _value[1];
		value <<= 16;
		value |= _value[2];
		value <<= 8;
		value |= _value[3];

		return value;
	}
	return 0;
};

uint64_t BitHomeParameter::getUInt64() { 
	uint64_t value = 0;
	if (_dataType == BITHOME_DATA_UINT32) {
		value |= _value[0];
		value <<= 56;
		value |= _value[1];
		value <<= 48;
		value |= _value[2];
		value <<= 40;
		value |= _value[3];
		value <<= 32;
		value |= _value[4];
		value <<= 24;
		value |= _value[5];
		value <<= 16;
		value |= _value[6];
		value <<= 8;
		value |= _value[7];

		return value;
	}
	return 0;
};

int8_t BitHomeParameter::getInt8() { 
	return 0;
};

int16_t BitHomeParameter::getInt16() { 
	return 0;
};

int32_t BitHomeParameter::getInt32() { 
	return 0;
};

int64_t BitHomeParameter::getInt64() { 
	return 0;
};