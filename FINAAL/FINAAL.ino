#include <MCP47X6.h>

#include "Wire.h"
#include <MPU6050_light.h>

//------------------------------------------------------------------ INITIALISATION ------------------------------------------------------------------------------//

MPU6050 mpu(Wire);

// Sound level sensor
const int pinAdc = A3;
const int debounceIntervalMicrophone = 1000;
unsigned long lastDetectionTime = 0;
int sound;

// Hall sensors
const int hallPin1 = A0;
const float lowerThreshold = 2.46;
const float upperThreshold = 2.5;
bool pulseDetected = false;
const float wheelCircumference = 2.1;

// Sliding window variables
const int numReadings = 4;
float speedReadings[numReadings];
unsigned long lastPulseTimeHall = 0; // Fixed name
float speed = 0;
float averageSpeed = 0;
int readingCount = 0;

const unsigned long timeoutInterval = 1500;
const int buttonPin = 2;

//------------------------------------------------------------------ SETUP AND LOOP ------------------------------------------------------------------------------//

  void setup() {
    Serial.begin(115200);
    unsigned long currentTime = millis();
    lastDetectionTime = currentTime;
    // MPU6050 Gyroscope and Accelerometer setup
    Wire.begin();
    byte status = mpu.begin();
    Serial.print(F("MPU6050 status: "));
    Serial.println(status);
      while (status != 0) {
    }
    Serial.println(F("Calculating offsets, do not move MPU6050"));
    mpu.calcOffsets();
    Serial.println("Done!\n");

    // Initialize speed readings array
    for (int i = 0; i < numReadings; i++) {
      speedReadings[i] = 0;
    }

    pinMode(buttonPin, INPUT_PULLUP);
  }

//-------------------------------------------------------------------- MPU6050 Gyroscope and accelerometer ----------------------------------------------------------------//

  void loop() {
    mpu.update();
    Serial.print(round(mpu.getAngleZ()));
    Serial.print(",");

//-------------------------------------------------------------------- Microphone with debounce -------------------------------------------------------------------------//
  
    long sum = 0;
    for(int i=0; i<32; i++)
    {
        sum += analogRead(pinAdc);
    }

    sum >>= 5;
    
    if(sum >= 1000 ) {
     Serial.print("1");
   
    } else {
      Serial.print("0");
    }
    Serial.print(",");


//------------------------------------------------------------ Hall sensors for speed calculation --------------------------------------------------------------------------//

  int sensorValue = analogRead(hallPin1);
  float voltage = sensorValue * (5.0 / 1023.0);

  // Detect pulse based on voltage threshold
  if (voltage <= lowerThreshold && !pulseDetected) {
    unsigned long timeIntervalHall = millis() - lastPulseTimeHall;
    if (timeIntervalHall > 0) {
      speed = wheelCircumference / (timeIntervalHall / 1000.0);
      lastPulseTimeHall = millis(); // Fixed name
      addSpeedToWindow(speed);
    }
    pulseDetected = true;
  }

  //skip measurements in the same pulse
  if (voltage >= upperThreshold) {
    pulseDetected = false;
  }

  //reset if no pulse detected for too long
  if (millis() - lastPulseTimeHall > timeoutInterval) { // Fixed name
    resetSlidingWindow();
  }

  Serial.print(averageSpeed);
  Serial.print(",");

//------------------------------------------------------------------ Pressing the button ------------------------------------------------------------------------------//

  int buttonState = 1 - digitalRead(buttonPin);
  Serial.println(buttonState);
  }

//------------------------------------------------------------------ Handling the sliding window ------------------------------------------------------------------------//

  void addSpeedToWindow(float newSpeed) {
    if (readingCount < numReadings) {
      speedReadings[readingCount] = newSpeed;
      readingCount++;
    } else {
      for (int i = 1; i < numReadings; i++) {
        speedReadings[i - 1] = speedReadings[i];
      }
      speedReadings[numReadings - 1] = newSpeed;
    }

    float hallSum = 0;
    for (int i = 0; i < readingCount; i++) {
      hallSum += speedReadings[i];
    }
    averageSpeed = hallSum / readingCount;
  }

  void resetSlidingWindow() {
    for (int i = 0; i < numReadings; i++) {
      speedReadings[i] = 0;
    }
    readingCount = 0;
    averageSpeed = 0;
  }
