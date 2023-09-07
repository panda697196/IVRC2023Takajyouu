using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class ArmAngle_v2 : MonoBehaviour
{
    public GameManager gameManager;

    public FlyArmReadyDetection _flyArmReadyDetection;
    [SerializeField] private ArmCollisionDetection _armCollisionDetection; 
    
    public Transform trackerWaist; // トラッカー1のTransformコンポーネント

    public float span = 0.01f;

    public Transform goalPosition;
    public Transform tmpGoalPosObj;

    private Vector3 _prevPositionWaist;
    private float angle;

    private bool _isFirstReadyOfArm;
    [SerializeField] private bool _isFirstReadyOfArmForFlyFlag;

    private int _sceneTarans;

    private float _delta = 0;

    public bool flyFlag = false;　//飛び立ちフラグ
    
    //改良版製作用変数
    [SerializeField] private float DeleyTime = 1.0f; //フライフラグをだす遅延時間

    //改良版2
    [SerializeField] private float TrackerSpeed;
    private List<float> _queueArray = new List<float>();
    [SerializeField] private int _listMaxSize = 5;//キューの最大サイズ
    [SerializeField] private float BaseSpeed = 0.8f;//基準となるスピード
    [SerializeField] private float _averageSpeed;//トラッカーの平均変化量


    private void Start()
    {
        _sceneTarans = gameManager.GetComponent<GameManager>().GetgameSceneState(); //scene遷移の初期化
        _prevPositionWaist = trackerWaist.position;//trackerの過去の位置初期化
        _isFirstReadyOfArmForFlyFlag = false;//_isFirstReadyOfArmForFlyFlag
    }

    private void Update()
    {
        _isFirstReadyOfArm = _flyArmReadyDetection.GetIsFirstReadyOfArm();//コライダーと腕との接触位置判定をする
        
        _sceneTarans = gameManager.GetComponent<GameManager>().GetgameSceneState();//シーンの遷移を取得する
        
        this.Set_isFirstReadyOfArmForFlyFlag(); //flyflagように腕の接触を保存する

        // Debug.Log("Pre;"+prevPosition1+"now"+tracker1.position); //Debug用
        // Debug.Log(_sceneTarans); //Debug用シーン遷移の確認
        
        if (_delta > span)//spanの時間間隔
        {
            Vector3 currentPosition1 = trackerWaist.position;// トラッカーの現在の位置情報を取得
            
            Vector3 positionDiff1 = (currentPosition1 - _prevPositionWaist) / span;// 前フレームからの位置変化を計算

            TrackerSpeed = positionDiff1.magnitude;
            
            // Debug.Log(TrackerSpeed); //debug:trackerのスピード
            
            Enqueue(TrackerSpeed);//トラッカーの差分をインキュー(Maxを超えるとデキュー)
            // Debug.Log("きゅーのかず："+queueArray.Count);//debug:現在のキューの数

            _averageSpeed = GetAverage();
            Debug.Log("平均スピード："+_averageSpeed);//Debug:平均スピード

            //条件が揃ったらフライフラグを立て,ゴールをセットする
            if (_averageSpeed >= BaseSpeed && (_sceneTarans == 2 || _sceneTarans == 5) && _isFirstReadyOfArmForFlyFlag == true)
            {
                flyFlag = true;//フライフラグを立てる
                Invoke(nameof(SetGoal), DeleyTime);//deley後にゴールをセットする
                _isFirstReadyOfArmForFlyFlag = false;//腕位置検知のフラグを戻す
            }

            // 現在の位置情報を保存
            _prevPositionWaist = currentPosition1;

            this._delta = 0; //deltaの初期化
        }
        
        _delta += Time.deltaTime;
    }
    
    public bool GetIsFirstReadyOfArm()//飛ばす準備ができた
    {
        return _isFirstReadyOfArm;
    }

    public bool GetFlyFlag()//フライフラグの取得
    {
        return flyFlag;
    }

    public void RsetFlyFlag() //フライフラグを初期化
    {
        flyFlag = false;
    }
    
    private void SetGoal()//鷹のターゲットの位置をセットする
    {
        gameManager.SetEagleTarget(tmpGoalPosObj.position);
        Debug.Log("一時ゴールの位置："+tmpGoalPosObj.position);
    }
    
    
    private void Enqueue(float item)//キューに追加する（インキュー相当）
    {
        // Debug.Log("インキュー:"+item);
        _queueArray.Add(item);

        // キューの最大サイズを超えた場合、デキューを実行
        if (_queueArray.Count > _listMaxSize)
        {
            Dequeue();
        }
    }

    // 先頭の要素を取り出す（Dequeue相当）
    private float Dequeue()
    {
        // Debug.Log("deキュー:");
        if (_queueArray.Count == 0)
        {
            Debug.LogWarning("キューが空です。");
            return -1; // エラー値またはデフォルト値を返す
        }

        var item = _queueArray[0];
        _queueArray.RemoveAt(0);
        return item;
    }
    
    //_isFirstReadyOfArmForFlyFlagをセットする
    private void Set_isFirstReadyOfArmForFlyFlag()
    {
        if ((_sceneTarans == 2 || _sceneTarans == 5) && _isFirstReadyOfArm == true)
        {
            _isFirstReadyOfArmForFlyFlag = true;
        }
    }
    
    //キューの指定サイズをもとに平均をだす
    private float GetAverage()
    {
        float average = 0.0f;
        if (_queueArray.Count >= _listMaxSize)//キューに格納されている値がMaxSizeで分あれば平均を出す
        {
            float sum = 0.0f;//キューの中の値の合計値
                
            for (int i = 0; i < _listMaxSize; i++)//合計を求めるループ
            {
                sum += _queueArray[i];
            }

            average = sum / _listMaxSize;//合計の計算
            // Debug.Log("Average: " + average);//Debug:平均スピードの計算
        }

        return average;
    }
    
    //仮のゴールをセットする
    private void SetPlaceholderGoal(Vector3 tmpGoalPos)
    {
        gameManager.SetEagleTarget(tmpGoalPos);
    }
    

}

