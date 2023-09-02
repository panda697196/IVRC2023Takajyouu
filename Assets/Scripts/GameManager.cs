using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int gameSceneState;
    
    private bool callOnceFlag;

    public TextMeshProUGUI titleLabel; //タイトル画面
    public TextMeshProUGUI instructionLabel; //instructionUI
    public TextMeshPro scoreText; // UI 得点を表示するためのテキスト要素
    public TextMeshProUGUI thankYouText;// UI "Thank you for playing "を表示するためのテキスト要素。
    
    private int initialSpeed; // 初速
    private int initialSpeedFromCase2; // ケース2で得られた初速
    public int crowCountFromCase2; // ケース2で得られたカラスの数
    private int initialSpeedFromCase6; // ケース6で得られた初速
    public int crowCountFromCase6; // ケース6で得られたカラスの数
    private bool flyFlag; //とびたちフラグ

    public Eagle_Edit eagleEdit;
    public Eagle_Navigation eagleNavigation;
    public GameObject flyFlagObj;
    public Transform rawfingerPos;
    private Vector3 fingerPos;
    

    // Start is called before the first frame update
    void Awake()
    {
        gameSceneState = 0;
        //シーン遷移用変数 0:スタート,１：待機a, 2:飛び立ちa、3：帰還a、４：結果a,５：待機b、６：飛び立ちb、７：帰還b、8:結果b, 9:終了処理
        callOnceFlag = false; //１シーンに一回呼び出すときに使う変数,trueなら呼び出し済み

        //_uiDispalyaa=UI,Getcomponent<UIdisplayer>();
        // 
        // titleLabel = GameObject.Find("TitleLabel").GetComponent<Text>();//いる？
        // instructionLabel = GameObject.Find("InstructionLabel").GetComponent<Text>();

        // titleLabelとインストラクションの初期化
        titleLabel.gameObject.SetActive(false);
        instructionLabel.gameObject.SetActive(false);

        // Eagle_EditコンポーネントとEagle_Navigationコンポーネントの取得
        // eagleEdit = GetComponent<Eagle_Edit>();
        // eagleNavigation = GetComponent<Eagle_Navigation>();

        flyFlag = flyFlagObj.GetComponent<ArmAngle>().flyFlag;

        fingerPos = rawfingerPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Updata");
        switch (gameSceneState)
        {
            // スタートシーン
            case 0:
                // スタートシーンでの処理内容(毎フレーム)
                
                
                if (callOnceFlag == false) //一回だけ実行(if文の中)
                {
                    titleLabel.gameObject.SetActive(true);
                    instructionLabel.gameObject.SetActive(true);

                    titleLabel.text = "Takasyo"; //タイトル
                    instructionLabel.text = "Please press enter key"; // プロンプトテキストの設定

                    callOnceFlag = true;

                    // eagleNavigation.;
                }

                if (Input.GetKeyDown(KeyCode.Return) ) //鷹がうでにとまる
                {
                    callOnceFlag = false; //一回フラグの初期化

                    // ヘッダーとアラートを隠す
                    titleLabel.gameObject.SetActive(false);
                    instructionLabel.gameObject.SetActive(false);

                    gameSceneState = 1;
                }
                break;
            case 1:
                // イントロダクションでの処理内容(毎フレーム)
                
                
                if (callOnceFlag == false) //一回だけ実行(if文の中)
                {
                    titleLabel.gameObject.SetActive(true);
                    instructionLabel.gameObject.SetActive(true);

                    titleLabel.text = "Takasyo"; //タイトル
                    instructionLabel.text = "Please press enter key"; // プロンプトテキストの設定

                    callOnceFlag = true;

                    // eagleNavigation.;
                }

                if (Input.GetKeyDown(KeyCode.Return) ) //鷹がうでにとまる
                {
                    callOnceFlag = false; //一回フラグの初期化

                    // ヘッダーとアラートを隠す
                    titleLabel.gameObject.SetActive(false);
                    instructionLabel.gameObject.SetActive(false);

                    gameSceneState = 2;
                }
                break;
            // 待機a
            case 2:
                // Debug.Log("待機A");
                // 待機aでの処理内容(毎フレーム)
                flyFlag = flyFlagObj.GetComponent<ArmAngle>().flyFlag;
                Debug.Log("flyflag:" + flyFlag);
                
                if (callOnceFlag == false)
                {
                    // 待機aでの処理内容(1回)
                    Debug.Log("待機A");
                    callOnceFlag = true;
                }

                // if (Input.GetKeyDown(KeyCode.Return)) // 腕の振りを検出
                if (Input.GetKeyDown(KeyCode.Return) || flyFlag == true) // 腕の振りを検出
                {
                    // Enterキーが押されたら、次のゲーム状態
                    flyFlagObj.GetComponent<ArmAngle>().flyFlag = false; //flyflag初期化
                    callOnceFlag = false; // 
                    gameSceneState = 3; // 
                }
                break;
                
            // 飛び立ちa
            case 3:
                // 飛び立ちa（Case 2）
                if (callOnceFlag == false)
                {
                    
                    Debug.Log("飛び立ちA");
                    callOnceFlag = true;

                  
                   // Vector3 targetPosition = GetTargetPosition(); // 目標位置の取得
                   //float flightSpeed = GetFlightSpeed(); // 飛行速度を得る

                    //initialSpeedFromCase2 = flightSpeed;

                    

                    if (eagleEdit != null && eagleNavigation != null)
                    {
                        // Eagle_Editスクリプトを使用したターゲットの場所の設定
                        eagleEdit.SetEagleState(Eagle_Edit.EagleState.Takeoff);

                        // フライトの開始
                       // eagleEdit.TakeOff(targetPosition);

                        // Eagle_Navigationで目標位置と飛行速度を設定する
                       // eagleNavigation.SetTarget(targetPosition);
                       // eagleNavigation.SetSpeed(flightSpeed);
                    }
                    //カラス？？
                    crowCountFromCase2 = 38;//殺したカラス
                }


                if (Input.GetKeyDown(KeyCode.Return))
                {
                    callOnceFlag = false;
                    gameSceneState = 4;
                }

                break;
            // 帰還a
            case 4:
                // 帰還a（Case 3）
                if (callOnceFlag == false)
                {
                    Debug.Log("帰還A");
                    callOnceFlag = true;

                    // トラッカーから提供されたプレーヤーの手の位置を取得する。
                   // Vector3 playerHandPosition = GetPlayerHandPosition();

                    // Eagle_EditコンポーネントとEagle_Navigationコンポーネントの取得
                    Eagle_Edit eagleEdit = GetComponent<Eagle_Edit>();
                    Eagle_Navigation eagleNavigation = GetComponent<Eagle_Navigation>();

                    if (eagleEdit != null && eagleNavigation != null)
                    {
                        // Eagle_Editスクリプトを使用したターゲットの場所の設定
                        eagleEdit.SetEagleState(Eagle_Edit.EagleState.Takeoff);

                        // イーグルの目標位置をプレーヤーの手の位置に設定する。
                       // eagleNavigation.SetTarget(playerHandPosition);

                        // 必要に応じて飛行速度を設定する
                        //eagleNavigation.SetSpeed(yourSpeedValue);
                    }
                }


                if (Input.GetKeyDown(KeyCode.Return) )
                {
                    callOnceFlag = false;
                    gameSceneState = 5;
                }
               
                break;
            // 待機b
            case 5:

                // 待機aでの処理内容(毎フレーム)
                if (callOnceFlag == false)
                {
                    // 待機aでの処理内容(1回)
                    Debug.Log("待機B");
                    callOnceFlag = true;
                }

                if (Input.GetKeyDown(KeyCode.Return)) // 腕の振りを検出?
                {
                    // Enterキーが押されたら、次のゲーム状態
                    callOnceFlag = false; // 
                    gameSceneState = 6; // 
                }
                break;
                
            // 飛び立ちb
            case 6:
                // 飛び立ちa（Case 6)
                if (callOnceFlag == false)
                {

                    Debug.Log("飛び立ちB");
                    callOnceFlag = true;


                   // Vector3 targetPosition = GetTargetPosition(); // 目標位置の取得
                    //float flightSpeed = GetFlightSpeed(); // 飛行速度を得る

                    //initialSpeedFromCase6 = flightSpeed;

                    // Eagle_EditコンポーネントとEagle_Navigationコンポーネントの取得
                    Eagle_Edit eagleEdit = GetComponent<Eagle_Edit>();
                    Eagle_Navigation eagleNavigation = GetComponent<Eagle_Navigation>();

                    if (eagleEdit != null && eagleNavigation != null)
                    {
                        // Eagle_Editスクリプトを使用したターゲットの場所の設定
                        eagleEdit.SetEagleState(Eagle_Edit.EagleState.Takeoff);

                        // フライトの開始
                       // eagleEdit.TakeOff(targetPosition);

                        // Eagle_Navigationで目標位置と飛行速度を設定する
                      //  eagleNavigation.SetTarget(targetPosition);
                       // eagleNavigation.SetSpeed(flightSpeed);
                    }
                    crowCountFromCase6 = 39;//ころしかカラス
                }

                if (Input.GetKeyDown(KeyCode.Return) )
                {
                    callOnceFlag = false;
                    gameSceneState = 7;
                }
                break;
            // 帰還b（スコアボードを持ってくる鷹）
            case 7:
                //ーーー 帰還b（Case 7）毎フレームの処理ーーーーー
                
                
                //-------------------------------------------
                if (callOnceFlag == false)//一回だけの処理----------------------
                {
                    Debug.Log("帰還B");
                    callOnceFlag = true;

                    // トラッカーから提供されたプレーヤーの手の位置を取得する。
                    //Vector3 playerHandPosition = GetPlayerHandPosition();

                    if (eagleEdit != null && eagleNavigation != null)
                    {
                        // Eagle_Editスクリプトを使用したターゲットの場所の設定
                        eagleEdit.SetEagleState(Eagle_Edit.EagleState.Takeoff);

                        // イーグルの目標位置をプレーヤーの手の位置に設定する。
                       // eagleNavigation.SetTarget(playerHandPosition);

                        // 必要に応じて飛行速度を設定する
                       // eagleNavigation.SetSpeed(yourSpeedValue);
                    }
                }


                if (Input.GetKeyDown(KeyCode.Return) )
                {
                    callOnceFlag = false;
                    gameSceneState = 8;
                }


                break;
            // スコア表示
            case 8:
                //--------毎フレームの処理ーーーーーーーーーーーーーーーー
                //----------------------------------------------
                if (callOnceFlag == false)//一回だけの処理
                {

                    Debug.Log("结果B");
                    callOnceFlag = true;

                    // ケース6で計算した初速とカラスの数を用いてスコアを計算する
                    int finalScore = initialSpeedFromCase6 + crowCountFromCase6;

                    // UI Text要素にスコアを表示する
                    if (scoreText != null)
                    {
                        scoreText.text = "スコア: " + finalScore.ToString();
                    }
                }

                if (Input.GetKeyDown(KeyCode.Return))//シーンの移行処理
                {
                    callOnceFlag = false;
                    gameSceneState = 9;
                 
                }
                break;
            // 終了処理
            case 9:
                // 終了処理
                if (callOnceFlag == false)
                {
                    // 
                    Debug.Log("終了処理");
                    callOnceFlag = true;

                    // 
                    if (thankYouText != null)
                    {
                        thankYouText.text = "Thank you for playing";
                    }
                   
                }

                //
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    callOnceFlag = false; // 
                    gameSceneState = 0; // 
                }
                break;
        }
        
        
    }
    
    public int GetgameSceneState()
    {
        return gameSceneState;
    }
}
