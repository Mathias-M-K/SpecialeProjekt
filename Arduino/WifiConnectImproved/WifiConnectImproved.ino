#include <ESP8266WiFi.h>
#include <WiFiClient.h>

const char* ssid = "internet";
const char* password = "password";
const int port = 26;

WiFiServer server(port);
WiFiClient client;

bool clientDisconnectNotify = true;
void setup() {
  Serial.begin(115200);
  Serial.println("");

  WiFi.begin(ssid, password);
  Serial.print("Connecting");

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
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
  if (client) {
    Serial.println("");
    Serial.println("Client connected");
    clientDisconnectNotify = false;

    
    while (client.connected()) {

      if (client.available() > 0) {
        Serial.print("Data Available:");
        String tempString;

        Serial.print("Reading...");
        while (client.available() > 0) {
          char c = client.read();
          tempString += c;
        }
        Serial.println(" Done!");
        Serial.print("Recived Data: ");
        Serial.println(tempString);
      }
      
      float sensorVal = analogRead(A0);

      // Send the distance to the client, along with a break to separate our messages
      client.print(sensorVal);
      client.print('\r');

      // Delay before the next reading
      delay(10);
    }
  }else{
    if(!clientDisconnectNotify){
      Serial.println("Client Disconnected");
      clientDisconnectNotify = true;
    }
    
  }
}
