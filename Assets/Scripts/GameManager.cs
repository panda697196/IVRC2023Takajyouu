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


    public  bool _isEagleGetOnArm;//鷹がうでにとまった状態であるかどうかのフラグ

    [SerializeField] private int gameSceneState;//ゲームシーン遷移を管理する変数
    //0:スタート　,1:鷹到着シーン　2：待機a, 3:飛び立ちa、4：帰還a、５：待機b、６：飛び立ちb、７：帰還b、8:結果b, 9:終了処理

    private bool callOnceFlag; //１シーンに一回呼び出すときに使う変数,trueなら呼び出し済み
    
    // [SerializeField] private Eagle_Edit eagleEdit;//鷹の状態用スクリプト
    // [SerializeField] private Eagle_Navigation eagleNavigation;//鷹の移動を管理するスクリプト
    [SerializeField] private EagleManager eagleManager;//鷹の移動を管理するスクリプト
    [SerializeField] private ArmAngle_v2 _armAngle_v2;//飛び立ちフラグを管理するスクリプト
    [SerializeField] private HardwareManager _hardwareManager; //ハードウェア班からのスクリプト
    [SerializeField] private ScoreReceiver _scoreReceiver;
    [SerializeField] private Transform rawfingerPos;//左手の親指の位置
    [SerializeField] private GameObject _eagleTarget;//鷹の飛行すべき目標位置
    
    [SerializeField] private bool hardwareFlag; //ハードウェアと鷹との連携に使用する
    [SerializeField] private bool eagleWaitFlag; //ハードウェアと鷹との連携に使用する
    [SerializeField] private int gameScore; //スコアを入れる変数
    [SerializeField] private int _crowCount1stTry; // ケース2で追い払ったカラスの数
    [SerializeField] private int _crowCount2ndTry; // ケース6で追い払ったカラスの数
    [SerializeField] private bool huntFinFlag; //飛び時ハードウェアと鷹の準備が完了したかどうかのフラグ
    [SerializeField] private bool eargleHasScorebord;//スコアボードを鷹が持ったかどうか
    [SerializeField] private bool putScorebord;//スコアボードを地面に置いたことを判定する
    

    
    // private int initialSpeed; // 初速（現状不要）
    // private int initialSpeedFromCase2; // ケース2で得られた初速（現状不要）
    // private int initialSpeedFromCase6; // ケース6で得られた初速（現状不要）
    [SerializeField] private bool _isFirstReadyOfPlayerArm;
    [SerializeField] private bool flyFlag; //飛び立ちフラグ
    private Vector3 fingerPos;//左手の親指の座標（transform.position）
    private Vector3 eagleTargetPos;//左手の親指の座標（transform.position）
    private float _secToCome;
    private bool _isReadyToPopCrow = false;
    private bool _isArounding = false;

    [SerializeField] private bool _isUseHardware = false;//ハードウェアを使わずにデバッグしたい場合はこれを切ってください


    void Awake()
    {
        if (_isUseHardware)
        {
            _hardwareManager.NotUseHardware();
        }
        gameSceneState = 0; //シーン遷移用変数の初期化
        gameScore = 0; //ゲームスコアの初期化
        
        callOnceFlag = false; //一回呼び出し用フラグの初期化

        hardwareFlag = false;//ハードウェアフラグ初期化
        eagleWaitFlag = false;//鷹待機フラグ初期化
        _isEagleGetOnArm = false;//鷹がうでに止まったかのフラグ初期化
        putScorebord = false;//スコアボードを地面に置くフラグを初期化
        eargleHasScorebord = false;//スコアボードを鷹が持ったかの判定フラグの初期化
        _isReadyToPopCrow = false; 
        _isArounding = false;


        flyFlag = _armAngle_v2.GetFlyFlag();//flyFlagの取得かつ初期化

        fingerPos = rawfingerPos.position;//左手の座標の取得かつ初期化

        // eagleTargetPos = eagleTarget.position;//鷹のターゲットの位置を初期化
    }

    void Update()
    {
        fingerPos = rawfingerPos.position;//左手の座標の取得
        
        
        switch (gameSceneState)
        {
            case 0:// スタートシーン(case0)腕を構えるまで
                // --------------------スタートシーンでの処理内容(毎フレーム)------------------------------------------------
                
                //player = true//プレイヤーが準備できかを監視
                
                //----------------------------------------------------------------------------------------------------
                
                

                if (Input.GetKeyDown(KeyCode.Return)) //シーン遷移処理（プレイヤーの準備が完了すると）
                {
                    callOnceFlag = false; //一回フラグの初期化

                    gameSceneState = 1;//鷹到着シーンへ（case1）
                }
                break;

            case 1://鷹到着シーン
                // --------------------------------鷹到着シーンでの処理内容(毎フレーム)--------------------------------------------
                //鷹がうでにとまったかどうか監視するスクリプト
                
                //----------------------------------------------------------------------------------------------------------------
                
                
                if (callOnceFlag == false) //一回だけ実行(if文の中)
                {
                    _hardwareManager.StandbyComeHawk();//鷹がくる準備　この後に3secくらいは欲しい
                    //TODO:鷹が腕に留まるまで
                    callOnceFlag = true;
                }

                //SetComeHawkSecond にてHardware（ComeHawk（））を動かす

                if (Input.GetKeyDown(KeyCode.Return) || _isEagleGetOnArm == true) //シーン遷移処理（鷹がうでに止まる）
                {
                    callOnceFlag = false; //一回フラグの初期化
                    _isEagleGetOnArm = false;

                    gameSceneState = 2;//待機シーン（case2）へ
                }
                break;

            case 2:// 待機a
                // ---------------------------待機aでの処理内容(毎フレーム)---------------------------------------------------------
                //_isFirstReadyOfPlayerArm = flyFlagObj.GetIsFirstReadyOfArm(); //うでの最初の準備完了状態の監視
                flyFlag = _armAngle_v2.GetFlyFlag(); //うでの振り速度の閾値越えの監視
                Debug.Log("flyflag:"+flyFlag);


                //-------------------------------------------------------------------------------------------------------------
                if (callOnceFlag == false)//1回だけの処理
                {
                    Debug.Log("待機A");
                    callOnceFlag = true;

                    //TODO:カラス沸かせる1（少なめ，集中，固定）

                }

                // ReadyToPop()により次への動作を取得，_hardwareManager.StandbyDisappear()も処理した．


                // if (Input.GetKeyDown(KeyCode.Return)) // 腕の振りを検出
                if (Input.GetKeyDown(KeyCode.Return) || (flyFlag == true  && _isReadyToPopCrow)) // シーン遷移処理（腕の振りを検出）
                {
                    //飛び立つ瞬間（飛んでない）
                    _hardwareManager.Disappear(); //飛び立ち刺激　本当は飛び立つ0.5secに送りたい　←よさそう
                    //TODO:TakeOffアニメーション もしくは 適当なターゲットに飛ぶ（ソフトウェア）
                    //_eagleTarget = （仮のターゲット指定）
                    // eagleManager.EagleTarget2Around(_eagleTarget);

                    _armAngle_v2.flyFlag = false; //flyFlag初期化
                    flyFlag = false; //GameManagerのフライフラフ初期化
                    callOnceFlag = false; // 1回フラグの初期化
                    gameSceneState = 3; // 飛び立ちシーン（case3）へ
                    _isReadyToPopCrow = false;
                }
                break;

            case 3:// 飛び立ちaシーン
               
                if (callOnceFlag == false)//1回だけの処理
                {
                     //飛び立った直後
                    Debug.Log("飛び立ちA");
                    callOnceFlag = true;



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
                }
                // -----------------------------------飛び立ちaシーンにおける処理(毎フレーム)-----------------------------------------------------
                
                //ハードウェアからのフラグを監視 - すでにSetHardwareFlagで実装済　消してもいい
               

                //アニメーションからのStandbyGoHome()によりUI表示

                if (_isArounding)//TODO:旋回中　腕を固定したかの判定
                {
                    //TODO:腕を固定していたら
                    _hardwareManager.StandbyComeHawk();//UIを消す？
                }
                
                
                //-----------------------------------------------------------------------------------------------------
               


                if (Input.GetKeyDown(KeyCode.Return) || hardwareFlag == true)//シーン遷移（ハードの準備と鷹の準備ができたら）
                {
                    eagleManager.EagleAround2GetOn(hardwareFlag);//関数にしてもいいよ

                    callOnceFlag = false;
                    huntFinFlag = false;//初期化
                    hardwareFlag = false;
                    gameSceneState = 4;
                    _isArounding = false;
                }
                break;

            case 4:// 一回目の帰還シーン（帰還a）(case4)
                // -----------------------------------一回目の帰還における処理(毎フレーム)-----------------------------------------------------
                
                
                //-----------------------------------------------------------------------------------------------------
                if (callOnceFlag == false)//１回だけの処理
                {
                    Debug.Log("帰還A");
                    callOnceFlag = true;

                }

                //SetComeHawkSecond にてHardware（ComeHawk（））を動かす

                if (Input.GetKeyDown(KeyCode.Return) || _isEagleGetOnArm == true)//シーン遷移(鷹がうでに止まる)
                {
                    callOnceFlag = false;
                    _isEagleGetOnArm = false;
                    gameSceneState = 5;//二回目の待機シーンへ
                }
                break;

            case 5://二回目の待機シーン（待機b）(case５)

                // -------------------------------二回目の待機での処理内容(毎フレーム)-----------------------------------------------------
                flyFlag = _armAngle_v2.GetFlyFlag(); //うでの振り速度の閾値越えの監視
                Debug.Log("flyflag:"+flyFlag);


                //-------------------------------------------------------------------------------------------------------------
                if (callOnceFlag == false)//１回だけの処理
                {
                    Debug.Log("待機B");
                    callOnceFlag = true;

                    _crowCount1stTry = 38;//TODO:１回目のカラス取得

                    //TODO：スカイボックスを変更し，カラスを飛び立たせる
                    //TODO:カラス沸かせる２（多め，バラバラ）
                }

                // ReadyToPop()により次への動作を取得，_hardwareManager.StandbyDisappear()も処理した．


                // if (Input.GetKeyDown(KeyCode.Return)) // 腕の振りを検出
                if (Input.GetKeyDown(KeyCode.Return) || (flyFlag == true && _isReadyToPopCrow)) // シーン遷移処理（腕の振りを検出）
                {
                    //飛び立つ瞬間（飛んでない）
                    _hardwareManager.Disappear(); //飛び立ち刺激　本当は飛び立つ0.5secに送りたい　←よさそう
                    //TODO:TakeOffアニメーション もしくは 適当なターゲットに飛ぶ（ソフトウェア）
                    //_eagleTarget = （仮のターゲット指定）
                    // eagleManager.EagleTarget2Around(_eagleTarget);

                    _armAngle_v2.flyFlag = false; //flyFlag初期化
                    flyFlag = false; //GameManagerのフライフラフ初期化
                    callOnceFlag = false; // 1回フラグの初期化
                    gameSceneState = 6; // 飛び立ちシーン（case3）へ
                    _isReadyToPopCrow = false;
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
                }

                if (Input.GetKeyDown(KeyCode.Return) || _isArounding)//シーン遷移処理(鷹がカラスを追い払い終わり、旋回開始
                {
                    callOnceFlag = false;
                    gameSceneState = 7;
                }
                break;

            case 7://スコアボードを持ってくる鷹
                //---------------------------------帰還b（Case 7）毎フレームの処理-----------------------------------------------------
                
                // putScorebord = true;//スコアボードを地面に置いたかどうかを監視
                
                //---------------------------------------------------------------------------------------------------------------
                
                if (callOnceFlag == false)//1回だけの処理
                {
                    Debug.Log("帰還B");
                    callOnceFlag = true;

                    _crowCount2ndTry = 38;//TODO:２回目のカラス取得
                    //スコアの算出と伝達
                    _scoreReceiver.GetScore(_crowCount1stTry + _crowCount2ndTry);
                }

                //ゲームの終了


                /*
                if (Input.GetKeyDown(KeyCode.Return) || putScorebord == true)//シーン遷移（鷹がスコアボードを落としたら）
                {
                    callOnceFlag = false;
                    gameSceneState = 8;
                }
                */


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

    public Vector3 GeteagleTargetPos()//カラス目標とするターゲットの座標を取得
    {
        return eagleTargetPos;
    }

    public Vector3 GetFingerPos() //鷹が止まる場所の位置座標の取得
    {
        return fingerPos;
    }

    public void SetHardwareFlag (bool isHardwareStandby)
    {
        hardwareFlag = isHardwareStandby;
    }
    public void SetComeHawkSecond(float comeToSec)
    {
        _secToCome = comeToSec;
        _hardwareManager.ComeHawkToSecond(_secToCome);
    }
    public void ReadyToPop()
    {
        _isReadyToPopCrow = true;
        _hardwareManager.StandbyDisappear();
        //TODO:振りかぶるUI表示
    }
    public void SetEagleTarget(Vector3 target)
    {
        _eagleTarget.transform.position = target;
    }
    public void StandbyGoHome()
    {
        //帰還準備UI
        _isArounding = true;
    }
}
