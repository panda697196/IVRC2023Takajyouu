#include <SPI.h>
#include "BluetoothSerial.h"
#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif

BluetoothSerial SerialBT;
// ピン定義。
#define PIN_STBY 19
#define PIN_IN1 17
#define PIN_IN2 5
#define PIN_PWM 18
#define PIN_BUTTON 16
bool _afterstop = false;
void setup() {
  // espの設定
  SerialBT.begin("DCMotorR"); //Bluetooth device name
  delay(1000);
  pinMode(PIN_STBY, OUTPUT);
  pinMode(PIN_IN1, OUTPUT);
  pinMode(PIN_IN2, OUTPUT);
  pinMode(PIN_PWM, OUTPUT);
  pinMode(PIN_BUTTON, INPUT_PULLUP);

  // モータの初期設定
  digitalWrite(PIN_STBY, HIGH);
  digitalWrite(PIN_IN1, LOW);
  digitalWrite(PIN_IN2, LOW);
  analogWrite(PIN_PWM, 0);
}

void loop() {
if(SerialBT.available()){
    String command = SerialBT.readStringUntil('\n');
    if(command == "S"){ // 停止命令
      stop();
    }
    else{
      int speed = SerialBT.readStringUntil('\n').toInt();
      if(command == "C"){ // 順転命令
        rotate(true, speed);
      }
      else if(command == "R"){ // 逆転命令
        rotate(false, speed);
      }
    }
  }

  if(digitalRead(PIN_BUTTON) == LOW && _afterstop == false){ // 紐を張るときのみ，ボタン入力によりモータを止めつつボタン入力無効の状態になる
    stop();
    _afterstop = true;
    SerialBT.print("AS"); // モータが止まったことをunityに知らせる
  }
  if(digitalRead(PIN_BUTTON) == HIGH && _afterstop == true){ // 紐が緩まったらボタンによるモータ停止が作動する状態にする
    _afterstop = false;
    SerialBT.print("BS"); // 糸が張っていないことをunityに知らせる
  }
}

/*引数
cw   : 回転方向(true:時計回り, false:反時計回り)
speed : 回転速度[0~255]
*/

void rotate(bool cw, int speed){ // モータを回す関数 boolに直す
  analogWrite(PIN_PWM, speed);
  if(cw){
    digitalWrite(PIN_IN1, HIGH);
    digitalWrite(PIN_IN2, LOW);
  }
  else{
    digitalWrite(PIN_IN1, LOW);
    digitalWrite(PIN_IN2, HIGH);
  }
}

void stop(){ // モータを止める関数
  digitalWrite(PIN_IN1, HIGH);
  digitalWrite(PIN_IN2, HIGH);
}