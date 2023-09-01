#include <SPI.h>
#include <ESP32Servo.h>
#include "BluetoothSerial.h"
#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif

BluetoothSerial SerialBT;
// ピン定義。
#define PIN_Servo 16

Servo _myServo;

void setup() {
  // espの設定
  SerialBT.begin("FreedomServo"); //Bluetooth device name
  Serial.begin(9600);
  delay(1000);
  _myServo.attach(PIN_Servo);
  _myServo.write(0);
}

void loop() {
  if(SerialBT.available()){
    String command = SerialBT.readStringUntil('\n');
    int spd = command.toInt();
    _myServo.write(spd);
  }
}
