#ifndef BITHOME_ACTION_H
#define BITHOME_ACTION_H

#include <stdarg.h>
#include <BitHomeParameter.h>

class BitHomeAction {

public:
    typedef void (*action_function)(BitHomeParameter*);

	BitHomeAction(char* apiName, char* description, uint8_t returnType, 
		action_function action,	
		int numParams, BitHomeParameter* parameters);


	BitHomeAction(char* apiName, char* description, uint8_t returnType, 
		action_function action,	
		int numParams, ...);

	BitHomeParameter getParameter(uint8_t parameterIndex) { return _parameters[parameterIndex];}
	BitHomeParameter* getParameters() { return _parameters; }
	uint8_t getReturnType() { return _returnType; }
	uint8_t getNumParameters() { return _numParameters; }
	uint8_t getOptions() { return _options; }
	char* getApiName() { return _apiName; }
	char* getDesc() { return _description; }
	action_function getFunction() { return _action; }

	BitHomeParameter* _parameters;
	action_function _action;
	char* _apiName;
	char* _description;
	uint8_t _returnType;
	uint8_t _numParameters;
	uint8_t _options;
};

#endif
