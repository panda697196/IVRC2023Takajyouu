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
    public Vector3 _SpwanCenter;

    private List<GameObject> _crowList = new List<GameObject>(1);
    private List<GameObject> _randomTargetList = new List<GameObject>(1);

    //出現可能の場所候補
    [SerializeField] private GameObject _flyArea;
    [SerializeField] private GameObject lookObject; // 注視したいオブジェクトをInspectorから入れておく

    private int _chooseNum;
    private Vector3 _point1;
    private Vector3 _point2;
    private Vector3 _areaSize;
    private Vector3 _offset;
    private float _areaMin = -0.5f;
    private float _areaMax = 0.5f;

    

    void RandomPositionTarget(int b)
    {
        for (int i = 0; i <= b; i++)
        {
            float randomRangeX = Random.Range(_areaMin, _areaMax);
            float randomRangeY = Random.Range(_areaMin, _areaMax);
            float randomRangeZ = Random.Range(_areaMin, _areaMax);
            float xPos = randomRangeX * _areaSize.x;
            float yPos = randomRangeY * _areaSize.y;
            float zPos = randomRangeZ * _areaSize.z;
            Vector3 position = new Vector3(xPos, yPos, zPos) + _offset;
            // ターゲット方向のベクトルを取得
            Vector3 relativePos = lookObject.transform.position - position;
            // Prefabをインスタンス化する
            GameObject newCrow = Instantiate(_crow, position, Quaternion.LookRotation (relativePos));
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
        _crow = (GameObject)Resources.Load("lb_crow_target");
        _randomTarget = (GameObject)Resources.Load("Sphere");
        _crowStorage =GameObject.Find("CrowStorage");
        _targetStorage = GameObject.Find("TargetStorage");
        _areaSize = _flyArea.transform.localScale;
        _offset = _flyArea.transform.position;
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
        RandomPositionTarget(_crowMaxNumber);
    }
}
