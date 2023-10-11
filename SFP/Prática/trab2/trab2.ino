#include <ESP8266WiFi.h>

#define BLUE_LED 2
#define MAX_LEVEL_BUTTON 4
#define MIN_LEVEL_BUTTON 0
#define ULTRASONIC_SENSOR A0

int gpio2_value, gpio4_value, gpio0_value, ultrassonic_value;
WiFiServer server(80);

const char *htmlCode = R"(
<!DOCTYPE html>
<html lang="pt-br">
  <head>
    <meta charset="utf-8" />
    <title>Trabalho 2</title>
    <style>
      /* Center the entire page vertically and horizontally */
      body {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        height: 100vh;
        margin: 0;
      }

      .row {
        display: flex;
        flex-wrap: wrap;
        justify-content: center;
        align-items: center;
      }

      .column {
        flex: 1;
        text-align: center;
      }

      img {
        max-width: 100%;
        height: auto;
      }

      /* Style the ON button as green */
      .btn-on {
        padding: 10px;
        font-size: 16px;
        border-radius: 5px;
        background-color: #4caf50;
        color: white;
        border: none;
        cursor: pointer;
      }

      .btn-on:hover {
        background-color: #3e8e41;
      }

      /* Style the OFF button as red */
      .btn-off {
        padding: 10px;
        font-size: 16px;
        border-radius: 5px;
        background-color: #f44336;
        color: white;
        border: none;
        cursor: pointer;
      }

      .btn-off:hover {
        background-color: #d32f2f;
      }

      p {
        font-size: 24px;
        font-weight: bold;
      }
    </style>
  </head>

  <body>
    <div class="row">
      <div class="column">
        <img
          src="https://forms.ua.pt/upload/themes/survey/UA_Default/files/LogoUAfacebook.png"
          alt="Logo Universidade de Aveiro"
        />
      </div>
      <div class="column">
        <p>Control Blue LED</p>
        <button class="btn-on" onclick="window.location.href='/GPIO=1'">ON</button>
        <button class="btn-off" onclick="window.location.href='/GPIO=0'">OFF</button>
      </div>
      <div class="column">
          <p>GPIO 0: <span id="gpio0Status" style="color: red;">OFF</span></p>
          <p>GPIO 4: <span id="gpio4Status" style="color: red;">OFF</span></p>
          <p>GPIO 2: <span id="gpio2Status" style="color: red;">OFF</span></p>
          <p>A0: <span id="analog0Value">0</span></p>
      </div>
    </div>
    <div class="row">
      <div class="column">
        <p>André Pragosa Clérigo, Número Mecanográfico: 98485</p>
      </div>
    </div>
  </body>
</html>
)";

void setup() {
    // Inicia o objecto Rs232
    Serial.begin(115200);
    // Pede ligação WiFi, ao Router
    WiFi.begin("idk", "tiagomarques");

    // espera que o Router aceite a ligação WiFi
    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
    }

    Serial.println("ESP connected to router, by Wifi");

    // ativa o servidor TCP/IP
    server.begin();
    Serial.println("Servidor TCP/IP ativo");

    // IP address
    Serial.println(WiFi.localIP());
    pinMode(BLUE_LED, OUTPUT);
    pinMode(MAX_LEVEL_BUTTON, INPUT);
    pinMode(MIN_LEVEL_BUTTON, INPUT);
    digitalWrite(BLUE_LED, LOW);
}

void loop() {
    // Espera que o Browser peça ligação TCP/IP
    WiFiClient client = server.available();
    if (client) { // Se chegou uma ligação TCPIP
        digitalWrite(BLUE_LED, HIGH);
        // Espera que o Browser envie dados , ex. GET index.html
        Serial.println("nova ligação TCP/IP \r\n");
        while (!client.available()) {
            delay(1);
        }

        Serial.println("Mensagem HTTP + HTML enviada PELO Browser");
        
        // Lê a mensagem recebida pelo ESP, enviada pelo Browser
        String ss = client.readStringUntil('\r'); // String ss = client.readString();
        Serial.println(ss);
        client.flush();
        // Processa mesg recebida pelo ESP
        if (ss.indexOf("GPIO=1") > 0) {
            gpio2_value = 1;
        }
        if (ss.indexOf("GPIO=0") > 0) {
            gpio2_value = 0;
        }
        digitalWrite(2, gpio2_value);

        // Envia pág HTML para o Browser
        client.print("HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE HTML>\r\n");
        client.print(htmlCode);

        gpio0_value = digitalRead(MIN_LEVEL_BUTTON);
        gpio4_value = digitalRead(MAX_LEVEL_BUTTON);
        ultrassonic_value = analogRead(ULTRASONIC_SENSOR);

        // Generate JavaScript based on the current GPIO status
        String script = R"(
        <script>
            document.getElementById('gpio2Status').innerHTML = ')" +
                        String(gpio2_value == 1 ? "ON" : "OFF") + R"(';
            document.getElementById('gpio2Status').style.color = ')" +
                        String(gpio2_value == 1 ? "green" : "red") + R"(';
            document.getElementById('gpio0Status').innerHTML = ')" +
                        String(gpio0_value == 0 ? "ON" : "OFF") + R"(';
            document.getElementById('gpio0Status').style.color = ')" +
                        String(gpio0_value == 0 ? "green" : "red") + R"(';
            document.getElementById('gpio4Status').innerHTML = ')" +
                        String(gpio4_value == 0 ? "ON" : "OFF") + R"(';
            document.getElementById('gpio4Status').style.color = ')" +
                        String(gpio4_value == 0 ? "green" : "red") + R"(';
            document.getElementById('analog0Value').innerHTML = ')" +
                        String(ultrassonic_value) + R"(';
        </script>
        )";
        client.print(script.c_str()); // Send the JavaScript

        // Debug Send the response to Rs232
        Serial.println("Mensagem HTTP + HTML enviada PARA o Browser");
        Serial.print("HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n<!DOCTYPE HTML>\r\n<html>\r\n");
        
        if (gpio2_value == 1) {
            Serial.println("GPIO is 1");
            digitalWrite(BLUE_LED, LOW);
        } else {
            Serial.println("GPIO is 0");
            digitalWrite(BLUE_LED, HIGH);
        }
        Serial.print("</html>\n");
    }
}
