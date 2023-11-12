#include <ESP8266WiFi.h>

#define BLUE_LED 2
#define MAX_LEVEL_BUTTON 4
#define MIN_LEVEL_BUTTON 0
#define ULTRASONIC_SENSOR A0

const char *ssid = "idk";
const char *password = "tiagomarques";
const char *host = "192.168.43.175";
String url, path;
String gpio2_value, gpio4_value, gpio0_value, ultrasonic_value;
WiFiClient client;
const int httpPort = 80;

void setup()
{
    Serial.begin(115200);
    pinMode(MAX_LEVEL_BUTTON, INPUT);
    pinMode(MIN_LEVEL_BUTTON, INPUT);
    pinMode(BLUE_LED, OUTPUT);
    pinMode(ULTRASONIC_SENSOR, OUTPUT);

    WiFi.mode(WIFI_STA);
    WiFi.begin(ssid, password);
    while (WiFi.status() != WL_CONNECTED)
    {
        delay(500);
        Serial.print(".");
    }

    Serial.println(WiFi.localIP());
}

void loop()
{
    // TENTA LIGAR POR TCP/IP ao APACHE
    if (client.status() == 0)
    {
        Serial.print("connecting to ");
        Serial.println(host); // envia para o serial monitor
        if (!client.connect(host, httpPort))
        { // Tenta LIGAÇAO TCP
            Serial.println("connection failed");
            client.flush();
            delay(1000);
        }
    }

    // SE LIGACAO TCP ESTABELECIDA
    if (client.status() == 4)
    {
        gpio4_value = digitalRead(MAX_LEVEL_BUTTON);
        gpio0_value = digitalRead(MIN_LEVEL_BUTTON);
        gpio2_value = digitalRead(BLUE_LED);
        ultrasonic_value = analogRead(ULTRASONIC_SENSOR);

        path = "/CtrPLC.php";
        String postData = "saidaY0=1&saidaY1=100&saidaY2=200&saidaX0=" + gpio0_value + "&saidaX1=" + gpio4_value + "&saidaX2=" + gpio2_value + "&saidaX3=" + ultrasonic_value;

        // Construct the HTTP request
        url = String("POST ") + path + " HTTP/1.1\r\n" +
              "Host: " + host + "\r\n" +
              "Content-Type: application/x-www-form-urlencoded\r\n" +
              "Content-Length: " + String(postData.length()) + "\r\n" +
              "Connection: keep-alive\r\n\r\n" +
              postData;

        client.print(url);
        Serial.println("Enviou msg HTTP para o Apache, a pedir o ficheiro CtrPLC.php");
        Serial.println(url);

        // Espera pela resposta do Apache
        delay(100);
    }

    // SE CHEGARAM DADOS por TCP/IP
    if (client.available() > 0)
    {
        String line = client.readStringUntil('ç');
        if (line.indexOf("gpio2") != -1)
        {
            // Extract the value in front of "gpio2:"
            int index = line.indexOf("gpio2:") + 6; // 6 is the length of "gpio2:"
            String valueString = line.substring(index);

            Serial.println("Fetched from DB, GPIO2 is set at " + valueString);
            digitalWrite(BLUE_LED, valueString.toInt());
        }
        Serial.print(line);
        client.flush();
    }

    Serial.println();
    Serial.println();
    Serial.println();
    delay(1000);
}
