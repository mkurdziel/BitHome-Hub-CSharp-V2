#ifndef BITHOMECLIENT_H
#define BITHOMECLIENT_H

#include <BitHomeXBee.h>
#include <BitHomeAction.h>

class BitHomeClient
{
public:
	BitHomeClient(int serialSpeed, int numActions, BitHomeAction* actions);

	void start(); 

	bool poll();
	void setDeviceVersion(uint8_t major, uint8_t minor);
	void setManufacturerId(uint16_t manufacturerId);

	void sendStatusResponse(uint8_t status);
	void sendInfoResponse();
	void sendCatalogResponse(uint8_t actionIndex);
	void sendParameterResponse(uint8_t actionIndex, uint8_t paramIndex);

	int _serialSpeed;
	BitHomeXBee* _xbee;

	uint8_t _numActions;
	uint8_t _numInterfaces;

	BitHomeAction* _actions;

	uint8_t _option;
	uint8_t _data;

	uint8_t _version_major;
	uint8_t _version_minor;
	uint16_t _manufacturerId;

	void flashLed(int pin, int times, int wait);
};

#endif