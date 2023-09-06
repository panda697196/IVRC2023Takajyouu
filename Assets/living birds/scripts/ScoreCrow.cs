using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCrow : MonoBehaviour
{
    private GameObject _randomScoreTarget;

    public GameObject _randomScoreTargetStorage;
    public Vector3 _spwanCenter;

    private List<GameObject> _backCrowList = new List<GameObject>(1);
    private List<GameObject> _randomScoreTargetList = new List<GameObject>(1);

    //出現可能の場所候補
    [SerializeField] private float radius; //カラスが出現するエリアの半径
    [SerializeField] private int _comebackCrow; //帰って来るカラスの数
    [SerializeField] private GameObject lookObject; //カラスの向く向きを決めるオブジェクト
    private GameObject CrowManager;
    CrowGenerater Crowgene;



    void RandomCirclePos(int a)
    {
        for (int i = 0; i < a; i++)
        {
            
            // 指定された半径の円内のランダム位置
            var circlePos = radius * Random.insideUnitCircle;
            // XZ平面で指定された半径、中心点の円内のランダム位置を計算
            var spawnPos = new Vector3(circlePos.x, 0, circlePos.y) + _spwanCenter;
            // ターゲット方向のベクトルを取得
            Vector3 relativePos = lookObject.transform.position - spawnPos;
            // Prefabをインスタンス化する
            GameObject newTarget = Instantiate(_randomScoreTarget, spawnPos, Quaternion.LookRotation(relativePos));
            //見やすいように生成したTargetをRandomScoreTargetStorageに格納
            newTarget.transform.parent = _randomScoreTargetStorage.transform;
            //生成したTargetをリストに追加
            _randomScoreTargetList.Add(newTarget);
            lb_Crow lbCrow = _backCrowList[i].GetComponent<lb_Crow>();
            lbCrow.SetTargetList(_randomScoreTargetList);
            lbCrow.SetCrowState(lb_Crow.birdBehaviors.flyToTarget);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //TargetのPregfabの読み取り
        _randomScoreTarget = (GameObject)Resources.Load("Sphere");
        //TragetのPregfabから生成したオブジェの格納
        _randomScoreTargetStorage = GameObject.Find("RandomScoreTargetStorage");
        CrowManager = GameObject.Find("CrowManager");
        lb_CrowTrigger lbTrigger = new lb_CrowTrigger();
        Crowgene = CrowManager.GetComponent<CrowGenerater>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ScaredCrowNumber();
            ScoreCrowPos();
        }

    }

    //カラスリスト内にある，鷹によって飛んだカラスを数えるメソッド
    public void ScaredCrowNumber()
    {
        int i = 0;
        int count = 0;
        List<GameObject> _crowList = Crowgene.CrowList;
        foreach (GameObject crow in _crowList)
        {
            if (crow.transform.GetChild(2).GetComponent<lb_CrowTrigger>().IsEagleScared)
            {
                count++; //鷹によって飛んだカラスを数える
            }
            else
            {
                _backCrowList.Add(_crowList[i]); //追い払えなかったカラスを格納
            }
            i++;
        }
        _comebackCrow = _backCrowList.Count;
    }

    //カラスを_crowMaxNumberまで生成するメソッド　スポーンはCenterの位置を中心に正方形に生成
    public void ScoreCrowPos()
    {
        RandomCirclePos(_comebackCrow);
    }
}
