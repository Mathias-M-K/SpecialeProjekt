#include <ESP8266WiFi.h>
#include <WiFiClient.h>

const char* ssid = "internet";
const char* password = "password";
const int port = 26;

WiFiServer server(port);
WiFiClient client;

long duration, distance; // Used to calculate distance
bool clientDisconnectNotify;
int incomingByte = 0;

void setup() {
  Serial.begin(115200);


  WiFi.begin(ssid, password);
  Serial.println("Connecting");

  while (WiFi.status() != WL_CONNECTED) {
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
  if (client) {
    Serial.println("Client connected");
    while (client.connected()) {
      if (client.available() > 0) {
        // read the incoming byte:
        incomingByte = client.read();

        // say what you got:
        Serial.print("I received: ");
        Serial.println(incomingByte, DEC);
      }
    }

    if (!client.connected() && !clientDisconnectNotify) {
      Serial.println("Client Disconnected");
      clientDisconnectNotify = true;
    }
  }
}
