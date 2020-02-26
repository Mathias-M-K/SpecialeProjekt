#include <ESP8266WiFi.h>
#include <WiFiClient.h>

const char* ssid = "Martin Router King";
const char* password = "password";

WiFiServer server(26);
WiFiClient client;

long duration, distance; // Used to calculate distance
int i = 0;
void setup() {
  Serial.begin(115200);

  
  WiFi.begin(ssid,password);
  Serial.println("Connecting");

  while(WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.print("Connected to "); 
  Serial.println(ssid);
  
  Serial.print("IP Address: "); 
  Serial.println(WiFi.localIP());
 
  // Start the TCP server
  server.begin();
}

void loop() {
  // Listen for connecting clients
  client = server.available();
  if (client){
    Serial.println("Client connected");
    while (client.connected()){

        float sensorVal = analogRead(A0);
        Serial.print("sensorVal: ");Serial.println(sensorVal);

        // Send the distance to the client, along with a break to separate our messages
        client.print(sensorVal);
        client.print('\r');

        i++;

        // Delay before the next reading
        delay(200);
    }
  }
 } 
