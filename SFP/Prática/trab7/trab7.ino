#include <ESP8266WiFi.h>
#include <PubSubClient.h>

const char *ssid = "idk";
const char *ssid_password = "tiagomarques";
const char *server_ip = "192.168.43.175";
String gpio2_value, gpio4_value, gpio0_value, ultrasonic_value;
WiFiClient TCP_Client;
PubSubClient client(TCP_Client);

void callback(char *topic, byte *payload, unsigned int length)
{
    Serial.print("Message arrived [");
    Serial.print(topic);
    Serial.print("] ");
    for (int i = 0; i < length; i++)
    {
        Serial.print((char)payload[i]);
    }

    Serial.println();
    // Switch on the LED if an 1 was received as first character
    if ((char)payload[0] == '1')
    {
        digitalWrite(2, LOW); // Turn the LED on (Note that LOW is the voltage level
    }
    else
    {
        digitalWrite(2, HIGH); // Turn the LED off by making the voltage HIGH
    }
}

void setup()
{
    pinMode(2, OUTPUT); // Initialize the BUILTIN_LED pin as an output
    Serial.begin(115200);
    // COnnect to WiFi
    Serial.println("\nConnecting to " + String(ssid));
    WiFi.mode(WIFI_STA);
    WiFi.begin(ssid, ssid_password);
    while (WiFi.status() != WL_CONNECTED)
    {
        delay(500);
        Serial.print(".");
    }
    Serial.println("\nWiFi connected");
    Serial.print("IP address: " + WiFi.localIP().toString());

    // Set MQTT Server
    client.setServer("192.168.43.175", 1883);
    client.setCallback(callback);
    client.connect("esp8266-98485");

    Serial.println("\nSubscribing control_gpio2");
    client.subscribe("control_gpio2");
}

void loop()
{
    if (!client.connected())
    {
        client.connect("esp8266");
    }
    client.loop();

    // Read GPIO values
    gpio2_value = String(digitalRead(2));
    gpio4_value = String(digitalRead(4));
    gpio0_value = String(digitalRead(0));

    // Publish GPIO values
    client.publish("gpio2", gpio2_value.c_str());
    client.publish("gpio4", gpio4_value.c_str());
    client.publish("gpio0", gpio0_value.c_str());

    // Publish ultrasonic sensor value
    ultrasonic_value = String(analogRead(A0));
    client.publish("ultrasonic", ultrasonic_value.c_str());

    delay(500);
}
