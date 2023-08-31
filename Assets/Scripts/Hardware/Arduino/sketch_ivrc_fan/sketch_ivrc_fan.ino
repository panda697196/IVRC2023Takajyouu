#include <SPI.h>
#include "BluetoothSerial.h"
#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif

BluetoothSerial SerialBT;
// ピン定義。
#define PIN_Relay 16

void setup() {
  // espの設定
  SerialBT.begin("FanTranser"); //Bluetooth device name
  delay(1000);
  pinMode(PIN_Relay, OUTPUT);

  digitalWrite(PIN_Relay, LOW);
}

void loop() {
  if(SerialBT.available()){
    String command = SerialBT.readStringUntil('\n');
    if(command == "S"){ // スイッチ切り替え命令
      Switching();
    }
  }
}

void Switching(){
  digitalWrite(PIN_Relay, HIGH);
  delay(100);
  digitalWrite(PIN_Relay, LOW);
  delay(100);
}