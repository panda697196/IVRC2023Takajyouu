using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*注意事項(まずはここを確認する)
     *
     * シーン遷移のフラグとなる部分が未実装のものはすべてEnterキーを押すと次のシーンに遷移する用に作ってあります
     * 
     */


    [SerializeField] private int gameSceneState;//ゲームシーン遷移を管理する変数
    //0:スタート　,1:鷹到着シーン　2：待機a, 3:飛び立ちa、4：帰還a、５：待機b、６：飛び立ちb、７：帰還b、8:結果b, 9:終了処理

    private bool callOnceFlag; //１シーンに一回呼び出すときに使う変数,trueなら呼び出し済み

    [SerializeField] private TextMeshProUGUI titleLabel; //タイトル画面
    [SerializeField] private TextMeshProUGUI instructionLabel; //instructionUI
    [SerializeField] private TextMeshPro scoreText; // UI 得点を表示するためのテキスト要素
    [SerializeField] private TextMeshProUGUI thankYouText;// UI "Thank you for playing "を表示するためのテキスト要素。
    
    // [SerializeField] private Eagle_Edit eagleEdit;//鷹の状態用スクリプト
    // [SerializeField] private Eagle_Navigation eagleNavigation;//鷹の移動を管理するスクリプト
    [SerializeField] private EagleManager eagleManager;//鷹の移動を管理するスクリプト
    [SerializeField] private ArmAngle flyFlagObj;//飛び立ちフラグを管理するスクリプト
    [SerializeField] private HardwareManager _hardwareManager; //ハードウェア班からのスクリプト
    [SerializeField] private Transform rawfingerPos;//左手の親指の位置
    [SerializeField] private GameObject eagleTarget;//鷹の飛行すべき目標位置
    
    [SerializeField] private bool hardwareFlag; //ハードウェアと鷹との連携に使用する
    [SerializeField] private bool eagleWaitFlag; //ハードウェアと鷹との連携に使用する
    [SerializeField] private int gameScore; //スコアを入れる変数
    [SerializeField] private int crowCountFromCase2; // ケース2で追い払ったカラスの数
    [SerializeField] private int crowCountFromCase6; // ケース6で追い払ったカラスの数
    [SerializeField] private bool playerReady; //プレイヤーが準備完了したかどうかのフラグ
    [SerializeField] private bool eagleGetOnArm;//鷹がうでにとまった状態であるかどうかのフラグ
    [SerializeField] private bool huntFinFlag; //飛び時ハードウェアと鷹の準備が完了したかどうかのフラグ
    [SerializeField] private bool eargleHasScorebord;//スコアボードを鷹が持ったかどうか
    [SerializeField] private bool putScorebord;//スコアボードを地面に置いたことを判定する
    

    
    // private int initialSpeed; // 初速（現状不要）
    // private int initialSpeedFromCase2; // ケース2で得られた初速（現状不要）
    // private int initialSpeedFromCase6; // ケース6で得られた初速（現状不要）
    [SerializeField] private bool IsFirstReadyOfPlayerArm;
    [SerializeField] private bool flyFlag; //飛び立ちフラグ
    private Vector3 fingerPos;//左手の親指の座標（transform.position）
    private Vector3 eagleTargetPos;//左手の親指の座標（transform.position）

    [SerializeField] private bool _withHardware = false;//ハードウェアを使わずにデバッグしたい場合はこれを切ってください


    void Awake()
    {
        if (_withHardware)
        {
            _hardwareManager.NotUseHardware();
        }
        gameSceneState = 0; //シーン遷移用変数の初期化
        gameScore = 0; //ゲームスコアの初期化
        
        callOnceFlag = false; //一回呼び出し用フラグの初期化

        hardwareFlag = false;//ハードウェアフラグ初期化
        eagleWaitFlag = false;//鷹待機フラグ初期化
        playerReady = false;//プレイヤー準備フラグの初期化
        eagleGetOnArm = false;//鷹がうでに止まったかのフラグ初期化
        putScorebord = false;//スコアボードを地面に置くフラグを初期化
        eargleHasScorebord = false;//スコアボードを鷹が持ったかの判定フラグの初期化
        
        titleLabel.gameObject.SetActive(false);// titleLabelの初期化
        instructionLabel.gameObject.SetActive(false);//インストラクションの初期化
        
        flyFlag = flyFlagObj.GetFlyFlag();//flyFlagの取得かつ初期化

        fingerPos = rawfingerPos.position;//左手の座標の取得かつ初期化

        // eagleTargetPos = eagleTarget.position;//鷹のターゲットの位置を初期化
    }

    void Update()
    {
        fingerPos = rawfingerPos.position;//左手の座標の取得
        
        
        switch (gameSceneState)
        {
            case 0:// スタートシーン(case0)
                // --------------------スタートシーンでの処理内容(毎フレーム)------------------------------------------------
                
                //player = true//プレイヤーが準備できかを監視
                
                //----------------------------------------------------------------------------------------------------
                
                
                if (callOnceFlag == false) //一回だけ実行(if文の中)
                {
                    titleLabel.gameObject.SetActive(true);
                    instructionLabel.gameObject.SetActive(true);

                    titleLabel.text = "Takasyo"; //タイトル
                    instructionLabel.text = "Please press enter key"; // プロンプトテキストの設定

                    callOnceFlag = true;
                }

                if (Input.GetKeyDown(KeyCode.Return) || playerReady == true) //シーン遷移処理（プレイヤーの準備が完了すると）
                {
                    callOnceFlag = false; //一回フラグの初期化

                    // ヘッダーとアラートを隠す
                    titleLabel.gameObject.SetActive(false);
                    instructionLabel.gameObject.SetActive(false);

                    gameSceneState = 1;//鷹到着シーンへ（case1）
                }
                break;
            case 1://鷹到着シーン
                // --------------------------------鷹到着シーンでの処理内容(毎フレーム)--------------------------------------------
                //鷹がうでにとまったかどうか監視するスクリプト
                
                //----------------------------------------------------------------------------------------------------------------
                
                
                if (callOnceFlag == false) //一回だけ実行(if文の中)
                {
                    
                    
                    callOnceFlag = true;
                    
                    
                }
                
                

                if (Input.GetKeyDown(KeyCode.Return) || eagleGetOnArm == true) //シーン遷移処理（鷹がうでに止まる）
                {
                    callOnceFlag = false; //一回フラグの初期化
                    
                    gameSceneState = 2;//待機シーン（case2）へ
                }
                break;
            case 2:// 待機a
                // ---------------------------待機aでの処理内容(毎フレーム)---------------------------------------------------------
                IsFirstReadyOfPlayerArm = flyFlagObj.GetIsFirstReadyOfArm(); //うでの最初の準備完了状態の監視
                flyFlag = flyFlagObj.GetFlyFlag(); //うでの振り速度の閾値越えの監視
                Debug.Log("flyflag:"+flyFlag);


                //-------------------------------------------------------------------------------------------------------------
                if (callOnceFlag == false)//1回だけの処理
                {
                    Debug.Log("待機A");
                    callOnceFlag = true;
                }

                // if (Input.GetKeyDown(KeyCode.Return)) // 腕の振りを検出
                if (Input.GetKeyDown(KeyCode.Return) || flyFlag == true) // シーン遷移処理（腕の振りを検出）
                {
                    flyFlagObj.flyFlag = false; //flyFlag初期化
                    flyFlag = false; //GameManagerのフライフラフ初期化
                    callOnceFlag = false; // 1回フラグの初期化
                    gameSceneState = 3; // 飛び立ちシーン（case3）へ
                }
                break;
            case 3:// 飛び立ちaシーン
               
                if (callOnceFlag == false)//1回だけの処理
                {
                    
                    Debug.Log("飛び立ちA");
                    callOnceFlag = true;

                    eagleManager.EagleTarget2Around(eagleTarget);


                    // Vector3 targetPosition = GetTargetPosition(); // 目標位置の取得
                    //float flightSpeed = GetFlightSpeed(); // 飛行速度を得る（現状不要）

                    //initialSpeedFromCase2 = flightSpeed;



                    // if (eagleEdit != null && eagleNavigation != null)
                    // {
                    //     // Eagle_Editスクリプトを使用したターゲットの場所の設定
                    //     eagleEdit.SetEagleState(Eagle_Edit.EagleState.Takeoff);
                    //
                    //     // フライトの開始
                    //    // eagleEdit.TakeOff(targetPosition);
                    //    eagleGetOnArm = false;
                    //
                    //    // Eagle_Navigationで目標位置と飛行速度を設定する
                    //    // eagleNavigation.SetTarget(targetPosition);
                    //    // eagleNavigation.SetSpeed(flightSpeed);
                    // }
                    //カラス？？
                    // crowCountFromCase2 = 38;//殺したカラス
                }
                // -----------------------------------飛び立ちaシーンにおける処理(毎フレーム)-----------------------------------------------------
                
                //hardwareFlag = hoge.flag://ハードウェアからのフラグを監視
                // eagleWaitFlag = taka.flag;//鷹からの待機状態のフラグを監視

                // if (hardwareFlag == true && eagleWaitFlag == true)
                // {
                //     huntFinFlag = true;//ハードと鷹が帰還準備完了であることを記録
                // }
                
                eagleManager.EagleAround2GetOn(hardwareFlag);
                
                
                //-----------------------------------------------------------------------------------------------------
               


                if (Input.GetKeyDown(KeyCode.Return) || hardwareFlag == true)//シーン遷移（ハードの準備と鷹の準備ができたら）
                {
                    callOnceFlag = false;
                    huntFinFlag = false;//初期化
                    hardwareFlag = false;
                    gameSceneState = 4;
                }

                break;
            case 4:// 一回目の帰還シーン（帰還a）(case4)
                // -----------------------------------一回目の帰還における処理(毎フレーム)-----------------------------------------------------
                
                //鷹がうでに止まったかどうかを監視
                eagleGetOnArm = eagleManager.EagleOnHand();
                
                
                
                //-----------------------------------------------------------------------------------------------------
                if (callOnceFlag == false)//１回だけの処理
                {
                    Debug.Log("帰還A");
                    callOnceFlag = true;

                    // トラッカーから提供されたプレーヤーの手の位置を取得する。

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


                if (Input.GetKeyDown(KeyCode.Return) ||　eagleGetOnArm == true)//シーン遷移(鷹がうでに止まる)
                {
                    callOnceFlag = false;
                    gameSceneState = 5;//二回目の待機シーンへ
                }
                break;
            case 5://二回目の待機シーン（待機b）(case５)

                // -------------------------------二回目の待機での処理内容(毎フレーム)-----------------------------------------------------
                IsFirstReadyOfPlayerArm = flyFlagObj.GetIsFirstReadyOfArm(); //うでの最初の準備完了状態の監視
                flyFlag = flyFlagObj.GetFlyFlag(); //うでの振り速度の閾値越えの監視
                Debug.Log("flyflag:"+flyFlag);


                //-------------------------------------------------------------------------------------------------------------
                if (callOnceFlag == false)//１回だけの処理
                {
                    Debug.Log("待機B");
                    callOnceFlag = true;
                }

                if (Input.GetKeyDown(KeyCode.Return)　|| flyFlag == true) // シーン遷移（うでの振り検知）
                {
                    callOnceFlag = false; // 一回だけフラグの初期化
                    
                    flyFlagObj.flyFlag = false; //flyFlag初期化
                    flyFlag = false; //GameManagerのフライフラフ初期化


                    gameSceneState = 6; // 二回目の飛び立ちシーンへ
                }
                break;
                
            case 6: //2回目の飛び立ちシーン処理(飛び立ちb)(case6)
                //---------------------------------------------------2回目の飛び立ちシーンの処理-----------------------------------------------------------------------------------
                
                // eagleWaitFlag = taka.flag;//鷹からの待機状態のフラグを監視
                //鷹がスコアボードを持ったか判定
                //if hogehoge eagleHasScorebord = true;


                //------------------------------------------------------------------------------------------------------------------------
                if (callOnceFlag == false)//一回だけの処理
                {
                    Debug.Log("飛び立ちB");
                    callOnceFlag = true;
                    
                    eagleManager.EagleTarget2Around(eagleTarget);



                   // Vector3 targetPosition = GetTargetPosition(); // 目標位置の取得
                    //float flightSpeed = GetFlightSpeed(); // 飛行速度を得る

                    //initialSpeedFromCase6 = flightSpeed;

                    // Eagle_EditコンポーネントとEagle_Navigationコンポーネントの取得
                    

                    
                }

                if (Input.GetKeyDown(KeyCode.Return) || eargleHasScorebord == true)//シーン遷移処理(鷹がカラスを追い払い終わり、スコアボードを持ったら？)
                {
                    callOnceFlag = false;
                    gameSceneState = 7;
                }
                break;
            case 7:// 2回目の帰還シーン（スコアボードを持ってくる鷹）(帰還b)(case2)
                //---------------------------------帰還b（Case 7）毎フレームの処理-----------------------------------------------------
                
                // putScorebord = true;//スコアボードを地面に置いたかどうかを監視
                
                //---------------------------------------------------------------------------------------------------------------
                
                if (callOnceFlag == false)//1回だけの処理
                {
                    Debug.Log("帰還B");
                    callOnceFlag = true;

                    // トラッカーから提供されたプレーヤーの手の位置を取得する。
                    //Vector3 playerHandPosition = GetPlayerHandPosition();

                    // if (eagleEdit != null && eagleNavigation != null)
                    // {
                    //     // Eagle_Editスクリプトを使用したターゲットの場所の設定
                    //     eagleEdit.SetEagleState(Eagle_Edit.EagleState.Takeoff);
                    //
                    //     // イーグルの目標位置をプレーヤーの手の位置に設定する。
                    //    // eagleNavigation.SetTarget(playerHandPosition);
                    //
                    //     // 必要に応じて飛行速度を設定する
                    //    // eagleNavigation.SetSpeed(yourSpeedValue);
                    // }
                }


                if (Input.GetKeyDown(KeyCode.Return) || putScorebord == true)//シーン遷移（鷹がスコアボードを落としたら）
                {
                    callOnceFlag = false;
                    gameSceneState = 8;
                }


                break;
            case 8://スコア表示シーン(case8)
                //--------スコア表示シーンの処理(毎フレーム）-------------------------------------------------------------------
                
                //特にないかな？
                
                
                //-----------------------------------------------------------------------------------------------------
                if (callOnceFlag == false)//一回だけの処理
                {

                    Debug.Log("结果B");
                    callOnceFlag = true;

                    // ケース6で計算した初速とカラスの数を用いてスコアを計算する
                    // int finalScore = initialSpeedFromCase6 + crowCountFromCase6;（現状不要）

                    // UI Text要素にスコアを表示する（べつスクリプトで行う可能性あり）
                    if (scoreText != null)
                    {
                        // scoreText.text = "スコア: " + finalScore.ToString();
                    }
                }

                if (Input.GetKeyDown(KeyCode.Return))//シーンの移行処理(EnterKeyを押す)
                {
                    callOnceFlag = false;
                    gameSceneState = 9;
                 
                }
                break;
            case 9:// 終了処理(case9)
                //------------------------------ 終了処理（毎フレーム）--------------------------------------------------------------
                
                
                //-------------------------------------------------------------------------------------------------------------
                if (callOnceFlag == false)
                {
                    // 
                    Debug.Log("終了処理");
                    callOnceFlag = true;

                    // 
                    // if (thankYouText != null)
                    // {
                    //     thankYouText.text = "Thank you for playing";
                    // }

                    UnityEditor.EditorApplication.isPlaying = false;//Unityの実行を終了する

                }
                
                // if (Input.GetKeyDown(KeyCode.Return))//終了処理(Enterキーを押す)
                // {
                //     callOnceFlag = false; // 
                //     gameSceneState = 0; // 
                // }
                break;
        }
        
        
    }
    
    public int GetgameSceneState()//ゲームシーン遷移を取得
    {
        return gameSceneState;
    }

    public bool GetflyFlag()//フライフラグの取得
    {
        return flyFlag;
    }

    public bool GethardwareFlag()//ハードウェアからのフラグを取得
    {
        return hardwareFlag;
    }

    public bool GeteagleWaitFlag()//鷹待機のフラグを取得
    {
        return eagleWaitFlag;
    }

    public int GetgameScore()//ゲームのスコアを取得
    {
        return gameScore;
    }

    public int GetcrowCountFromCase2()//一回目のカラスの撃退数を取得
    {
        return crowCountFromCase2;
    }

    public int GetcrowCountFromCase6()//二回目のカラスの撃退数を取得
    {
        return crowCountFromCase6;
    }

    public Vector3 GeteagleTargetPos()//カラス目標とするターゲットの座標を取得
    {
        return eagleTargetPos;
    }

    public Vector3 GetFingerPos() //鷹が止まる場所の位置座標の取得
    {
        return fingerPos;
    }
}
