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
    [SerializeField] private int _ropeSpeedR = 100;
    [SerializeField] private int _weightSpeedC = 255;
    [SerializeField] private int _weightSpeedR = 255;
    // モータを止めるまでの時間[s]，接頭語はモータの種類，接尾語は順転か逆転か
    [Header("モータを止めるまでの時間[s]")]
    [SerializeField] private float _ropeTimeC = 5;
    [SerializeField] private float _ropeTimeR = 5;
    [SerializeField] private float _weightTimeC = 5;
    [SerializeField] private float _weightTimeR = 5;
    [Header("デバック等")]
    public bool _isDebug = true;
    [SerializeField] private float _weightPosition = 0;//おもりの位置　上が0で下げるほど＋
    [SerializeField] private float _ropePosition = 0; //ロープの位置　たるんでいると0できつくするほど+

    private bool _isWeightUp = false; // おもりが動いているかどうか
    private bool _isWeightDown = false;
    private bool _isRopeTight = false; // ロープが動いているがどうか
    private bool _isRopeLoose = false;

    private float _timeToUpdate;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //おもりの場所を把握（単位は[sec]）
        _timeToUpdate = Time.deltaTime;
        if (_isWeightUp)
            _weightPosition -= _timeToUpdate;
        if (_isWeightDown)
            _weightPosition += _timeToUpdate;
        if (_isRopeTight)
            _ropePosition += _timeToUpdate;
        if (_isRopeLoose)
            _ropePosition -= _timeToUpdate;
        
        
        //Debug
        if (_isDebug)
        {
            if (Input.GetKey(KeyCode.P))
            {
                StartCoroutine(Appear());
            }
            if (Input.GetKey(KeyCode.L))
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
        }
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
        _ropeSender.DataSend("C\n" + _ropeSpeedC.ToString() + "\n"); // 紐を張る
        _isRopeTight = true;
        
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
        _pressSender._afterstop = false;
        yield return new WaitForSeconds(_ropeTimeR);
        _ropeSender.DataSend("S\n");
        _isRopeLoose = false;
    }

    public IEnumerator StandbyShock()
    {
        _ropeSender.DataSend("C\n" + _ropeSpeedC.ToString() + "\n"); // 紐を張る
        _isRopeTight = true;
        
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
        _pressSender._afterstop = false;
        yield return new WaitForSeconds(_ropeTimeR);
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

    /*
    public IEnumerator AppearPress() // 鷹が腕に止まるときの圧力提示の関数
    {
        //爪で掴むプログラム
    }

    public IEnumerator DisappearPress() // 鷹が飛び立つときの圧力提示の関数
    {
        //爪を放すプログラム
    }

    public IEnumerator AppearWind() // 鷹が腕に止まるときの風提示の関数
    {
        WindSender.DataSend(風を起こす命令);
        yield return new WaitForSeconds(風を起こす時間); // 一定時間風を起こす
        WindSender.DataSend(風をやませる命令);
    }

    public IEnumerator DisappearWind() // 鷹が飛び立つときの風提示の関数
    {
        WindSender.DataSend(風を起こす命令);
        yield return new WaitForSeconds(風を起こす時間); // 一定時間風を起こす
        WindSender.DataSend(風をやませる命令);
    }
    */
}
