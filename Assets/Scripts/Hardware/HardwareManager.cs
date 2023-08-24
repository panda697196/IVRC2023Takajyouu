using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardwareManager : MonoBehaviour
{
    [SerializeField] private Sender WeightSender;
    [SerializeField] private Sender RopeSender;
    [SerializeField] private Sender PressSender;
    [SerializeField] private Sender WindSender;
    // モータの回転速度(0-255)，接頭語はモータの種類，接尾語は順転か逆転か
    const int _RopeSpeedC = 100;
    const int _RopeSpeedR = 100;
    const int _WeightSpeedC = 100;
    const int _WeightSpeedR = 100;
    // モータを止めるまでの時間[s]，接頭語はモータの種類，接尾語は順転か逆転か
    const float _RopeTimeC = 5;
    const float _RopeTimeR = 5;
    const float _WeightTimeC = 5;
    const float _WeightTimeR = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
 
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
        RopeSender.DataSend("C\n" + _RopeSpeedC.ToString() + "\n"); // 紐を張る
        yield return new WaitUntil(() => RopeSender._afterstop == true); // 紐が張ったことを確認できるまで待機
        WeightSender.DataSend("C\n" + _WeightSpeedC.ToString() + "\n"); // 重りを落とし始める
        yield return new WaitForSeconds(_WeightTimeC);
        WeightSender.DataSend("S\n"); // 重りが落ちたらモータを止める
        RopeSender.DataSend("R\n" + _RopeSpeedR.ToString() + "\n"); // 紐を緩める
        yield return new WaitForSeconds(_RopeTimeR);
        RopeSender.DataSend("S\n");
    }

    public IEnumerator DisappearShock() // 鷹が飛び立つときの衝撃提示の関数
    {
        WeightSender.DataSend("R\n" + _WeightSpeedR.ToString() + "\n");
        yield return new WaitForSeconds(_WeightTimeR);
        WeightSender.DataSend("S\n");
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
