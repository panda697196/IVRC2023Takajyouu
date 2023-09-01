using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Eagle_Navigation : MonoBehaviour
{
    private Eagle_Edit _edit;
    [SerializeField] private GameObject _target;
    
    
    // 仮なので時間があるときに作り直して
    private bool _isFly;

    [Header("球形補完のパラメータ")]
    //[SerializeField] private Vector3 _slerpCenter;
    [SerializeField] private float _limitTime;
    private float _flyTime;
    private bool _flyFirst;
    private Vector3 _slerpStart;

    public float _speed = 1.0f;
    public enum FlyState
    {
        laud,target,targetAround
    }

    [Header("飛行状況")]public FlyState _flyState;
    [Header("旋回飛行の半径")] public float _radius;
    [Header("旋回飛行の高さ補正")] public float _height;　
    private  float _totalTime;
    void Start()
    {
        _edit = gameObject.GetComponent<Eagle_Edit>();
    }

    // Update is called once per frame
    void Update()
    {
        Animator _animator = GetComponent<Animator>();
        _animator.SetFloat("Speed", _speed);
        
        switch (_edit.EagleCurrentState)
        {
            case Eagle_Edit.EagleState.Idle:
                if (_isFly)
                {
                    FlyTo(_target.transform,_flyState.ToString());
                }
                break;
            case Eagle_Edit.EagleState.Takeoff:
                TakeOff(_target.transform);
                break;
            case Eagle_Edit.EagleState.Lauding:
                Land(_target.transform);
                break;
        }
    }
    
    // 飛び立つ
    void TakeOff(Transform target)
    {
        _isFly = true;
        _flyFirst = true;
    }

    [SerializeField] private GameObject debug;
    // 向かう
    void FlyTo(Transform target,string state)
    {
        switch (state)
        {
            case "laud":
            {
                Debug.Log("laud");
                //アニメーションのxz平面の移動距離とy軸方向の移動距離
                var xzMargin = 2.0f;
                var yMargin = 1.5f;
        
                //ターゲットオブジェクトの着地面を補正
                var objectTop = target.localScale.y/2;
        
                var arrangeTarget = target.position + (gameObject.transform.position - target.position).normalized * xzMargin;
                arrangeTarget.y = objectTop + yMargin;

        
                if (_flyFirst)
                {
                    _slerpStart = gameObject.transform.position;
                    _flyTime = 0;
                    _flyFirst = false;
                }
        

        
                // 次の移動場所を計算する
                _flyTime += Time.deltaTime;
                var nextPos = Vector3.Slerp(_slerpStart, arrangeTarget, Mathf.Min(1.0f,_flyTime / _limitTime));
               

                debug.transform.position = nextPos;
        
                // 向かせる
                gameObject.transform.LookAt(nextPos);

                if (Mathf.Abs((gameObject.transform.position - arrangeTarget).magnitude) < 1f)
                {
                    _edit.SetEagleState(Eagle_Edit.EagleState.Lauding);
                    _isFly = false;
                }
                // 位置を設定
                //gameObject.transform.position = nextPos;
                break;
            }

            case "target":
            {
                Debug.Log("target");
                // 向かせる
                gameObject.transform.LookAt(_target.transform);
                debug.transform.position = _target.transform.position;
                break;
            }

            case "targetAround":
            {
                Debug.Log("targetAround");
                //ターゲットの周りを周回させる
                gameObject.transform.LookAt(CalcRotationPosition(_target));
                debug.transform.position = CalcRotationPosition(_target);
                break;
            }
        }
        
        
        

    }
    
    //着地
    void Land(Transform target)
    {
        _isFly = false;
    }
    
    //回転運動を作る
    Vector3 CalcRotationPosition(GameObject _centerObject)
    {
        Vector3 initPos = _centerObject.transform.position; 
        _totalTime += Time.deltaTime;
        // 位相を計算.
        var phase = (float)(_totalTime * 3.905*_speed / _radius);
        
        // 位相から位置を計算．
        float xPos = _radius * Mathf.Cos(phase) ;
        float zPos = _radius * Mathf.Sin(phase);

        // ゲームオブジェクトの位置を設定.中心となるオブジェクトから半径分の円運動をする．高さは中心物体の高さ＋_height
        Vector3 pos = new Vector3(initPos.x+ xPos, initPos.y +_height,initPos.z + zPos);
        return (pos);
    }
}
