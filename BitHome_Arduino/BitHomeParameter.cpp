#include <BitHomeParameter.h>
#include <stdint.h>

BitHomeParameter::BitHomeParameter(char* name, int aType, uint8_t min, uint8_t max) {};
BitHomeParameter::BitHomeParameter(char* name, int aType, uint16_t min, uint16_t max) {};
BitHomeParameter::BitHomeParameter(char* name, int aType, uint32_t min, uint32_t max) {};
BitHomeParameter::BitHomeParameter(char* name, int aType, uint64_t min, uint64_t max) {};

BitHomeParameter::BitHomeParameter(char* name, int aType, int8_t min, int8_t max) {};
BitHomeParameter::BitHomeParameter(char* name, int aType, int16_t min, int16_t max) {};
BitHomeParameter::BitHomeParameter(char* name, int aType, int32_t min, int32_t max) {};
BitHomeParameter::BitHomeParameter(char* name, int aType, int64_t min, int64_t max) {};


uint8_t BitHomeParameter::getInt8() {};
uint16_t BitHomeParameter::getInt16() {};
uint32_t BitHomeParameter::getInt32() {}; 
uint64_t BitHomeParameter::getInt64() {};
int8_t BitHomeParameter::getInt8() {};
int16_t BitHomeParameter::getInt16() {};
int32_t BitHomeParameter::getInt32() {};
int64_t BitHomeParameter::getInt64() {};