#include <BitHome.h>
#include <BitHomeClient.h>
#include <BitHomeXBee.h>
#include <BitHomeProtocol.h>
#include <Arduino.h>
#include <HardwareSerial.h>
// #include <SoftwareSerial.h>

BitHomeClient::BitHomeClient(
	int serialSpeed, 
	int numActions, 
	BitHomeAction* actions)
{ 
	_serialSpeed = serialSpeed;
	_numActions = numActions;
	_actions = actions;
	_version_minor = 0;
	_version_major = 0;
	_manufacturerId = 0;
	_numInterfaces = 0;
	// SoftwareSerial mySerial(1, 2); 
	// _xbee = new BitHomeXBee(mySerial);
	_xbee = new BitHomeXBee(Serial);
}

void BitHomeClient::setDeviceVersion(uint8_t major, uint8_t minor) 
{
	_version_major = major;
	_version_minor = minor;
}

void BitHomeClient::setManufacturerId(uint16_t manufacturerId) {
	_manufacturerId = manufacturerId;
}

void BitHomeClient::start() {
	_xbee->begin(19200);

	sendStatusResponse(BITHOME_STATUS_HWRESET);
}

bool BitHomeClient::poll() {
	int byteIn;

	// If there's a packet available coming in, process it...
	if (_xbee->poll()) 
	{
		switch ( _xbee->getAPI() ) {
			case BITHOME_API_STATUSREQUEST:
				{
					sendStatusResponse(BITHOME_STATUS_ACTIVE);
				}
				break; 
			case BITHOME_API_INFOREQUEST:
				{
					sendInfoResponse();
				}
				break; 
			case BITHOME_API_CATALOGREQUEST:
				{
					byteIn = _xbee->getNextDataByte();
					sendCatalogResponse(byteIn);
				}
				break;
			case BITHOME_API_PARAMREQUEST:
				{
					uint8_t actionIndex = _xbee->getNextDataByte();
					uint8_t paramIndex = _xbee->getNextDataByte();
					sendParameterResponse(actionIndex, paramIndex);
				}
				break;
			case BITHOME_API_FUNCTIONTRANSMIT:
				{
					uint8_t actionIndex = _xbee->getNextDataByte();
					uint8_t options = _xbee->getNextDataByte();
					if (actionIndex < _numActions) {
						BitHomeAction action = _actions[actionIndex];

						// Copy the parameters
						for (int i=0; i<action.getNumParameters(); i++) {
							BitHomeParameter param = action.getParameter(i);

							switch (param.getDataType()) {
								case BITHOME_DATA_UINT8:
								case BITHOME_DATA_INT8:
									param.setValue(_xbee->getNextDataByte());
									break;
								case BITHOME_DATA_UINT16:
								case BITHOME_DATA_INT16:
									param.setValue(
										_xbee->getNextDataByte(),
										_xbee->getNextDataByte()
									);
									break;
								case BITHOME_DATA_UINT32:
								case BITHOME_DATA_INT32:
									param.setValue(
										_xbee->getNextDataByte(),
										_xbee->getNextDataByte(),
										_xbee->getNextDataByte(),
										_xbee->getNextDataByte()
									);
									break;
								case BITHOME_DATA_UINT64:
								case BITHOME_DATA_INT64:
									param.setValue(
										_xbee->getNextDataByte(),
										_xbee->getNextDataByte(),
										_xbee->getNextDataByte(),
										_xbee->getNextDataByte(),
										_xbee->getNextDataByte(),
										_xbee->getNextDataByte(),
										_xbee->getNextDataByte(),
										_xbee->getNextDataByte()
									);
									break;
							}
						}	
						action.getFunction()(action.getParameters());
					}
				}
				break;
   		}

   		return true;
	}

	return false;
}

void BitHomeClient::sendStatusResponse(uint8_t status) {
	_xbee->startNewPacket();
	_xbee->addDataByte(BITHOME_API_STATUSRESPONSE); // API
	_xbee->addDataByte(BITHOME_VERSION);			// Protocol Version
	_xbee->addDataByte(_version_major);				// Major version
	_xbee->addDataByte(_version_minor);				// Minor version
	_xbee->addDataByte(status);						// Device status
	_xbee->sendDataPacket();
}

void BitHomeClient::sendInfoResponse() {
	_xbee->startNewPacket();
	_xbee->addDataByte(BITHOME_API_INFORESPONSE); // API
	_xbee->addDataByte(_manufacturerId>>8);				// Manufac Id
	_xbee->addDataByte(_manufacturerId);				// Manufac Id
	_xbee->addDataByte(_numActions);				// Number of actions
	_xbee->addDataByte(_numInterfaces);				// Number of interfaces
	_xbee->sendDataPacket();
}

void BitHomeClient::sendCatalogResponse(uint8_t actionIndex) {
	if (actionIndex < _numActions) {
		BitHomeAction action = _actions[actionIndex];

		_xbee->startNewPacket();
		_xbee->addDataByte(BITHOME_API_CATALOGRESPONSE); // API
		_xbee->addDataByte(actionIndex);
		_xbee->addDataByte(action.getReturnType());
		_xbee->addDataByte(action.getNumParameters());
		_xbee->addDataByte(action.getOptions());
		_xbee->addDataString(action.getApiName());
		_xbee->addDataString(action.getDesc());
		_xbee->sendDataPacket();	
	}
}

void BitHomeClient::sendParameterResponse(uint8_t actionIndex, uint8_t paramIndex) {
	if (actionIndex < _numActions) {
		BitHomeAction action = _actions[actionIndex];

		if (action.getNumParameters() > paramIndex) {
			BitHomeParameter param = action.getParameter(paramIndex);

			_xbee->startNewPacket();
			_xbee->addDataByte(BITHOME_API_PARAMRESPONSE); // API
			_xbee->addDataByte(actionIndex);
			_xbee->addDataByte(paramIndex);
			_xbee->addDataByte(param.getDataType());
			_xbee->addDataByte(param.getOptions());
			_xbee->addDataString(param.getName());

			switch(param.getDataType()) {
				case BITHOME_DATA_UINT8 :
				case BITHOME_DATA_INT8 :
				case BITHOME_DATA_STRING :
					_xbee->addDataBytes(param.getMinimum(), 1);
					_xbee->addDataBytes(param.getMaximum(), 1);
					break;
				case BITHOME_DATA_UINT16 :
				case BITHOME_DATA_INT16 :
					_xbee->addDataBytes(param.getMinimum(), 2);
					_xbee->addDataBytes(param.getMaximum(), 2);
					break;
				case BITHOME_DATA_UINT32 :
				case BITHOME_DATA_INT32 :
					_xbee->addDataBytes(param.getMinimum(), 4);
					_xbee->addDataBytes(param.getMaximum(), 4);
					break;
				case BITHOME_DATA_UINT64 :
				case BITHOME_DATA_INT64 :
					_xbee->addDataBytes(param.getMinimum(), 8);
					_xbee->addDataBytes(param.getMaximum(), 8);
					break;
			}

			_xbee->sendDataPacket();	
		}
	}
}
