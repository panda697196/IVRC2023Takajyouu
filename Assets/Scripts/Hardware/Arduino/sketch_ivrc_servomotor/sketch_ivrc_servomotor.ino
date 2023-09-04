#include <SPI.h>
#include <ESP32Servo.h>
#include "BluetoothSerial.h"
#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif

BluetoothSerial SerialBT;
// ピン定義。
#define PIN_BUTTON 16
#define PIN_Servo 27
int _buttonState = 0;

Servo _myServo;

void setup() {
  // espの設定
  SerialBT.begin("ServoMotor"); //Bluetooth device name
  delay(1000);
  pinMode(PIN_BUTTON, INPUT_PULLUP);
  _buttonState = digitalRead(PIN_BUTTON);
  
  _myServo.attach(PIN_Servo);
  _myServo.write(180);
}

void loop() {
  if(_buttonState != digitalRead(PIN_BUTTON)){
    SerialBT.println("AS"); // モータが止まったことをunityに知らせる
    delay(500);
    _buttonState = digitalRead(PIN_BUTTON);
  }
  
  if(SerialBT.available()){
    int spd = SerialBT.readStringUntil('\n').toInt();
    _myServo.write(spd);
  }
}
