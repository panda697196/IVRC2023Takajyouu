#include <SPI.h>
#include "BluetoothSerial.h"
#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif

BluetoothSerial SerialBT;
// ピン定義。
#define PIN_BUTTON 16
int _buttonState = 0;

void setup() {
  // espの設定
  SerialBT.begin("ServoMotor"); //Bluetooth device name
  Serial.begin(9600);
  delay(1000);
  pinMode(PIN_BUTTON, INPUT_PULLUP);
  _buttonState = digitalRead(PIN_BUTTON);
}

void loop() {
  if(_buttonState != digitalRead(PIN_BUTTON)){
    Serial.print("AS");
    SerialBT.print("AS"); // モータが止まったことをunityに知らせる
    _buttonState = digitalRead(PIN_BUTTON);
  }
}