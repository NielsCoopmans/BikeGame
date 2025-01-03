const int hallPin = A0;  // Analog input pin to read the sensor
const int ledPin = 13;   // LED to indicate sensor activity
int hallValue = 0;
int count = 0;

void setup() {
  pinMode(ledPin, OUTPUT);
  Serial.begin(115200);   // Initialize serial communication
}

void loop() {
  hallValue = analogRead(hallPin);  // Read the analog value from the sensor (0-1023)
  
  // Convert the analog reading to a voltage (for a 5V reference)
  float voltage = hallValue * (5.0 / 1023.0);
  
  // Print the voltage value
  Serial.print("Sensor Voltage: ");
  Serial.println(count);
  
  // Example logic: Light the LED if a strong South pole is detected
  if (voltage > 2.65 || voltage < 2.53) {  // Arbitrary threshold for South pole
    digitalWrite(ledPin, HIGH);
    count++;
  } else {
    digitalWrite(ledPin, LOW);
  }

  //delay(500);  // Delay for readability
}