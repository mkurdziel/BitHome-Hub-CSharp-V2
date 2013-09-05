#include <stdarg.h>
#include <BitHomeAction.h>
#include <BitHomeParameter.h>

BitHomeAction::BitHomeAction(char* apiName, char* description, uint8_t returnType, 
		action_function action,	
		int numParams, BitHomeParameter* parameters) {

	_apiName = apiName;
	_description = description;
	_numParameters = numParams;
	_parameters = new BitHomeParameter [numParams];
	_action = action;

	for (int i = 0; i < numParams; i++) {
    	_parameters[i] = parameters[i];
	}
}