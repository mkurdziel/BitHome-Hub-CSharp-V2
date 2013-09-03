#include <BitHome.h>
#include <stdint.h>

void setup() {
  // put your setup code here, to run once:

	BitHomeParameter parameters1[] = {
		BitHomeParameter( (char*)"Red", BITHOME_DATA_UINT16,  (uint16_t)0, (uint16_t)4095),
		BitHomeParameter( (char*)"Green", BITHOME_DATA_UINT16, (uint16_t)0, (uint16_t)4095),
		BitHomeParameter( (char*)"Blue", BITHOME_DATA_UINT16, (uint16_t)0, (uint16_t)4095),
		BitHomeParameter( (char*)"Fade Time", BITHOME_DATA_UINT8, (uint16_t)0, (uint16_t)65535)
	};

	BitHomeAction actions[] = {
	    BitHomeAction( (char*)"RGB_FadeLights", (char*)"Fade Lights", BITHOME_DATA_VOID, 4, parameters1, &RGB_FadeLights) 
	};
}

void loop() {
  // put your main code here, to run repeatedly: 

}

void RGB_FadeLights( BitHomeParameter* parameters) {
	uint16_t red = parameters[0].getUint16();
	uint16_t green = parameters[1].getUint16();
	uint16_t blue = parameters[2].getUint16();
	uint16_t fade = parameters[3].getUint16();

}
