using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreGene : MonoBehaviour
{
    private GameObject _crow;
    private GameObject _randomTarget;
    
    //debug用
    //private GameObject _sphere;

    public GameObject _crowStorage;
    public GameObject _targetStorage;
    public int _crowMaxNumber;
    public int _crowMinNumber;
    public Vector3 _spwanCenter;

    private List<GameObject> _crowList = new List<GameObject>(1);
    private List<GameObject> _randomTargetList = new List<GameObject>(1);

    //出現可能の場所候補
    [SerializeField] private float radius;
    [SerializeField] private GameObject _scoreArea; //カラスを出現させるエリア
    [SerializeField] private GameObject lookObject; // 注視したいオブジェクトをInspectorから入れておく

    private Vector3 _areaSize;
    private Vector3 _offset;
    private float _areaMin = -0.5f;
    private float _areaMax = 0.5f;

    

    void RandomCirclePos(int a)
    {
        for (int i = 0; i <= a; i++)
        {
            /*float randomRangeX = Random.Range(_areaMin, _areaMax);
            float randomRangeY = Random.Range(_areaMin, _areaMax);
            float randomRangeZ = Random.Range(_areaMin, _areaMax);
            float xPos = randomRangeX * _areaSize.x;
            float yPos = randomRangeY * _areaSize.y;
            float zPos = randomRangeZ * _areaSize.z;
            Vector3 position = new Vector3(xPos, yPos, zPos) + _offset;*/

            // 指定された半径の円内のランダム位置
            var circlePos = radius * Random.insideUnitCircle;
            // XZ平面で指定された半径、中心点の円内のランダム位置を計算
            var spawnPos = new Vector3(circlePos.x, 0, circlePos.y) + _spwanCenter;
            // ターゲット方向のベクトルを取得
            Vector3 relativePos = lookObject.transform.position - spawnPos;
            // Prefabをインスタンス化する
            GameObject newCrow = Instantiate(_crow, spawnPos, Quaternion.LookRotation (relativePos));
            lb_Crow lbCrow = newCrow.GetComponent<lb_Crow>();
            //見やすいように生成したカラスをCrowStorageに格納
            newCrow.transform.parent = _crowStorage.transform;
            //生成したカラスをリストに追加
            _crowList.Add(newCrow);
            lbCrow.SetTargetList(_randomTargetList);
            lbCrow.SetCrowState(lb_Crow.birdBehaviors.sing);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //カラスのPregfabの読み取り
        _crow = (GameObject)Resources.Load("lb_crow_target");
        //TargetのPregfabの読み取り
        _randomTarget = (GameObject)Resources.Load("Sphere");
        //カラスのPregfabから生成したオブジェの格納
        _crowStorage = GameObject.Find("CrowStorage");
        //TragetのPregfabから生成したオブジェの格納
        _targetStorage = GameObject.Find("TargetStorage");
        _areaSize = _scoreArea.transform.localScale;
        _offset = _scoreArea.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            CrowGenerator();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            int flyCrow=ScaredCrowNumber();

            Debug.Log("飛んだカラス"+flyCrow);
        }
        
    }

    //カラスリスト内にある，鷹によって飛んだカラスを数えるメソッド
    public int ScaredCrowNumber()
    {
        int count = 0;
        foreach(GameObject crow in _crowList)
        {
            if (crow.transform.GetChild(2).GetComponent<lb_CrowTrigger>().IsEagleScared)
            {
                count++;
            }
        }

        return count;
    }

    //カラスを_crowMaxNumberまで生成するメソッド　スポーンはCenterの位置を中心に正方形に生成
    public void CrowGenerator()
    {
        RandomCirclePos(_crowMaxNumber);
    }
}
