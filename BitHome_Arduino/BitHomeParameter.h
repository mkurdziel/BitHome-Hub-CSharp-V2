#ifndef BITHOME_PARAMETER_H
#define BITHOME_PARAMETER_H

#include <stdint.h>

class BitHomeParameter {

public:

	BitHomeParameter();
	BitHomeParameter(char* name, int aType, uint8_t min, uint8_t max);
	BitHomeParameter(char* name, int aType, uint16_t min, uint16_t max);
	BitHomeParameter(char* name, int aType, uint32_t min, uint32_t max);
	BitHomeParameter(char* name, int aType, uint64_t min, uint64_t max);

	BitHomeParameter(char* name, int aType, int8_t min, int8_t max);
	BitHomeParameter(char* name, int aType, int16_t min, int16_t max);
	BitHomeParameter(char* name, int aType, int32_t min, int32_t max);
	BitHomeParameter(char* name, int aType, int64_t min, int64_t max);

	uint8_t getUInt8();
	uint16_t getUInt16();
	uint32_t getUInt32();
	uint64_t getUInt64();
	int8_t getInt8();
	int16_t getInt16();
	int32_t getInt32();
	int64_t getInt64();

	void setValue(uint8_t b1) {_value[0] = b1;}
	void setValue(uint8_t b1, uint8_t b2) {
		_value[0] = b1;
		_value[1] = b2;
	}
	void setValue(uint8_t b1, uint8_t b2, uint8_t b3, uint8_t b4) {
		_value[0] = b1;
		_value[1] = b2;
		_value[2] = b3;
		_value[3] = b4;
	}
	void setValue(uint8_t b1, uint8_t b2, uint8_t b3, uint8_t b4, uint8_t b5, uint8_t b6, uint8_t b7, uint8_t b8) {
		_value[0] = b1;
		_value[1] = b2;
		_value[2] = b3;
		_value[3] = b4;
		_value[4] = b5;
		_value[5] = b6;
		_value[6] = b7;
		_value[7] = b8;
	}

	uint8_t getDataType() { return _dataType; }
	char* getName() { return _name; }
	uint8_t getOptions() { return _options; }
	uint8_t* getMinimum() { return _minimum; }
	uint8_t* getMaximum() { return _maximum; }

	char* _name;
	uint8_t _dataType;
	uint8_t _options;	
	uint8_t* _value;
	uint8_t* _minimum;
	uint8_t* _maximum;
};

#endif
