using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class  lb_CrowTrigger: MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject _Crow;
    private lb_Crow _lbCrow;
    private bool _isOnceTarget =true;
    private bool _isCrowFly;
    //private Transform newTarget ;
    private GameObject newTarget;
    private GameObject _crowTargetBox;
    private bool _isEagleScared;
    public bool IsEagleScared => _isEagleScared;
    //public CrowCount _crowCount;
   
    void Start()
    {
      
        //カラスの親オブジェクトを取得　これはスクリプトに触るために必要
        _Crow = transform.parent.gameObject;
        //カラスのスクリプトを取得
        _lbCrow = _Crow.GetComponent<lb_Crow>();
        //カラスの移動先となるオブジェクトを生成
        newTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //移動先オブジェクトのコライダーとメッシュレンダラーをオフに
        newTarget.GetComponent<BoxCollider>().enabled=false;
        newTarget.GetComponent<MeshRenderer>().enabled = false;
        //カラスの移動先を格納するBoxを検索
        _crowTargetBox=GameObject.Find("CrowTargetBox");
        //移動先オブジェクトをBoxに格納
        newTarget.transform.parent = _crowTargetBox.transform;
        //_crowCount = GetComponent<CrowCount>();

    }

    // Update is called once per frame
    void Update()
    {
        if (_isCrowFly)
        {
            _lbCrow.SetCrowState(lb_Crow.birdBehaviors.flyToTarget);
            _lbCrow.Flytest(newTarget.transform);
            
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        //ぶつかったオブジェクトのタグがイーグルなら鷹の逆方向に飛ぶ立つ
        if (col.tag == "eagle")
        {
            //鷹で飛んだことを示すブール
            _isEagleScared = true;

            if (_isOnceTarget)
            {
                _isOnceTarget = false;
                //var newTargetX=
                //鷹2カラスのベクトルを計算
                Vector2 eagle2crow =new Vector2 (_lbCrow.transform.position.x,_lbCrow.transform.position.z)
                                    -new Vector2(col.transform.position.x,col.transform.position.z);
                // カラス座標を通る鷹2カラスのベクトルの法線ベクトルを計算
                Vector2 normalCrow = new Vector2(eagle2crow.x, -eagle2crow.y);
                
                //カラスの位置を通る任意の長さの法線ベクトルを利用し，目標となるターゲット地点を作成
                Vector2 moveCrow = new Vector2(_lbCrow.transform.position.x, _lbCrow.transform.position.y) +
                                   Random.Range(-20f, 20f) * normalCrow; 
                Vector2 newTargetxz = moveCrow + Random.Range(0.1f, 10f) * eagle2crow;
                //適当に高さを増加 高さは適当です
                float newTargety = _lbCrow.transform.position.y + Random.Range(5.0f, 14f);
                //3次元データに変換
                newTarget.transform.position = 100*new Vector3(newTargetxz.x,newTargety, newTargetxz.y);
               //計算した目的地をカラスのスクリプトに渡す
                _lbCrow.SetTarget(newTarget);
                //_crowCount.CountUp();
                //鷹のイーグルマネージャーにアクセス
                var eagleM = col.gameObject.GetComponent<EagleManager>();
                eagleM.GetSetCrowCount+=1;
            }
            //カラスの移動フラグをオン
            _isCrowFly=true;
        }
    }
    
}
