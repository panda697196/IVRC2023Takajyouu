﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardwareManager : MonoBehaviour
{
    [Header("参照スクリプト")]
    [SerializeField] private Sender _weightSender;
    [SerializeField] private Sender _ropeSender;
    [SerializeField] private Sender _pressSender;
    [SerializeField] private Sender _windSender;
    [SerializeField] private Sender _freedomDropSender;
    [SerializeField] private PullInspector _pullInspector;
    // モータの回転速度(0-255)，接頭語はモータの種類，接尾語は順転か逆転か
    [Header("モータ速度")]
    [SerializeField] private int _ropeTightSpeed = 100;
    [SerializeField] private int _ropeLooseSpeed = 255;
    [SerializeField] private int _ropeSpeedFast = 200;
    [SerializeField] private int _ropeSpeedMiddle = 100;
    [SerializeField] private int _ropeSpeedLow = 50;
    [SerializeField] private int _weightDropSpeed = 255;
    [SerializeField] private int _weightLiftSpeed = 255;
    // モータを止めるまでの時間[s]，接頭語はモータの種類，接尾語は順転か逆転か
    [Header("モータを止めるまでの時間[s]")]
    [SerializeField] private float _ropeTimeFast = 5f;
    [SerializeField] private float _ropeTimeMiddle = 5f;
    [SerializeField] private float _weightDropTime = 2f;
    [SerializeField] private float _weightLiftTime = 2f;
    [SerializeField] private float _weightLiftStandbyTime = 1.5f;
    [Header("ファン稼働時間[s]")]
    [SerializeField] private float _fanTimeOfCome = 1.5f;
    [SerializeField] private float _fanTimeOfGo = 2f;
    [Header("デバック等")]
    public bool _isDebug = true;
    [SerializeField] private float _weightSec = 0;//おもりの位置　上が0で下げるほど＋
    [SerializeField] private float _ropeSec = 0 ;
    [SerializeField] private float _ropePosition = 0; //ロープの位置　たるんでいると0できつくするほど+
    [SerializeField] private int _angleOfStopper = 65;
    [SerializeField] private int _angleOfGrab = 0;

    private bool _isWeightUp = false; // おもりが動いているかどうか
    private bool _isWeightDown = false;
    private bool _isRopeTight = false; // ロープが動いているがどうか
    private bool _isRopeLoose = false;

    private bool _isStandbyFinished = false;

    private float _timeToUpdate;
    private int _tightenSpeed;
    private float _timeFromTighten = 0;

    // Start is called before the first frame update
    void Start()
    {
        _tightenSpeed = _ropeSpeedFast;
        _weightSender.DataSend("S\n");
        _ropeSender.DataSend("S\n");
        _freedomDropSender.DataSend("0\n");
        _pressSender.DataSend("180\n");

        if (_weightDropTime != (_weightLiftTime + _weightLiftStandbyTime))
        {
            Debug.LogWarning("重りの巻取りと緩めの時間が一致していません　_weightDropTime系を確認してください");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //おもりの場所を把握（単位は[sec]）
        _timeToUpdate = Time.deltaTime;
        if (_isWeightUp)
            _weightSec -= _timeToUpdate;
        if (_isWeightDown)
            _weightSec += _timeToUpdate;
        if (_isRopeTight)
        {
            _ropeSec += _timeToUpdate;
            _timeFromTighten += _timeToUpdate;
            _ropePosition += _timeToUpdate * _tightenSpeed;
        }
        if (_isRopeLoose)
        {
            _ropeSec -= _timeToUpdate;
            _ropePosition -= _timeToUpdate * _ropeLooseSpeed;
        }

        //ロープを巻き取るとき，速度を可変にする
        if (_isRopeTight)
            ChangeRopeTightenSpeed();

        //Debug
        if (_isDebug)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                // StartCoroutine(StandbyComeHawk());
                StandbyComeHawk();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                //StartCoroutine(ComeHawk());
                ComeHawk();
            }
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                //StartCoroutine(Disappear());
                StandbyDisappear();
            }
            if (Input.GetKeyDown(KeyCode.Period))
            {
                //StartCoroutine(Disappear());
                Disappear();
                //_freedomDropSender.DataSend("0\n");
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                _freedomDropSender.DataSend(_angleOfStopper + "\n");
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                _freedomDropSender.DataSend("0\n");
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                _ropeSender.DataSend("C\n" + _ropeTightSpeed.ToString() + "\n"); // 紐を張る
            }
            if (Input.GetKeyUp(KeyCode.I))
            {
                _ropeSender.DataSend("S\n");
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                _ropeSender.DataSend("R\n" + _ropeLooseSpeed.ToString() + "\n"); // 紐を緩める
            }
            if (Input.GetKeyUp(KeyCode.J))
            {
                _ropeSender.DataSend("S\n");
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                _weightSender.DataSend("R\n" + _weightLiftSpeed.ToString() + "\n"); // 重りを上げ始める
            }
            if (Input.GetKeyUp(KeyCode.U))
            {
                _weightSender.DataSend("S\n");
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                _weightSender.DataSend("C\n" + _weightDropSpeed.ToString() + "\n"); // 重りを落とし始める
            }
            if (Input.GetKeyUp(KeyCode.H))
            {
                _weightSender.DataSend("S\n");
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                StartCoroutine(AppearWind());
                //_freedomDropSender.DataSend("65\n");
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                _pressSender.DataSend(_angleOfGrab.ToString() + "\n"); // 鷹がつかんでいる感覚の提示
            }
            if (Input.GetKeyUp(KeyCode.G))
            {
                _pressSender.DataSend("180\n");
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                _weightSender.DataSend("S\n");
                _ropeSender.DataSend("S\n");
            }
        }
    }

    private void OnApplicationQuit()
    {
        //ゲーム終了時にすべてのモータを停止する
        _weightSender.DataSend("S\n");
        _ropeSender.DataSend("S\n");
        _freedomDropSender.DataSend("0\n");
        _pressSender.DataSend("180\n");
    }
    public void StandbyComeHawk()// 鷹が腕に留まる前に　紐を張るなど準備をする
    {
        StartCoroutine(StandbyRope());
        StartCoroutine(StandbyWeight());
        //GameManagerに処理が終了したことの報告
    }
    public void ComeHawk()// 鷹が腕に留まる瞬間
    {
        StartCoroutine(Stimulate());
        StartCoroutine(AppearWind());
        StartCoroutine(PressByHawk());
        //GameManagerに処理が終了したことの報告
    }

    public void StandbyDisappear()
    {
        StartCoroutine(StandbyDisappearShock());
    }
    public void Disappear() // 鷹が飛び立つときの関数
    {
        StartCoroutine(DisappearShock());
        StartCoroutine(DisappearWind());
        StartCoroutine(UnpressByHawk());
        //GameManagerに処理が終了したことの報告
    }
    
    public IEnumerator StandbyRope()
    {
        //Debug.Log("Debug");
        StartCoroutine(StandbyWeight());
        //_ropeSender.DataSend("C\n" + _ropeSpeedC.ToString() + "\n"); // 紐を張る
        _ropeSender.DataSend("C\n" + _ropeSpeedFast.ToString() + "\n"); // 紐を張る
        _isRopeTight = true;
        _timeFromTighten = 0;
        _tightenSpeed = _ropeSpeedFast;

        //yield return new WaitUntil(() => _pressSender._afterstop == true); // 紐が張ったことを確認できるまで待機
        yield return new WaitUntil(() => _pullInspector.GetPullStatus() == true); // 紐が張ったことを確認できるまで待機
        _ropeSender.DataSend("S\n"); // 紐の巻き取り停止
        _isRopeTight = false;
        _isStandbyFinished = true;
    }

    public IEnumerator StandbyWeight()
    {
        //紐を抑える
        _freedomDropSender.DataSend(_angleOfStopper.ToString() + "\n");
        yield return new WaitForSeconds(0.5f);
        //紐を下ろす
        _weightSender.DataSend("C\n" + _weightDropSpeed.ToString() + "\n"); // 重りを落とし始める
        _isWeightDown = true;
        yield return new WaitForSeconds(_weightDropTime);
        _weightSender.DataSend("S\n"); // 重りが落ちたらモータを止める
        _isWeightDown = false;
        //*/
    }

    public IEnumerator Stimulate()
    {
        /* 自由落下でないバージョンのコード
        _weightSender.DataSend("C\n" + _weightDropSpeed.ToString() + "\n"); // 重りを落とし始める
        _isWeightDown = true;
        yield return new WaitForSeconds(_weightDropTime);
        _weightSender.DataSend("S\n"); // 重りが落ちたらモータを止める
        _isWeightDown = false;
        */
        _freedomDropSender.DataSend("0\n");
        yield return new WaitForSeconds(1f);
        
        _ropeSender.DataSend("R\n" + _ropeLooseSpeed.ToString() + "\n"); // 紐を緩める
        _isRopeLoose = true;
        _pullInspector.OffPullStatus();
        float _looseTime = CalcLoosenTime();
        Debug.Log("Loose time = " + _looseTime);
        yield return new WaitForSeconds(_looseTime);
        //yield return new WaitForSeconds(_ropeTimeR);
        _ropeSender.DataSend("S\n");
        _isRopeLoose = false;
        _isStandbyFinished = false;
    }

    public IEnumerator StandbyDisappearShock()
    {
        _weightSender.DataSend("R\n" + _weightLiftSpeed.ToString() + "\n");
        _isWeightUp = true;
        yield return new WaitForSeconds(_weightLiftStandbyTime);
        _weightSender.DataSend("S\n");
        _isWeightUp = false;
    }
    public IEnumerator DisappearShock() // 鷹が飛び立つときの衝撃提示の関数
    {
        _weightSender.DataSend("R\n" + _weightLiftSpeed.ToString() + "\n");
        _isWeightUp = true;
        yield return new WaitForSeconds(_weightLiftTime);
        _weightSender.DataSend("S\n");
        _isWeightUp = false;
    }

    private void ChangeRopeTightenSpeed() //時間ごとにロープの巻き取り速度を遅くする　閾値は_ropeTime系
    {
        if (_timeFromTighten > _ropeTimeFast && (_timeFromTighten - _timeToUpdate) <= _ropeTimeFast)
        {
            _tightenSpeed = _ropeSpeedMiddle;
            _ropeSender.DataSend("C\n" + _tightenSpeed.ToString() + "\n"); // 紐を張る
        }
        else if (_timeFromTighten > (_ropeTimeFast + _ropeTimeMiddle) && (_timeFromTighten - _timeToUpdate) <= (_ropeTimeFast + _ropeTimeMiddle))
        {
            _tightenSpeed = _ropeSpeedLow;
            _ropeSender.DataSend("C\n" + _tightenSpeed.ToString() + "\n"); // 紐を張る
        }
    }

    private float CalcLoosenTime()//ロープを緩める時間を，きつくした時間から逆算
    {
        if (_timeFromTighten <= _ropeTimeFast)
        {
            return (_timeFromTighten * _ropeSpeedFast) / _ropeLooseSpeed;
        }
        else if (_timeFromTighten <= (_ropeTimeFast + _ropeTimeMiddle))
        {
            return ((_ropeTimeFast * _ropeSpeedFast) + (_timeFromTighten - _ropeTimeFast) * _ropeSpeedMiddle) / _ropeLooseSpeed;
        }
        else
        {
            return (_ropeTimeFast * _ropeSpeedFast + _ropeTimeMiddle * _ropeSpeedMiddle + (_timeFromTighten - _ropeTimeFast - _ropeTimeMiddle) * _ropeSpeedLow) / _ropeLooseSpeed;
        }
    }
    
    public IEnumerator PressByHawk() // 鷹が腕に止まるときの圧力提示の関数
    {
        yield return new WaitForSeconds(0.5f); //到着まで，時間を遅らせる
        _pressSender.DataSend(_angleOfGrab.ToString() + "\n"); // 鷹がつかんでいる感覚の提示
    }

    public IEnumerator　UnpressByHawk() // 鷹が飛び立つときの圧力提示の関数
    {
        //爪を放すプログラム
        yield return new WaitForSeconds(0.5f);//飛び立ちから離すまで，時間を遅らせる
        _pressSender.DataSend("180\n");
    }
    public IEnumerator AppearWind() // 鷹が腕に止まるときの風提示の関数
    {
        _windSender.DataSend("S\n");
        yield return new WaitForSeconds(_fanTimeOfCome); // 一定時間風を起こす
        _windSender.DataSend("S\n");
    }

    public IEnumerator DisappearWind() // 鷹が飛び立つときの風提示の関数
    {
        _windSender.DataSend("S\n");
        yield return new WaitForSeconds(_fanTimeOfGo); // 一定時間風を起こす
        _windSender.DataSend("S\n");
    }
}
