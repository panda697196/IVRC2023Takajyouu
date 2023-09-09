using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Eagle_Navigation : MonoBehaviour
{
    private Eagle_Edit _edit;
    [SerializeField] private GameObject _target;

    public void SetTarget(GameObject target)
    {
        _target = target;
    }
    
    // 仮なので時間があるときに作り直して
    //;private bool _isFly;

    [Header("球形補完のパラメータ")]
    //[SerializeField] private Vector3 _slerpCenter;
    [SerializeField] private float _limitTime;
    private float _flyTime;
    private bool _flyFirst;
    private Vector3 _slerpStart;

    public float _speed = 1.0f;
    public enum FlyState
    {
        laud,target,targetAround,getOnArm,onlyTarget
    }
    public void SetFlyState(FlyState state)
    {
        _flyState = state;
    }
    public float xzMargin = 2.0f;
    public float yMargin = 1.5f;
    [Header("飛行状況")]public FlyState _flyState;
    [Header("旋回飛行の半径")] public float _radius;
    [Header("旋回飛行の高さ補正")] public float _height;
    [Header("円運動の速さ")] public float _rotationSpeed=4.1f;
    private  float _totalTime;

    [Header("ユーザの手のオブジェクト目標地点")] public GameObject _hand;
    private GameObject _handPosition;

    public void SetHandPosition(GameObject a)
    {
        _handPosition=a;
    }
    
    private bool _isOnHand;
    public bool GetIsOnHand => _isOnHand;
    private GameObject _eaglePositionChange;
    //[Header("GameManagerにアクセス")] public GameManager _gameManager;
    //private ステイと　_gameState;
    public void SetIsOnHand(bool state)
    {
        _isOnHand = state;
    }
    private Vector3 _handAdjust;
    public void SetHandAdjust(Vector3 adjust)
    {
        _handAdjust = adjust;
    }
    
    private bool _isTargetCalcOnce = false;
    private Vector3 _targetAroundNew;

    private Vector3 _eagle2target;
    private bool _isHardGetOnStandby;

    public void SetIsHardGetOnStandby(bool b)
    {
        _isHardGetOnStandby=b;
    }



    private bool _isAroundOver;
    [SerializeField] private GameObject _debug;

    public float eagleHandTH;

    public Animator _animator;
    void Start()
    {
        _edit = gameObject.GetComponent<Eagle_Edit>();
        //Animator _animator = GetComponent<Animator>();
        _animator = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    void Update()
    {
       
       
        _animator.SetFloat("Speed", _speed);//アニメーターに再生速度を代入
        
        switch (_edit.GetEagleCurrentAnimState())
        {
            case Eagle_Edit.EagleState.Idle:
                //手から鷹までのベクトルを計算
                var hand2eagle = gameObject.transform.position - _hand.transform.position;
                //Debug.Log("Hand2Eagle Distance="+hand2eagle.magnitude);
                //もし手から鷹までの距離が一定以下かつ，鷹が手に載っていない場合
                if (hand2eagle.magnitude < eagleHandTH &&!_isOnHand)
                {
                    _isOnHand = true;
                    //鷹のアニメーターのルートモーションをオフにする これで鷹が手に乗るようになる．
                    _animator.applyRootMotion=false;
                    //鷹を手の子オブジェクトにし，手の動きを反映できるようにする．
                    transform.parent = _hand.transform;
                }
               
                break;
            case Eagle_Edit.EagleState.Takeoff:
                //鷹のルートモーションをオンにする．これで鷹の飛ぶ関連のアニメーションがしっかり動くようになる
                gameObject.GetComponent<Animator>().applyRootMotion=true;
                //手の上に載っているフラグを切る
                _isOnHand = false;
                //鷹の親子関係を切る．これで手の動きを鷹に反映させないようにする．
                gameObject.transform.parent = null;
                //鷹を目標の場所に向かって飛ばす
                TakeOff(_target.transform);
                break;
            
            case Eagle_Edit.EagleState.Fly:
            {
                //鷹をターゲットに向かって飛ばせる．飛んでいる状況によって挙動が異なる
                //laud,target,targetAround,getOnArm,onlyTarget
                //laud →目標まで飛んでいき，着陸する．物体の大きさで補正をかけているのでその分修正する必要あり．
                //target →目標に対して，少し余分に飛んでいき，その後旋回行動につながる
                //targetAround,→目標の周りを任意の半径，高さで旋回軌道する
                //getOnArm →腕（手）に向かって着陸する.なお，ターゲットはアニメーションの移動を考慮した座標におかれたゲームオブジェクトを使う必要がある．
                //onlyTarget →ただターゲットの方向を向いて飛んでいく
                FlyTo(_target.transform,_flyState.ToString());
                break;
            }
            case Eagle_Edit.EagleState.Landing:
                Land(_target.transform);
                break;
 
        }
    }
    
    // 飛び立つ
    void TakeOff(Transform target)
    {
        //目的地の方向に向かせる
        gameObject.transform.LookAt(target);
        _flyFirst = true;
    }

    
    // 向かう
    void FlyTo(Transform target,string state)
    {
        
        switch (state)
        {
            case "laud":
            {
                //ターゲットオブジェクトの着地面を補正
                var objectTop = target.transform.position.y;
        
                var arrangeTarget = target.position + (gameObject.transform.position - target.position).normalized * xzMargin;
                arrangeTarget.y = objectTop + yMargin ;

        
                if (_flyFirst)
                {
                    _slerpStart = gameObject.transform.position;
                    _flyTime = 0;
                    _flyFirst = false;
                }
                // 次の移動場所を計算する
                _flyTime += Time.deltaTime;
                //_flyTimeにリミットタイムの20分/1をたしているのは，最初のスタート地点が同じ場所だと鷹の方向がおかしくなるため．
                var nextPos = Vector3.Slerp(_slerpStart, arrangeTarget, Mathf.Min(1.0f,(_flyTime+_limitTime/20) / _limitTime));
            
                // 向かせる
                gameObject.transform.LookAt(nextPos);

                if (Mathf.Abs((gameObject.transform.position - arrangeTarget).magnitude) < 1f)
                {
                    gameObject.transform.LookAt(new Vector3(target.transform.position.x,arrangeTarget.y,target.transform.position.z));
                    _edit.SetEagleState(Eagle_Edit.EagleState.Landing);
                }
                
                break;
            }

            case "target":
            {
                if (!_isTargetCalcOnce)
                {
                    //移動位置をターゲット座標から＋3mの位置にしている．そうすることで多少オーバーランしてから旋回軌道に入るようになっている．
                    _eagle2target = (target.transform.position - gameObject.transform.position).normalized;
                    _targetAroundNew = target.transform.position + _eagle2target * 3;
                    _isTargetCalcOnce = true;
                }
               
                // 向かせる
                gameObject.transform.LookAt(_targetAroundNew);
                
                if ((_targetAroundNew - gameObject.transform.position).magnitude < 1f)//オーバーランの座標と鷹の座標の誤差が１以下の時
                {
                    _isTargetCalcOnce = false;
                    _flyState = FlyState.targetAround;
                }
                break;
            }

            case "targetAround":
            {
                

                gameObject.transform.LookAt(CalcRotationPosition(target));

                //xz平面でベクトルを計算
                var eagle2handxz=new Vector2(_hand.transform.position.x - gameObject.transform.position.x,_hand.transform.position.z - gameObject.transform.position.z);
                var eagle2targetxz = new Vector2(_eagle2target.x, _eagle2target.z);
                //　もともとの鷹の位置のベクトルと，今の鷹と次の手の補正位置へのベクトルの角度が一定以上の時に，腕の着地用ステイトに切り替え.なんとなく172度にしている．
                //_isHardGetOnStandbyはハードウェアの条件が満たされた時．
                if (_isHardGetOnStandby &&Vector2.Angle(eagle2handxz, eagle2targetxz) > 172f && _isAroundOver)
                {
                    _target = _handPosition;
                    _flyFirst = true;
                    _flyState = FlyState.getOnArm;
                }
                break;
            }
            case "getOnArm":
            {
                
                var arrangeTarget = target.position ;
                
                if (_flyFirst)
                {
                    _slerpStart = gameObject.transform.position;
                    _flyTime = 0;
                    _flyFirst = false;
                }
                
                // 次の移動場所を計算する
                _flyTime += Time.deltaTime;
                //_flyTimeにリミットタイムの20分/1をたしているのは，最初のスタート地点が同じ場所だと鷹の方向がおかしくなるため．
                var nextPos = Vector3.Slerp(_slerpStart, target.transform.position, Mathf.Min(1.0f,(_flyTime+_limitTime/20) / _limitTime));
                _debug.transform.position=nextPos;
                //近すぎると挙動が変になるので，距離が4センチより遠かったら実行
                if (Mathf.Abs((gameObject.transform.position - arrangeTarget).magnitude) > 0.4f)
                {
                    gameObject.transform.LookAt(nextPos);
                }
                //鷹とターゲットオブジェクトとの距離が0.01以下の時に着地モーションへと以降
                if (Mathf.Abs((gameObject.transform.position - arrangeTarget).magnitude) < 0.05f)
                {
                    //人間の手の上面を取得
                    //var ArmTopAjust = yMargin;
                    
                    //人間の手の位置に向くように修正
                    gameObject.transform.LookAt(new Vector3(_hand.transform.position.x,
                        gameObject.transform.position.y,_hand.transform.position.z));
                    //着地モーション再生
                    _edit.SetEagleState(Eagle_Edit.EagleState.Landing);
                    VariablesReset();
                }
                
                break;
                
            }
            case "onlyTarget":
            {
                gameObject.transform.LookAt(target);

                break;
            }
        }
        
        
        

    }
    
    //着地
    void Land(Transform target)
    {
        var ArmTopAdjust = yMargin;
        var eagle2hand = _hand.transform.position - gameObject.transform.position;
        var handLaudPoint = eagle2hand + gameObject.transform.position;
        handLaudPoint.y = gameObject.transform.position.y;
        //_debug.transform.position = handLaudPoint;
        //gameObject.transform.LookAt(handLaudPoint);
        //Debug.Log(new Vector3(target.transform.parent.transform.position.x,target.transform.parent.transform.position.y+ArmTopAdjust,target.transform.parent.transform.position.z));                

        //_isFly = false;
    }
    
    //回転運動を作る
    Vector3 CalcRotationPosition(Transform _centerObject)
    {
        Vector3 initPos = _centerObject.position; 
        _totalTime += Time.deltaTime;
        // 位相を計算.
        var phase = (float)(_totalTime * _rotationSpeed / _radius +Mathf.PI/2);
        
        // 位相から位置を計算．
        float xPos = _radius * Mathf.Cos(phase);
        float zPos = _radius * Mathf.Sin(phase);
        if (phase > 2.5 * Mathf.PI )
        {
            _isAroundOver = true;
        }
        // ゲームオブジェクトの位置を設定.中心となるオブジェクトから半径分の円運動をする．高さは中心物体の高さ＋_height
        Vector3 pos = new Vector3(initPos.x+ xPos, initPos.y +_height,initPos.z + zPos);
        //_debug.transform.position = pos;
        return (pos);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("PLAYYER COLLISION");
            _hand = col.gameObject;
            _isOnHand = true;
            _handAdjust = gameObject.transform.position-col.transform.position;
        }
    }

    private void VariablesReset()
    {
        _totalTime = 0.0f;
        _flyTime = 0.0f;
        _isAroundOver = false;
        _isHardGetOnStandby = false;
    }

    public bool IsRotating()
    {
        if(_flyState == FlyState.targetAround &&_isAroundOver)
        {
            return true;
        }
        return false;

    }
}
