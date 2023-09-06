using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCrow : MonoBehaviour
{
    private GameObject _randomScoreTarget;

    public GameObject _randomScoreTargetStorage;
    public Vector3 _spwanCenter;

    private List<GameObject> _crowList = new List<GameObject>(1);
    private List<GameObject> _randomScoreTargetList = new List<GameObject>(1);

    //出現可能の場所候補
    [SerializeField] private float radius; //カラスが出現するエリアの半径
    [SerializeField] private int _comebackCrow; //帰って来るカラスの数
    [SerializeField] private GameObject lookObject; //カラスの向く向きを決めるオブジェクト



    void RandomCirclePos(int a)
    {
        for (int i = 0; i <= a; i++)
        {
            
            // 指定された半径の円内のランダム位置
            var circlePos = radius * Random.insideUnitCircle;
            // XZ平面で指定された半径、中心点の円内のランダム位置を計算
            var spawnPos = new Vector3(circlePos.x, 0, circlePos.y) + _spwanCenter;
            // ターゲット方向のベクトルを取得
            Vector3 relativePos = lookObject.transform.position - spawnPos;
            // Prefabをインスタンス化する
            GameObject newTarget = Instantiate(_randomScoreTarget, spawnPos, Quaternion.LookRotation(relativePos));
            lb_Crow lbCrow = newTarget.GetComponent<lb_Crow>();
            //見やすいように生成したカラスをCrowStorageに格納
            newTarget.transform.parent = _randomScoreTarget.transform;
            //生成したカラスをリストに追加
            _randomScoreTargetList.Add(newTarget);
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
        lb_CrowTrigger lbTrigger = new lb_CrowTrigger();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {

            ScoreCrowPos();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            int flyCrow = ScaredCrowNumber();

            Debug.Log("飛んだカラス" + flyCrow);
        }

    }

    //カラスリスト内にある，鷹によって飛んだカラスを数えるメソッド
    public int ScaredCrowNumber()
    {
        int count = 0;
        foreach (GameObject crow in _crowList)
        {
            if (crow.transform.GetChild(2).GetComponent<lb_CrowTrigger>().IsEagleScared)
            {
                count++;
            }
        }

        return count;
    }

    //カラスを_crowMaxNumberまで生成するメソッド　スポーンはCenterの位置を中心に正方形に生成
    public void ScoreCrowPos()
    {
        RandomCirclePos(_comebackCrow);
    }
}
