using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardwareManager : MonoBehaviour
{
    [Header("参照スクリプト")]
    [SerializeField] private Sender _weightSender;
    [SerializeField] private Sender _ropeSender;
    [SerializeField] private Sender _pressSender;
    [SerializeField] private Sender _windSender;
    [SerializeField] private PullInspector _pullInspector;
    // モータの回転速度(0-255)，接頭語はモータの種類，接尾語は順転か逆転か
    [Header("モータ速度")]
    [SerializeField] private int _ropeSpeedC = 100;
    [SerializeField] private int _ropeSpeedR = 255;
    [SerializeField] private int _ropeSpeedFast = 200;
    [SerializeField] private int _ropeSpeedMiddle = 100;
    [SerializeField] private int _ropeSpeedLow = 50;
    [SerializeField] private int _weightSpeedC = 255;
    [SerializeField] private int _weightSpeedR = 255;
    // モータを止めるまでの時間[s]，接頭語はモータの種類，接尾語は順転か逆転か
    [Header("モータを止めるまでの時間[s]")]
    [SerializeField] private float _ropeTimeC = 5;
    [SerializeField] private float _ropeTimeR = 5;
    [SerializeField] private float _ropeTimeFast = 5f;
    [SerializeField] private float _ropeTimeMiddle = 5f;
    [SerializeField] private float _weightTimeC = 2f;
    [SerializeField] private float _weightTimeR = 2f;
    [Header("ファン稼働時間[s]")]
    [SerializeField] private float _fanTimeOfCome = 1.5f;
    [SerializeField] private float _fanTimeOfGo = 2f;
    [Header("デバック等")]
    public bool _isDebug = true;
    [SerializeField] private float _weightSec = 0;//おもりの位置　上が0で下げるほど＋
    [SerializeField] private float _ropeSec = 0 ;
    [SerializeField] private float _ropePosition = 0; //ロープの位置　たるんでいると0できつくするほど+

    private bool _isWeightUp = false; // おもりが動いているかどうか
    private bool _isWeightDown = false;
    private bool _isRopeTight = false; // ロープが動いているがどうか
    private bool _isRopeLoose = false;

    private float _timeToUpdate;
    private int _tightenSpeed;
    private float _timeFromTighten = 0;

    // Start is called before the first frame update
    void Start()
    {
        _tightenSpeed = _ropeSpeedFast;
        _weightSender.DataSend("S\n");
        _ropeSender.DataSend("S\n");
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
            _ropePosition -= _timeToUpdate * _ropeSpeedR;
        }

        //ロープを巻き取るとき，速度を可変にする
        if (_isRopeTight)
            ChangeRopeTightenSpeed();

        //Debug
        if (_isDebug)
        {
            if (Input.GetKey(KeyCode.P))
            {
                StartCoroutine(StandbyComeHawk());
            }
            if (Input.GetKey(KeyCode.L))
            {
                StartCoroutine(ComeHawk());
            }
            if (Input.GetKey(KeyCode.Comma))
            {
                StartCoroutine(Disappear());
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                _ropeSender.DataSend("C\n" + _ropeSpeedC.ToString() + "\n"); // 紐を張る
            }
            if (Input.GetKeyUp(KeyCode.I))
            {
                _ropeSender.DataSend("S\n");
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                _ropeSender.DataSend("R\n" + _ropeSpeedR.ToString() + "\n"); // 紐を緩める
            }
            if (Input.GetKeyUp(KeyCode.J))
            {
                _ropeSender.DataSend("S\n");
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                _weightSender.DataSend("R\n" + _weightSpeedR.ToString() + "\n");
            }
            if (Input.GetKeyUp(KeyCode.U))
            {
                _weightSender.DataSend("S\n");
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                _weightSender.DataSend("C\n" + _weightSpeedC.ToString() + "\n"); // 重りを落とし始める
            }
            if (Input.GetKeyUp(KeyCode.H))
            {
                _weightSender.DataSend("S\n");
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                StartCoroutine(AppearWind());
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
        _weightSender.DataSend("S\n");
        _ropeSender.DataSend("S\n");
    }

    public IEnumerator Appear() // 鷹が腕に止まるときの関数
    {
        StartCoroutine(AppearShock());
        /*
        yield return new WaitUntil(() => RopeSender._afterstop == true);
        yield return new WaitForSeconds(風を起こすまでの時間);
        StartCoroutine(AppearWind());
        yield return new WaitForSeconds(重りが到達するまでの時間);
        StartCoroutine(AppearPress());
        */
        yield return AppearShock();
        //GameManagerに処理が終了したことの報告
    }

    public IEnumerator StandbyComeHawk()// 鷹が腕に留まる前に　紐を張るなど準備をする
    {
        StartCoroutine(StandbyShock());
        /*
        yield return new WaitUntil(() => _pullInspector.GetPullStatus() == true);
        */
        yield return StandbyShock();
        //GameManagerに処理が終了したことの報告
    }

    public IEnumerator ComeHawk()// 鷹が腕に留まる瞬間
    {
        StartCoroutine(Stimulate());
        /*
        StartCoroutine(AppearWind());
        yield return new WaitForSeconds(重りが到達するまでの時間);
        StartCoroutine(AppearPress());
        */
        yield return Stimulate();
        //GameManagerに処理が終了したことの報告
    }

    public IEnumerator Disappear() // 鷹が飛び立つときの関数
    {
        StartCoroutine(DisappearShock());
        /*
        StartCoroutine(DisappearPress());
        yield return new WaitForSeconds(風を起こすまでの時間);
        StartCoroutine(DisappearWind());
        */
        yield return DisappearShock();
        //GameManagerに処理が終了したことの報告
    }

    public IEnumerator AppearShock() // 鷹が腕に止まるときの衝撃提示の関数
    {
        //_ropeSender.DataSend("C\n" + _ropeSpeedC.ToString() + "\n"); // 紐を張る
        _ropeSender.DataSend("C\n" + _ropeSpeedFast.ToString() + "\n"); // 紐を張る
        _isRopeTight = true;
        _timeFromTighten = 0;
        _tightenSpeed = _ropeSpeedFast;
        
        //yield return new WaitUntil(() => _pressSender._afterstop == true); // 紐が張ったことを確認できるまで待機
        yield return new WaitUntil(() => _pullInspector.GetPullStatus() == true); // 紐が張ったことを確認できるまで待機
        _ropeSender.DataSend("S\n"); // 紐の巻き取り停止
        _isRopeTight = false;
        
        _weightSender.DataSend("C\n" + _weightSpeedC.ToString() + "\n"); // 重りを落とし始める
        _isWeightDown = true;
        yield return new WaitForSeconds(_weightTimeC);
        _weightSender.DataSend("S\n"); // 重りが落ちたらモータを止める
        _isWeightDown = false;
        
        _ropeSender.DataSend("R\n" + _ropeSpeedR.ToString() + "\n"); // 紐を緩める
        _isRopeLoose = true;
        _pullInspector.OffPullStatus();
        _pressSender._afterstop = false;
        yield return new WaitForSeconds(CalcLoosenTime());
        //yield return new WaitForSeconds(_ropeTimeR);
        _ropeSender.DataSend("S\n");
        _isRopeLoose = false;
    }

    public IEnumerator StandbyShock()
    {
        //_ropeSender.DataSend("C\n" + _ropeSpeedC.ToString() + "\n"); // 紐を張る
        _ropeSender.DataSend("C\n" + _ropeSpeedFast.ToString() + "\n"); // 紐を張る
        _isRopeTight = true;
        _timeFromTighten = 0;
        _tightenSpeed = _ropeSpeedFast;

        //yield return new WaitUntil(() => _pressSender._afterstop == true); // 紐が張ったことを確認できるまで待機
        yield return new WaitUntil(() => _pullInspector.GetPullStatus() == true); // 紐が張ったことを確認できるまで待機
        _ropeSender.DataSend("S\n"); // 紐の巻き取り停止
        _isRopeTight = false;
    }

    public IEnumerator Stimulate()
    {
        _weightSender.DataSend("C\n" + _weightSpeedC.ToString() + "\n"); // 重りを落とし始める
        _isWeightDown = true;
        yield return new WaitForSeconds(_weightTimeC);
        _weightSender.DataSend("S\n"); // 重りが落ちたらモータを止める
        _isWeightDown = false;
        
        _ropeSender.DataSend("R\n" + _ropeSpeedR.ToString() + "\n"); // 紐を緩める
        _isRopeLoose = true;
        _pullInspector.OffPullStatus();
        _pressSender._afterstop = false;
        yield return new WaitForSeconds(CalcLoosenTime());
        //yield return new WaitForSeconds(_ropeTimeR);
        _ropeSender.DataSend("S\n");
        _isRopeLoose = false;
    }

    public IEnumerator DisappearShock() // 鷹が飛び立つときの衝撃提示の関数
    {
        _weightSender.DataSend("R\n" + _weightSpeedR.ToString() + "\n");
        _isWeightUp = true;
        yield return new WaitForSeconds(_weightTimeR);
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
            return (_timeFromTighten * _ropeSpeedFast) / _ropeSpeedR;
        }
        else if (_timeFromTighten <= (_ropeTimeFast + _ropeTimeMiddle))
        {
            return ((_ropeTimeFast * _ropeSpeedFast) + (_timeFromTighten - _ropeTimeFast) * _ropeSpeedMiddle) / _ropeSpeedR;
        }
        else
        {
            return (_ropeTimeFast * _ropeSpeedFast + _ropeTimeMiddle * _ropeSpeedMiddle + (_timeFromTighten - _ropeTimeFast - _ropeTimeMiddle) * _ropeSpeedLow) / _ropeSpeedR;
        }
    }

    /*
    public IEnumerator AppearPress() // 鷹が腕に止まるときの圧力提示の関数
    {
        //爪で掴むプログラム
    }

    public IEnumerator DisappearPress() // 鷹が飛び立つときの圧力提示の関数
    {
        //爪を放すプログラム
    }
    */
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
