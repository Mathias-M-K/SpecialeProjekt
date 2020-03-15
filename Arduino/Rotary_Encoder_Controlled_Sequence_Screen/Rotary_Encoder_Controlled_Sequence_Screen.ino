#define FASTLED_ESP8266_NODEMCU_PIN_ORDER

#include <FastLED.h>

/*
   struck Character defines how a character is build.
   c is which character the strucks represents, such as 'a' or 'd' or '2'
   x is the array for x coordinates
   y is the array for y coordiantes
   and len is the length of the arrays. The x and y array are always the same length, else you fucked up
*/
struct Character {
  char c;     //The represented character
  int x[30];  //array for x coordinates
  int y[30];  //array for y coordinates
  int len;    //lenth of arrays
};

struct Direction {
  Character c;
  CRGB color;
};

/*
   Things that you should not touch
*/
#define LED_PIN  3
#define COLOR_ORDER GRB
#define CHIPSET     WS2811
#define BRIGHTNESS 64

#define OUTPUT_A 14
#define OUTPUT_B 12
#define BTN_PIN 5

const uint8_t kMatrixWidth = 8;
const uint8_t kMatrixHeight = 8;
const bool    kMatrixSerpentineLayout = false;

#define NUM_LEDS (kMatrixWidth * kMatrixHeight)

CRGB leds_plus_safety_pixel[ NUM_LEDS + 1];
CRGB* const leds( leds_plus_safety_pixel + 1);

/*
   Rotary Encoder Values
*/
int counter = 0;
int aState;
int aLastState;

/*
   LED matrix values
*/
int internalOffset = 0;


/*
   Predefined Characters
*/
Character arrow_up = {'u', {5, 5, 5, 7, 6, 5, 4, 3, 6, 5, 4, 5}, {0, 1, 2, 3, 3, 3, 3, 3, 4, 4, 4, 5}, 11};
Character arrow_down = {'d', {5, 5, 5, 7, 6, 5, 4, 3, 6, 5, 4, 5}, {5, 4, 3, 2, 2, 2, 2, 2, 1, 1, 1, 0}, 11};
Character arrow_left = {'l', {7, 6, 5, 4, 4, 4, 4, 4, 3, 3, 3, 2}, {2, 2, 2, 4, 3, 2, 1, 0, 3, 2, 1, 2}, 11};
Character arrow_right = {'r', {5, 5, 5, 7, 6, 5, 4, 3, 6, 5, 4, 5}, {0, 1, 2, 3, 3, 3, 3, 3, 4, 4, 4, 5}, 11};

/*
   Predefined colors
*/
CRGB red = CRGB(255, 66, 66);
CRGB blue = CRGB(70, 163, 253);
CRGB green = CRGB(27, 185, 0);
CRGB yellow = CRGB(247, 255, 42);

/*
   Sequence values
*/
Direction sequence[16];
int sequenceCounter = 0;
int currentLengthOfSequence;


//////////////Settings////////////
/*
   Lock Rotary Encoder
   True = Rotary encoder can not scroll futher than the content
   False = Rotary encoder can scroll indefinitely
*/
bool lockRotaryEncoder = true;

void setup() {
  FastLED.addLeds<CHIPSET, LED_PIN, COLOR_ORDER>(leds, NUM_LEDS).setCorrection(TypicalSMD5050);
  FastLED.setBrightness(BRIGHTNESS);

  Serial.begin(74880);
  pinMode (OUTPUT_A, INPUT);
  pinMode (OUTPUT_B, INPUT);

  aLastState = digitalRead(OUTPUT_A);

  Direction d1 = {arrow_left, blue};
  Direction d2 = {arrow_left, blue};
  Direction d3 = {arrow_up, red};
  Direction d4 = {arrow_down, green};
  Direction d5 = {arrow_right, yellow};

  AddToSequence(d1);
  AddToSequence(d2);
  AddToSequence(d3);
  AddToSequence(d4);
  AddToSequence(d5);

}

void loop() {
  ReadEncoder();
  PrintSequence();
}



//Prints the directions in the sequence list
void PrintSequence() {
  internalOffset = 0; //Resetting offset
  turnOff();

  for (int i = 0; i < sequenceCounter; i++) {
    turnOnLeds(GetCharacter(sequence[i].c.c), sequence[i].color);
  }
  FastLED.show();
}

//Adds a direction to the sequence list
void AddToSequence(Direction d) {
  if (sequenceCounter == 16) {
    return;
  }

  sequence[sequenceCounter] = d;
  sequenceCounter++;

  currentLengthOfSequence = CalculateLengthOfSequence();
  Serial.print("New sequenceLength: "); Serial.println(currentLengthOfSequence);
}

//Clears sequence
void ClearSequence() {
  sequenceCounter = 0;
}

/*
  //Writes string
  void writeString(String s) {
  internalOffset = 0; //Resetting offset
  turnOff();

  for (int i = 0; i < s.length(); i++) {
    turnOnLeds(GetCharacter(s.charAt(i)),red);
  }

  FastLED.show();
  }
*/

void turnOnLeds(Character c, CRGB color) {
  for (int i = 0; i <= c.len; i++) {

    //Code for checking if any coordinates are out of bounds
    int positionToBeChecked = c.x[i] - counter - internalOffset;
    if (positionToBeChecked > 7 || positionToBeChecked < 0) {
      continue;
    }

    leds[ XY(c.x[i] - counter - internalOffset, c.y[i])]  = color;
  }
  internalOffset += GetWidth(c) + 1;
}

/*
   ReadEncoder manages the raw counter value produced from the rotary encoder.
   It reads the encoder, and assigns the value to the global counter value
*/
void ReadEncoder() {
  aState = digitalRead(OUTPUT_A); // Reads the "current" state of the outputA
  // If the previous and the current state of the outputA are different, that means a Pulse has occured
  if (aState != aLastState) {
    // If the outputB state is different to the outputA state, that means the encoder is rotating clockwise
    if (digitalRead(OUTPUT_B) != aState) {
      counter ++;
    } else {
      counter --;
    }
    Serial.print("Position: ");
    Serial.println(counter);
  }
  aLastState = aState; // Updates the previous state of the outputA with the current state

  if(lockRotaryEncoder){
    ContentPositionCorrection();
  }
}
void ContentPositionCorrection() {
  if (counter > 0) {
    counter = 0;
  }
  if (counter < -currentLengthOfSequence + 8) {
    counter = -currentLengthOfSequence + 8;
  }
}

int CalculateLengthOfSequence() {

  int tempValue = 0;
  for (int i = 0; i < sequenceCounter; i++) {
    Direction d = sequence[i];

    tempValue += GetWidth(d.c) + 1;
  }
  tempValue--;

  return tempValue;
}

//Returnt the width for a charater in pixels
int GetWidth(Character c) {

  int lowestValue = 7;

  for (int i = 0; i <= c.len; i++) {
    if (c.x[i] < lowestValue) {
      lowestValue = c.x[i];
    }
  }
  return 8 - lowestValue;
}

//Sets all leds to black (basically off)
void turnOff() {
  for (int i = 0; i < 8; i++) {
    for (int y = 0; y < 8; y++) {
      leds[ XY(i, y)]  = CHSV( 0, 0, 0);
    }
  }
}

//Returns a Character struct based on the parsed char
Character GetCharacter(char c) {
  switch (c) {
    case 'u':
      return arrow_up;
      break;
    case 'd':
      return arrow_down;
      break;
    case 'l':
      return arrow_left;
      break;
    case 'r':
      return arrow_right;
      break;
  }
}

/*
   ///////////////////LED help Mathods///////////////////
*/
uint16_t XY( uint8_t x, uint8_t y)
{
  uint16_t i;

  if ( kMatrixSerpentineLayout == false) {
    i = (y * kMatrixWidth) + x;
  }

  if ( kMatrixSerpentineLayout == true) {
    if ( y & 0x01) {
      // Odd rows run backwards
      uint8_t reverseX = (kMatrixWidth - 1) - x;
      i = (y * kMatrixWidth) + reverseX;
    } else {
      // Even rows run forwards
      i = (y * kMatrixWidth) + x;
    }
  }

  return i;
}
