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

    [Header("ユーザの手オブジェクトの目標地点")]public GameObject _hand;
    private bool _isOnHand;

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

    public bool _isTargetGet = false;
    private bool _isTargetCalcOnce = false;
    private Vector3 _targetAroundNew;

    private Vector3 _eagle2target;
    public bool _isOn;

    private bool _isAroundOver;
    [SerializeField] private GameObject debug;
    //public bool GetIsAroundOver => _isAroundOver;
    void Start()
    {
        _edit = gameObject.GetComponent<Eagle_Edit>();
        //_eaglePositionChange = gameObject.transform.parent.gameObject;
    }
    
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(_edit.GetEagleCurrentAnimState());
        Animator _animator = GetComponent<Animator>();
        _animator.SetFloat("Speed", _speed);
        
        switch (_edit.GetEagleCurrentAnimState())
        {
            case Eagle_Edit.EagleState.Idle:
                // if (_isFly)
                // {
                //    
                //     Debug.Log("Fly");
                //     FlyTo(_target.transform,_flyState.ToString());
                // }
                var hand2eagle = gameObject.transform.position - _hand.transform.position;
                if (hand2eagle.magnitude < 0.7f &&!_isOnHand)
                {
                    // _handAdjust = hand2eagle;
                    
                    _isOnHand = true;
                    gameObject.GetComponent<Animator>().applyRootMotion=false;
                    transform.parent = _hand.transform;
                }
               
                break;
            case Eagle_Edit.EagleState.Takeoff:
                //Debug.Log("takeoff");
                gameObject.GetComponent<Animator>().applyRootMotion=true;
                _isOnHand = false;
                gameObject.transform.parent = null;
                TakeOff(_target.transform);
                break;
            
            case Eagle_Edit.EagleState.Fly:
            {
                //Debug.Log("Fly");
                FlyTo(_target.transform,_flyState.ToString());
                break;
            }
            case Eagle_Edit.EagleState.Lauding:
               // Debug.Log("Lauding");
                Land(_target.transform);
                break;
 
        }
    }
    
    // 飛び立つ
    void TakeOff(Transform target)
    {
        //Debug.Log("takeoff");
        //目的地の方向に向かせる
        gameObject.transform.LookAt(target);
        //_isFly = true;
        _flyFirst = true;
        //_edit.TakeOff();
        //ここeagle_editのカレントステイトを編集してます．なんとかしたいね
        //_edit.SetEagleState(Eagle_Edit.EagleState.Idle);
    }

    
    // 向かう
    void FlyTo(Transform target,string state)
    {
        
        switch (state)
        {
            case "laud":
            {
                //Debug.Log("laud");
                //ターゲットオブジェクトの着地面を補正
                var objectTop = target.transform.position.y+target.localScale.y/2;
        
                var arrangeTarget = target.position + (gameObject.transform.position - target.position).normalized * xzMargin;
                arrangeTarget.y = objectTop + yMargin ;

        
                if (_flyFirst)
                {
                   // Debug.Log("FlyFirst");
                    _slerpStart = gameObject.transform.position;
                    _flyTime = 0;
                    _flyFirst = false;
                }
                // 次の移動場所を計算する
                _flyTime += Time.deltaTime;
                var nextPos = Vector3.Slerp(_slerpStart, arrangeTarget, Mathf.Min(1.0f,_flyTime / _limitTime));
               

                //debug.transform.position = nextPos;
        
                // 向かせる
                gameObject.transform.LookAt(nextPos);

                if (Mathf.Abs((gameObject.transform.position - arrangeTarget).magnitude) < 1f)
                {
                    gameObject.transform.LookAt(new Vector3(target.transform.position.x,arrangeTarget.y,target.transform.position.z));
                    _edit.SetEagleState(Eagle_Edit.EagleState.Lauding);
                    //_isFly = false;
                }
                
                break;
            }

            case "target":
            {
               //Debug.Log("target");
               if (!_isTargetCalcOnce)
               {
                   _eagle2target = (target.transform.position - gameObject.transform.position).normalized;
                   _targetAroundNew = target.transform.position + _eagle2target * 3;
                   _isTargetCalcOnce = true;
               }
               
                // 向かせる
                gameObject.transform.LookAt(_targetAroundNew);
                //debug.transform.position = _targetAroundNew;
                if ((_targetAroundNew - gameObject.transform.position).magnitude < 2f)
                {
                    _isTargetCalcOnce = false;
                    _flyState = FlyState.targetAround;
                }
                break;
            }

            case "targetAround":
            {
              // Debug.Log("targetAround");
                //ターゲットの周りを周回させる
                gameObject.transform.LookAt(CalcRotationPosition(target));
                //debug.transform.position = CalcRotationPosition(target);
                //Debug.Log(Vector3.Angle(_hand.transform.position-gameObject.transform.position,_eagle2target));
                //xz平面でベクトルを計算
                var eagle2handxz=new Vector2(_hand.transform.position.x - gameObject.transform.position.x,_hand.transform.position.z - gameObject.transform.position.z);
                var eagle2targetxz = new Vector2(_eagle2target.x, _eagle2target.z);
                //　もともとの鷹の位置のベクトルと，今の鷹と次の手の補正位置へのベクトルの角度が一定以上の時に，腕の着地用ステイトに切り替え
                if (_isOn &&Vector2.Angle(eagle2handxz, eagle2targetxz) > 172f && _isAroundOver)
                {
                    _target = _hand;
                    _flyFirst = true;
                    _flyState = _flyState = FlyState.getOnArm;
                }
                break;
            }
            case "getOnArm":
            {
               
                
                //Debug.Log("getOnArm");
                var arrangeTarget = target.position ;
                
                if (_flyFirst)
                {
                    _slerpStart = gameObject.transform.position;
                    _flyTime = 0;
                    _flyFirst = false;
                }
                
                // 次の移動場所を計算する
                _flyTime += Time.deltaTime;
                var nextPos = Vector3.Slerp(_slerpStart, target.transform.position, Mathf.Min(1.0f,_flyTime*4 / _limitTime));
                //デバッグ用．　目的地の位置にオブジェクトを置き合っているか確認している．
                //debug.transform.position = nextPos;
                // 向かせる
                //近すぎると挙動が変になるので，距離が2センチより遠かったら実行
                if (Mathf.Abs((gameObject.transform.position - arrangeTarget).magnitude) > 0.4f)
                {
                    //Debug.Log("LookAt");
                    gameObject.transform.LookAt(nextPos);
                }

                if (Mathf.Abs((gameObject.transform.position - arrangeTarget).magnitude) < 0.1f)
                {
                    //人間の手の上面を取得
                    var ArmTopAjust = yMargin;
                    //人間の手の位置に向くように修正
                    gameObject.transform.LookAt(new Vector3(target.transform.parent.transform.position.x,target.transform.parent.transform.position.y+ArmTopAjust,target.transform.parent.transform.position.z));
                    //Debug.Log(new Vector3(target.transform.parent.transform.position.x,target.transform.parent.transform.position.y+ArmTopAjust,target.transform.parent.transform.position.z));                
                                //着地モーション再生
                    _edit.SetEagleState(Eagle_Edit.EagleState.Lauding);
                    VariablesReset();
                    //_isFly = false;
                }
                
                break;
                
            }
            case "onlyTarget":
            {
                gameObject.transform.LookAt(target);
                //debug.transform.position = _targetAroundNew;
                break;
            }
        }
        
        
        

    }
    
    //着地
    void Land(Transform target)
    {
        var ArmTopAdjust = yMargin;
        gameObject.transform.LookAt(new Vector3(target.transform.parent.transform.position.x,target.transform.parent.transform.position.y+ArmTopAdjust,target.transform.parent.transform.position.z));
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
        debug.transform.position = pos;
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
        _isAroundOver = false;
        _isOn = false;
    }
}
