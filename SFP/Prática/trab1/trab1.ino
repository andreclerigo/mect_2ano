/* 2.2.4
// Controlo da saída digital nº2
void setup() {
    pinMode(2, OUTPUT);     //Pino 2 passa a ser saída
}

void loop() {
    // put your main code here, to run repeatedly:
    digitalWrite(2, HIGH);  // ativa Pino 2 (3,3 V) e o Led azul pequeno desliga
    delay(1000);            // espera 1000 ms
    digitalWrite(2,LOW);    // Pino nº 2 passa a 0v e o Led azul pequeno acende
    delay(1000);            // espera 1000 ms
}
*/

/* 2.2.5
// Leitura da entrada digital nº 4 e Controlo da saída digital nº 2
void setup() {
    pinMode(2, OUTPUT);     //Pino 2 passa a ser saída
    pinMode(4, INPUT);      //Pino 4 passa a ser entrada
}

int varA;

void loop() {
    varA = digitalRead(4);  // Lê o Pino 4 (0 ou 3,3 V)
    digitalWrite(2, varA);  // ativa/desativa Pino 2 (3,3 ou 0 V)

    // quando press no pin4 fica a low
}
*/

/* 2.2.6
// Controlo da saída digital nº 13 PWM
void setup() {
    pinMode(13,OUTPUT);     //Pino 13 passa a ser saída
}
void loop() {
    analogWrite(13, 0);     // ativa Pino 13 com 0 V
    delay(500);             // espera 0,5 seg
    analogWrite(13, 63);    // ativa Pino 13 com 3,3/4 V
    delay(500);
    analogWrite(13, 127);   // ativa Pino 13 com 3,3/2 V
    delay(500);
    analogWrite(13, 255);   // ativa Pino 13 com 3,3 V
    delay(500);
} 
*/

/* 2.2.7
// Leitura da entrada analógica
// Controlo da saída digital nº 13 PWM
void setup() {
    pinMode(13, OUTPUT);    // Pino 13 passa a ser saída
}

int ana;

void loop() {
    ana = analogRead(A0);   // Lê pino ADC 0 a 3v3 (0..1024)
    analogWrite(13, ana/4); // ativa Pino 13 com a tensão lida
    delay(500);             // espera ,5 seg

} 
*/

/* Notes:
On this exercise I will use the ESP buttons to simulate the water level.
When the button is pressed, it detects water, ich means, the binary value 0/LOW indicates that the button is pressed.
*/

#define GREEN_LED           12
#define MAX_LEVEL_BUTTON    4
#define MIN_LEVEL_BUTTON    0
#define ULTRASONIC_SENSOR   A0
#define LED_THRESHOLD       80


int water_level, max_level, min_level;                      // Variables to store the sensor values
bool motor_state = false;                                   // Motor state, true = on, false = off

void setup() {
    Serial.begin(115200);
    pinMode(GREEN_LED, OUTPUT);
    pinMode(MAX_LEVEL_BUTTON, INPUT);
    pinMode(MIN_LEVEL_BUTTON, INPUT);
}

void loop() {
    analogWrite(GREEN_LED, 0);                              // Motor is off
    water_level = analogRead(ULTRASONIC_SENSOR);            // Read ultrassonic sensor value
    max_level = digitalRead(MAX_LEVEL_BUTTON);              // Read press button status
    min_level = digitalRead(MIN_LEVEL_BUTTON);              // Read flash button status
    
    // Motor is off, check if the water level is low
    if (motor_state == false) {
        // Ultrassonic sensor checks the water level
        if (water_level < LED_THRESHOLD) {                  // Water level is low
            Serial.println("Tank is empty");
            Serial.println("Turning motor ON!");
            analogWrite(GREEN_LED, 255);                    // Motor is ON
            motor_state = true;                                  
        }
 
        // ESP buttons to simulate water level
        /*
        if (min_level == HIGH && max_level == HIGH) {       // Water level is low
            Serial.println("Tank is empty");
            Serial.println("Turning motor ON!");
            analogWrite(GREEN_LED, 255);                    // Motor is ON
            motor_state = true;                                  
        }
        */
    }

    // Motor fiiling the tank, check if the water level is high
    if (motor_state == true) {
        analogWrite(GREEN_LED, 255);                        // Motor is on
        if (min_level == LOW) {                             // Water is detected on the bottom
            Serial.println("Water detected on the bottom");
            if (max_level == LOW) {                         // Water is detected on the bottom and top
                Serial.println("The tank is full");
                Serial.println("Turning motor OFF!");
                analogWrite(GREEN_LED, 0);                  // Motor is off
                motor_state = false;
            }                                    
        }
    }

    // Debug info
    Serial.print("Motor state: " + String(motor_state));
    Serial.print(" | Ultrassonic sensor: " + String(water_level));
    Serial.print(" | Max button: " + String(digitalRead(MAX_LEVEL_BUTTON)));
    Serial.println(" | Min button: " + String(digitalRead(MIN_LEVEL_BUTTON)));

    delay(1000);
}
