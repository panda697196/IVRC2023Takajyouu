using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public int gameSceneState;

    public bool sceneTransitionFlag;

    private bool callOnceFlag;

    public Text titleLabel;
    public Text instructionLabel;

    public Text scoreText; // UI 得点を表示するためのテキスト要素
    private int initialSpeed; // 初速
    private int initialSpeedFromCase2; // ケース2で得られた初速
    private int crowCountFromCase2; // ケース2で得られたカラスの数
    private int initialSpeedFromCase6; // ケース6で得られた初速
    private int crowCountFromCase6; // ケース6で得られたカラスの数
    public Text thankYouText;// UI "Thank you for playing "を表示するためのテキスト要素。

    // Start is called before the first frame update
    void Awake()
    {
        gameSceneState = 0;
        //シーン遷移用変数 0:スタート,１：待機a, 2:飛び立ちa、3：帰還a、４：結果a,５：待機b、６：飛び立ちb、７：帰還b、8:結果b, 9:終了処理
        callOnceFlag = false; //１シーンに一回呼び出すときに使う変数,trueなら呼び出し済み
        sceneTransitionFlag = false;//デバッグ用フラグ

        //_uiDispalyaa=UI,Getcomponent<UIdisplayer>();
        // 
        titleLabel = GameObject.Find("TitleLabel").GetComponent<Text>();
        instructionLabel = GameObject.Find("InstructionLabel").GetComponent<Text>();

        // 
        titleLabel.gameObject.SetActive(false);
        instructionLabel.gameObject.SetActive(false);
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
                    titleLabel.gameObject.SetActive(true);
                    instructionLabel.gameObject.SetActive(true);

                    titleLabel.text = "鷹匠"; //タイトル
                    instructionLabel.text = "Enterキーを押してゲームを開始"; // プロンプトテキストの設定

                    callOnceFlag = true;
                }

                if (Input.GetKeyDown(KeyCode.Return) || /* 腕の振りを検出?*/)
                {
                    callOnceFlag = false;
                    sceneTransitionFlag = false;

                    // ヘッダーとアラートを隠す
                    titleLabel.gameObject.SetActive(false);
                    instructionLabel.gameObject.SetActive(false);

                    gameSceneState = 1;
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

                if (Input.GetKeyDown(KeyCode.Return)) // 腕の振りを検出?
                {
                    // Enterキーが押されたら、次のゲーム状態
                    callOnceFlag = false; // 
                    gameSceneState = 2; // 
                    break;
            // 飛び立ちa
            case 2:
            case 2:
                // 飛び立ちa（Case 2）
                if (callOnceFlag == false)
                {
                    
                    Debug.Log("飛び立ちA");
                    callOnceFlag = true;

                  
                    Vector3 targetPosition = GetTargetPosition(); // 目標位置の取得
                    float flightSpeed = GetFlightSpeed(); // 飛行速度を得る

                    initialSpeedFromCase2 = flightSpeed;

                    // Eagle_EditコンポーネントとEagle_Navigationコンポーネントの取得
                    Eagle_Edit eagleEdit = GetComponent<Eagle_Edit>();
                    Eagle_Navigation eagleNavigation = GetComponent<Eagle_Navigation>();

                    if (eagleEdit != null && eagleNavigation != null)
                    {
                        // Eagle_Editスクリプトを使用したターゲットの場所の設定
                        eagleEdit.SetEagleState(Eagle_Edit.EagleState.Takeoff);

                        // フライトの開始
                        eagleEdit.TakeOff(targetPosition);

                        // Eagle_Navigationで目標位置と飛行速度を設定する
                        eagleNavigation.SetTarget(targetPosition);
                        eagleNavigation.SetSpeed(flightSpeed);
                    }
                    crowCountFromCase2 = 38;//例を挙げる
                }

                initialSpeedFromCase2 = CalculateInitialSpeed();
                crowCountFromCase2 = CalculateCrowCount();

                if (sceneTransitionFlag == true)
                {
                    callOnceFlag = false;
                    sceneTransitionFlag = false;
                    gameSceneState = 3;
                }

                break;
            // 帰還a
            case 3:
                // 帰還a（Case 3）
                if (callOnceFlag == false)
                {
                    Debug.Log("帰還A");
                    callOnceFlag = true;

                    // トラッカーから提供されたプレーヤーの手の位置を取得する。
                    Vector3 playerHandPosition = GetPlayerHandPosition();

                    // Eagle_EditコンポーネントとEagle_Navigationコンポーネントの取得
                    Eagle_Edit eagleEdit = GetComponent<Eagle_Edit>();
                    Eagle_Navigation eagleNavigation = GetComponent<Eagle_Navigation>();

                    if (eagleEdit != null && eagleNavigation != null)
                    {
                        // Eagle_Editスクリプトを使用したターゲットの場所の設定
                        eagleEdit.SetEagleState(Eagle_Edit.EagleState.Takeoff);

                        // イーグルの目標位置をプレーヤーの手の位置に設定する。
                        eagleNavigation.SetTarget(playerHandPosition);

                        // 必要に応じて飛行速度を設定する
                        eagleNavigation.SetSpeed(yourSpeedValue);
                    }
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
                if (callOnceFlag == false)
                {

                    Debug.Log("结果A");
                    callOnceFlag = true;

                    // ケース2で計算した初速とカラスの数を用いてスコアを計算する
                    int finalScore = initialSpeedFromCase2 + crowCountFromCase2;

                    // UI Text要素にスコアを表示する
                    if (scoreText != null)
                    {
                        scoreText.text = "スコア: " + finalScore.ToString();
                    }
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    callOnceFlag = false;
                    gameSceneState = 5;
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
                    break;
            // 飛び立ちb
            case 6:
                // 飛び立ちa（Case 2）的处理
                if (callOnceFlag == false)
                {

                    Debug.Log("飛び立ちB");
                    callOnceFlag = true;


                    Vector3 targetPosition = GetTargetPosition(); // 目標位置の取得
                    float flightSpeed = GetFlightSpeed(); // 飛行速度を得る

                    initialSpeedFromCase6 = flightSpeed;

                    // Eagle_EditコンポーネントとEagle_Navigationコンポーネントの取得
                    Eagle_Edit eagleEdit = GetComponent<Eagle_Edit>();
                    Eagle_Navigation eagleNavigation = GetComponent<Eagle_Navigation>();

                    if (eagleEdit != null && eagleNavigation != null)
                    {
                        // Eagle_Editスクリプトを使用したターゲットの場所の設定
                        eagleEdit.SetEagleState(Eagle_Edit.EagleState.Takeoff);

                        // フライトの開始
                        eagleEdit.TakeOff(targetPosition);

                        // Eagle_Navigationで目標位置と飛行速度を設定する
                        eagleNavigation.SetTarget(targetPosition);
                        eagleNavigation.SetSpeed(flightSpeed);
                    }
                    crowCountFromCase6 = 39;//例を挙げる
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
                // 帰還a（Case 7）
                if (callOnceFlag == false)
                {
                    Debug.Log("帰還B");
                    callOnceFlag = true;

                    // トラッカーから提供されたプレーヤーの手の位置を取得する。
                    Vector3 playerHandPosition = GetPlayerHandPosition();

                    // Eagle_EditコンポーネントとEagle_Navigationコンポーネントの取得
                    Eagle_Edit eagleEdit = GetComponent<Eagle_Edit>();
                    Eagle_Navigation eagleNavigation = GetComponent<Eagle_Navigation>();

                    if (eagleEdit != null && eagleNavigation != null)
                    {
                        // Eagle_Editスクリプトを使用したターゲットの場所の設定
                        eagleEdit.SetEagleState(Eagle_Edit.EagleState.Takeoff);

                        // イーグルの目標位置をプレーヤーの手の位置に設定する。
                        eagleNavigation.SetTarget(playerHandPosition);

                        // 必要に応じて飛行速度を設定する
                        eagleNavigation.SetSpeed(yourSpeedValue);
                    }
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
                if (callOnceFlag == false)
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

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    callOnceFlag = false;
                    gameSceneState = 9;
                    break;
            // 終了処理
            case 9:
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
}
