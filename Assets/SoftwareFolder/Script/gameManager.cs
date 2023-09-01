using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public int gameSceneState;

    public bool sceneTransitionFlag;

    private bool callOnceFlag;
    
    // Start is called before the first frame update
    void Awake()
    {
        gameSceneState = 0;
        //シーン遷移用変数 0:スタート,１：待機a, 2:飛び立ちa、3：帰還a、４：結果a,５：待機b、６：飛び立ちb、７：帰還b、8:結果b, 9:終了処理
        callOnceFlag = false; //１シーンに一回呼び出すときに使う変数,trueなら呼び出し済み
        sceneTransitionFlag = false;//デバッグ用フラグ
        
        //_uiDispalyaa=UI,Getcomponent<UIdisplayer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameSceneState)
        {
            // スタートシーン
            case 0:
                // スタートシーンでの処理内容(毎フレーム)
                if (callOnceFlag == false)
                {

                    Debug.Log("StartScene");
                    callOnceFlag = true;
                }

                
                if (sceneTransitionFlag == true)//シーン遷移処理
                {
                    callOnceFlag = false;//一回呼び出し用フラグの初期化
                    sceneTransitionFlag = false;//デバッグ用シーン遷移フラグの初期化
                    gameSceneState = 1;//シーンを1（待機A）へ
                }
                break;
            // 待機a
            case 1:
                // 待機aでの処理内容(毎フレーム)
                if (callOnceFlag == false)
                {
                    // 待機aでの処理内容(1回)
                    Debug.Log("待機A");
                    callOnceFlag = true;
                }

                if (sceneTransitionFlag == true)
                {
                    callOnceFlag = false;
                    sceneTransitionFlag = false;
                    gameSceneState = 2;
                }
                break;
            // 飛び立ちa
            case 2:
                // 飛び立ちaでの処理内容(毎フレーム)
                if (callOnceFlag == false)
                {
                    // 飛び立ちaでの処理内容(一回)
                    Debug.Log("飛び立ちA");
                    callOnceFlag = true;
                }
                
                if (sceneTransitionFlag == true)
                {
                    callOnceFlag = false;
                    sceneTransitionFlag = false;
                    gameSceneState = 3;
                }

                break;
            // 帰還a
            case 3:
                // 帰還aでの処理内容(毎フレーム)
                if (callOnceFlag == false)
                {
                    // 帰還aでの処理内容(一回)
                    Debug.Log("帰還A");
                    callOnceFlag = true;
                }
                
                if (sceneTransitionFlag == true)
                {
                    callOnceFlag = false;
                    sceneTransitionFlag = false;
                    gameSceneState = 4;
                }
               
                break;
            // 結果a
            case 4:
                // 結果aでの処理内容(毎フレーム)
                if (callOnceFlag == false)
                {
                    // 結果aでの処理内容(一回)
                    Debug.Log("結果A");
                    callOnceFlag = true;
                }
                
                if (sceneTransitionFlag == true)
                {
                    callOnceFlag = false;
                    sceneTransitionFlag = false;
                    gameSceneState = 5;
                }

                break;
            // 待機b
            case 5:
                // 待機bでの処理内容(毎フレーム)
                if (callOnceFlag == false)
                {
                    // 待機bでの処理内容(一回)
                    Debug.Log("待機B");
                    callOnceFlag = true;
                }
                
                if (sceneTransitionFlag == true)
                {
                    callOnceFlag = false;
                    sceneTransitionFlag = false;
                    gameSceneState = 6;
                }

                break;
            // 飛び立ちb
            case 6:
                // 飛び立ちbでの処理内容(毎フレーム)
                if (callOnceFlag == false)
                {
                    //一回
                    Debug.Log("飛び立ちB");
                    callOnceFlag = true;
                }
                
                if (sceneTransitionFlag == true)
                {
                    callOnceFlag = false;
                    sceneTransitionFlag = false;
                    gameSceneState = 7;
                }

                break;
            // 帰還b
            case 7:
                // 帰還bでの処理内容(毎フレーム)
                if (callOnceFlag == false)
                {
                    //一回
                    Debug.Log("帰還B");
                    callOnceFlag = true;
                }
                
                if (sceneTransitionFlag == true)
                {
                    callOnceFlag = false;
                    sceneTransitionFlag = false;
                    gameSceneState = 8;
                }

                break;
            // 結果b
            case 8:
                // 結果bでの処理内容(毎フレーム)
                if (callOnceFlag == false)
                {
                    //一回
                    Debug.Log("結果ｂ");
                    callOnceFlag = true;
                }
                
                if (sceneTransitionFlag == true)
                {
                    callOnceFlag = false;
                    sceneTransitionFlag = false;
                    gameSceneState = 9;
                }

                break;
            // 終了処理
            case 9:
                // 終了処理での処理内容(毎フレーム)
                if (callOnceFlag == false)
                {
                    //一回
                    Debug.Log("終了処理");
                    callOnceFlag = true;
                }
                
                if (sceneTransitionFlag == true)
                {
                    callOnceFlag = false;
                    sceneTransitionFlag = false;
                    gameSceneState = 0;
                }
                
                break;
            default:
                Debug.Log("ERROR:このシーン変数は無効");
                break;
        }
        
    }
}
