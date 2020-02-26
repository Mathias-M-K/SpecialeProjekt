#include <ESP8266WiFi.h>
#include <WiFiClient.h>

const char* ssid = "Martin Router King";
const char* password = "password";
const int port = 26;

WiFiServer server(port);
WiFiClient client;

long duration, distance; // Used to calculate distance
bool clientDisconnectNotify;
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

  Serial.print("Port: "); 
  Serial.println(port);
 
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

        // Delay before the next reading
        delay(10);
    }

    if(!client.connected() && !clientDisconnectNotify){
      Serial.println("Client Disconnected");
      clientDisconnectNotify = true;
    }
  }
 } 
