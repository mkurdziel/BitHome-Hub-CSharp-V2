#include <BitHome.h>

#include <Tlc5940.h>
#include <tlc_config.h>

#define LOOP_DELAY 	1 
#define PIN_LED 	4 
#define PIN_RESET   2
#define CHANNELS    5
#define FADE_DELAY  1
#define MAX_LIGHT	4095

#define R_MAX 4095 //4095
#define G_MAX 1000 // 1000
#define B_MAX 200 // 200

double _currentRed[CHANNELS];
double _currentGreen[CHANNELS];
double _currentBlue[CHANNELS];

uint16_t _desiredRed[CHANNELS];
uint16_t _desiredGreen[CHANNELS];
uint16_t _desiredBlue[CHANNELS];

double _deltaRed[CHANNELS];
double _deltaGreen[CHANNELS];
double _deltaBlue[CHANNELS];

BitHomeAction actions[] = {
    BitHomeAction( (char*)"RGB_Fade", (char*)"Fade Lights", BITHOME_DATA_VOID, &RGB_FadeLights, 4, 
    	(BitHomeParameter[]) {
			BitHomeParameter( (char*)"Red", BITHOME_DATA_UINT16,  (uint16_t)0, (uint16_t)4095),
			BitHomeParameter( (char*)"Green", BITHOME_DATA_UINT16, (uint16_t)0, (uint16_t)4095),
			BitHomeParameter( (char*)"Blue", BITHOME_DATA_UINT16, (uint16_t)0, (uint16_t)4095),
			BitHomeParameter( (char*)"Fade Time", BITHOME_DATA_UINT16, (uint16_t)0, (uint16_t)65535)
		}
    ),
    BitHomeAction( (char*)"RGB_FadeChannel", (char*)"Fade Channel", BITHOME_DATA_VOID, &RGB_FadeChannel, 5, 
    	(BitHomeParameter[]) {
			BitHomeParameter( (char*)"Red", BITHOME_DATA_UINT16,  (uint16_t)0, (uint16_t)4095),
			BitHomeParameter( (char*)"Green", BITHOME_DATA_UINT16, (uint16_t)0, (uint16_t)4095),
			BitHomeParameter( (char*)"Blue", BITHOME_DATA_UINT16, (uint16_t)0, (uint16_t)4095),
			BitHomeParameter( (char*)"Channel", BITHOME_DATA_UINT8, (uint8_t)0, (uint8_t)4),
			BitHomeParameter( (char*)"Fade Time", BITHOME_DATA_UINT16, (uint16_t)0, (uint16_t)65535)
		}
    ),
    BitHomeAction( (char*)"Light_Fade", (char*)"Fade Natural", BITHOME_DATA_VOID, &Light_Fade, 2, 
    	(BitHomeParameter[]) {
			BitHomeParameter( (char*)"Brightness", BITHOME_DATA_UINT8,  (uint8_t)0, (uint8_t)100),
			BitHomeParameter( (char*)"Fade Time", BITHOME_DATA_UINT16, (uint16_t)0, (uint16_t)65535)
		}
    )
};

BitHomeClient bithomeClient(115200, 3, actions);

void setup() {
	// put your setup code here, to run once:
	pinMode(PIN_LED, OUTPUT);

	// Setup LED Chip
  	Tlc.init();

	// Start the lights at natural
	resetValues();
	setLights(R_MAX, G_MAX, B_MAX, 1000);
	for (int i=0; i<1000; i++) {
		checkForLightsUpdate();
		delay(LOOP_DELAY);        
	}

	// Delay to let the radio startup
	blinkLED (4, 500);

	bithomeClient.setDeviceVersion(0,1);
	bithomeClient.setManufacturerId(1212);
	bithomeClient.start();
}

void loop() {
	// put your main code here, to run repeatedly: 

	// poll the client for any incoming messages
	bithomeClient.poll();

	// Check if lights need update
	checkForLightsUpdate();

	// delay in between reads for stability
	delay(LOOP_DELAY);        
}

void resetValues() 
{
	for (int i=0; i<CHANNELS; i++) {
		_currentRed[i] = 0;
		_currentGreen[i] = 0;
		_currentBlue[i] = 0;
		_desiredRed[i] = 0;
		_desiredGreen[i] = 0;
		_desiredBlue[i] = 0;
		_deltaRed[i] = 0;
		_deltaGreen[i] = 0;
		_deltaBlue[i] = 0;
	}
}

void RGB_FadeLights( BitHomeParameter* parameters) {
	uint16_t red = parameters[0].getUInt16();
	uint16_t green = parameters[1].getUInt16();
	uint16_t blue = parameters[2].getUInt16();
	uint16_t fade = parameters[3].getUInt16();

	// Insert your code here!
	setLights(red, green, blue, fade);
}

void RGB_FadeChannel( BitHomeParameter* parameters) {
	uint16_t red = parameters[0].getUInt16();
	uint16_t green = parameters[1].getUInt16();
	uint16_t blue = parameters[2].getUInt16();
	uint8_t channel = parameters[3].getUInt8();
	uint16_t fade = parameters[4].getUInt16();

	// Insert your code here!
	setLights(red, green, blue, channel, fade);
}

void Light_Fade( BitHomeParameter* parameters) {
	double brightness = (double)parameters[0].getUInt8();
	double percent = brightness / 100.0;
	uint16_t fade = parameters[1].getUInt16();

	// Insert your code here!
	setLights((double)R_MAX*percent, (double)G_MAX*percent, (double)B_MAX*percent, fade);
}

void blinkLED(int p_num, int p_delay)
{
  for( int i=0; i<p_num; i++) {
    digitalWrite(PIN_LED, HIGH);
    delay(p_delay);
    digitalWrite(PIN_LED, LOW);
    delay(p_delay);
  }
}

void setLights(uint16_t red, uint16_t green, uint16_t blue, uint16_t fadeTime)
{
	setLights(red, green, blue, 0, fadeTime);
	setLights(red, green, blue, 1, fadeTime);
	setLights(red, green, blue, 2, fadeTime);
	setLights(red, green, blue, 3, fadeTime);
	setLights(red, green, blue, 4, fadeTime);
}

void setLights(uint16_t red, uint16_t green, uint16_t blue, uint8_t channel, uint16_t fadeTime)
{
	// Set the desired colors
	_desiredRed[channel] = red;
	_desiredGreen[channel] = green;
	_desiredBlue[channel] = blue;

	// Set the deltas based on the fade time
	_deltaRed[channel] = _desiredRed[channel] - _currentRed[channel];
	_deltaGreen[channel] = _desiredGreen[channel] - _currentGreen[channel];
	_deltaBlue[channel] = _desiredBlue[channel] - _currentBlue[channel];

	if (fadeTime > 0) {
		_deltaRed[channel] = (double)_deltaRed[channel] / ((double)fadeTime/(double)LOOP_DELAY);
		_deltaGreen[channel] = (double)_deltaGreen[channel] / ((double)fadeTime/(double)LOOP_DELAY);
		_deltaBlue[channel] = (double)_deltaBlue[channel] / ((double)fadeTime/(double)LOOP_DELAY);
	}
}

void checkForLightsUpdate() {
	for (int i=0; i<CHANNELS; i++) {
		if (_currentRed[i] != _desiredRed[i] ||
			_currentGreen[i] != _desiredGreen[i] ||
			_currentBlue[i] != _desiredBlue[i]) {
			// Update the light values because something needs changing
			updateLights(i);
		}
	}

	Tlc.update();
}

void updateLights(uint8_t channel) 
{
	double red, green, blue;

	// If the current is higher than the desired then subtract
	red = _currentRed[channel] + _deltaRed[channel];
	// normalize
	if ((_deltaRed[channel] < 0 && red <= _desiredRed[channel]) ||
		(_deltaRed[channel] > 0 && red >= _desiredRed[channel])) {
		red = _desiredRed[channel];
	}

	// If the current is higher than the desired then subtract
	green = _currentGreen[channel] + _deltaGreen[channel];
	// normalize
	if ((_deltaGreen[channel] < 0 && green <= _desiredGreen[channel]) ||
		(_deltaGreen[channel] > 0 && green >= _desiredGreen[channel])) {
		green = _desiredGreen[channel];
	}

		// If the current is higher than the desired then subtract
	blue = _currentBlue[channel] + _deltaBlue[channel];
	// normalize
	if ((_deltaBlue[channel] < 0 && blue <= _desiredBlue[channel]) ||
		(_deltaBlue[channel] > 0 && blue >= _desiredBlue[channel])) {
		blue = _desiredBlue[channel];
	}
	_currentRed[channel] = red;
	_currentGreen[channel] = green;
	_currentBlue[channel] = blue;

    Tlc.set(0+(channel*3), (int)_currentRed[channel]);
    Tlc.set(1+(channel*3), (int)_currentGreen[channel]);
    Tlc.set(2+(channel*3), (int)_currentBlue[channel]);
}
