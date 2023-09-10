using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Serialization;

public class ArmAngle_v2 : MonoBehaviour
{
    public GameManager gameManager;

    [SerializeField] private ArmCollisionDetection _armCollisionDetection;
    [SerializeField] private TargetChoicer _targetChoicer;
    
    public Transform trackerWaist; // トラッカー1のTransformコンポーネント
    public Transform _reference;

    public float span = 0.01f;

    [SerializeField] Transform _pointerPosition;

    private float _prevPositionWaist;
    private float angle;

    private bool _isPreReadyOfArm;
    private bool _isFirstReadyOfArm;
    [SerializeField] private bool _isFirstReadyOfArmForFlyFlag;
    [SerializeField] private float _armFlagContinuousTime = 0.2f;

    private int _scene;//シーンの遷移

    private bool _isMonitoringArm; //腕を監視するかどうか

    private float _delta = 0; //フレームのトータル時間(span秒ごとに初期化)

    public bool flyFlag = false;　//飛び立ちフラグ

    //改良版2
    [SerializeField] private float TrackerSpeed;
    private List<float> _queueArray = new List<float>();
    [SerializeField] private int _listMaxSize = 5;//キューの最大サイズ
    [SerializeField] private float BaseSpeed = 5f;//基準となるスピード
    [SerializeField] private float _averageSpeed;//トラッカーの平均変化量
    [SerializeField] private Collider _hmdSideCollider;
    
    [Header("仮のEagleTarget")]
    [SerializeField] private Vector3 _placeholderEagleTarget = new Vector3(0.0f,0.0f,0.0f); //仮のEagleTargetの位置座標


    private void Start()
    {
        _scene = gameManager.GetComponent<GameManager>().GetgameSceneState(); //scene遷移の初期化
        _prevPositionWaist = (_reference.position - trackerWaist.position).magnitude;//trackerと参照位置との過去の距離初期化
        _isFirstReadyOfArmForFlyFlag = false;//_isFirstReadyOfArmForFlyFlag
        _isPreReadyOfArm = false;
    }

    private void Update()
    {
        _scene = gameManager.GetComponent<GameManager>().GetgameSceneState();//シーンの遷移を取得する
        if (_scene == 2 || _scene == 5)
        {
            _isFirstReadyOfArm = _armCollisionDetection.isArmCllisionDetection(trackerWaist, _hmdSideCollider);//コライダーと腕との接触位置判定をする
                                                                                                               // Debug.Log("HMDSideColliderDetection:"+_isFirstReadyOfArm);//debug:HMDSideColliderの腕検知
            if (_isPreReadyOfArm && !_isFirstReadyOfArm)
            {
                StartCoroutine(LeaveReadyFlagTrue());
            }

            this.Set_isFirstReadyOfArmForFlyFlag(); //flyflag用に腕の接触を保存する

            // Debug.Log("Pre;"+prevPosition1+"now"+tracker1.position); //Debug用
            // Debug.Log(_sceneTarans); //Debug用シーン遷移の確認

            if (_delta > span)//spanの時間間隔
            {
                Vector3 currentVector1 = _reference.position - trackerWaist.position;// トラッカーから参照位置へのベクトルを算出
                float currentPosition1 = currentVector1.magnitude;
                //Debug.Log(currentPosition1);

                float TrackerSpeed = (_prevPositionWaist - currentPosition1) / span;// 前フレームからの距離変化を計算，距離が近くなれば値がプラスになるように引き算の順序を調整している

                // Debug.Log(TrackerSpeed); //debug:trackerのスピード

                Enqueue(TrackerSpeed);//トラッカーの差分をインキュー(Maxを超えるとデキュー)
                                      // Debug.Log("きゅーのかず："+queueArray.Count);//debug:現在のキューの数

                _averageSpeed = GetAverage();
                // Debug.Log("平均スピード："+_averageSpeed);//Debug:平均スピード

                //条件が揃ったらフライフラグを立て,ゴールをセットする
                if (_averageSpeed >= BaseSpeed && _isFirstReadyOfArmForFlyFlag == true && gameManager.GetIsReadyToPopCrow())
                {
                    flyFlag = true;//フライフラグを立てる
                    // Invoke(nameof(SetGoal), DeleyTime);//deley後にゴールをセットする
                    _isFirstReadyOfArmForFlyFlag = false;//FlyFlag用の腕位置検知のフラグを初期化
                }

                if (_averageSpeed >= BaseSpeed)
                {
                    Debug.Log("Fly!");
                }

                // 現在の位置情報を保存
                _prevPositionWaist = currentPosition1;

                this._delta = 0; //deltaの初期化
            }

            _isPreReadyOfArm = _isFirstReadyOfArm;
            _delta += Time.deltaTime;
        }
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
    
    // private void GetEagleTargetPos()//鷹のターゲットの位置をセットする
    // {
    //     gameManager.SetEagleTarget(tmpGoalPosObj.position);
    //     
    //     Debug.Log("一時ゴールの位置："+tmpGoalPosObj.position);
    // }
    public Vector3 GetEagleTargetFromSwing()//鷹のターゲットの位置を取得する(GameManagerがEagleTargetの位置を動かす)
    {
        //return _pointerPosition.position;
        Vector3 target = _targetChoicer.SetTarget();
        _pointerPosition.transform.position = target;
        return target;
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
        if ((_scene == 2 || _scene == 5) && _isFirstReadyOfArm == true)
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

        return average;//平均を返す
    }
    
    //仮のゴールをセットする
    public Vector3 GetPlaceholderEagleTargetPos()//仮のEagleTargetの座標を取得(GameManagerがEagleTargetの位置を動かす)
    {
        //仮の必要がなくなったので，直に入れています（試用）
        return _targetChoicer.SetTarget();
        //return _placeholderEagleTarget;
    }
    
    private IEnumerator LeaveReadyFlagTrue()
    {
        Debug.Log("飛ぶか！？");
        _isFirstReadyOfArmForFlyFlag = true;
        yield return new WaitForSeconds(_armFlagContinuousTime);
        _isFirstReadyOfArmForFlyFlag = false;
        Debug.Log("飛ばない～");
    }
}

