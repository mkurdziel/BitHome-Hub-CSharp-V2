#ifndef BITHOME_ACTION_H
#define BITHOME_ACTION_H

#include <BitHomeParameter.h>

class BitHomeAction {

public:

	BitHomeAction(char* apiName, char* description, int aType, 
		int numParams, BitHomeParameter* parameters,
		void (*)(BitHomeParameter*) action);


};

#endif
