using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.UIElements;
using static lb_Crow;
using static UnityEngine.GraphicsBuffer;

public class CrowGenerater : MonoBehaviour
{
    private GameObject _crow;
    private GameObject _randomTarget;
    //debug用
    //private GameObject _sphere;

    public GameObject _crowStorage;
    public GameObject _targetStorage;
    public int _crowMaxNumber;
    public int _crowMinNumber;

    private List<GameObject> _crowList = new List<GameObject>(1);
    private List<GameObject> _randomTargetList = new List<GameObject>(1);

    //出現可能の場所候補
    [SerializeField] private GameObject _flyArea;
    [SerializeField] private List<GameObject> _popUpPlaceList1;
    [SerializeField] private List<GameObject> _popUpPlaceList2;
    //重み付け
    [SerializeField] private int[] _weights;
    private int _totalWeight;
    //重みを含めたリスト
    private List<GameObject> _popUpPlaceList1W = new List<GameObject>(1);
    private List<GameObject> _popUpPlaceList2W = new List<GameObject>(1);

    [SerializeField] private int _scaredCrow;
    private List<GameObject> _backCrowList = new List<GameObject>(1);
    public int ScaredCrow => _scaredCrow;
    public List<GameObject> BackCrowList => _backCrowList;


    private int _chooseNum;
    private Vector3 _point1;
    private Vector3 _point2;
    private Vector3 _areaSize;
    private Vector3 _offset;
    private float _areaMin = -0.5f;
    private float _areaMax = 0.5f;

    void SumOfWeight()
    {
        // 重みの総和計算
        for (var i = 0; i < _weights.Length; i++)
        {
            _totalWeight += _weights[i]+1;
        }
    }

    void HaveWeightLsit()
    {
        int j = 0;
        for (int i = 0; i< _weights.Length; i++)
        {
            int s = _weights[i];
            for(int t = 0; t <= s; t++)
            {
                _popUpPlaceList1W.Add(_popUpPlaceList1[i]);
                _popUpPlaceList2W.Add(_popUpPlaceList2[i]);
                j++;
            }
        }
    }

    void RandomIdle()
    {
        //カラスの出現場所のリストの番号をランダムで選択
        _chooseNum = Random.Range(0, _totalWeight - 1);
        UnityEngine.Debug.Log(_popUpPlaceList1W[_chooseNum].name);
        _point1 = _popUpPlaceList1W[_chooseNum].transform.position;
        _point2 = _popUpPlaceList2W[_chooseNum].transform.position;
        Vector3 popLine = _point1 - _point2;
        float r = Random.Range(0, 1.0f);
        //Debug用
        /*GameObject newSphere = Instantiate(_sphere, (_point2 + popLine * r), Quaternion.Euler(0, Random.Range(0, 180), 0));
        GameObject newCrow = Instantiate(_crow, newSphere.transform.position, newSphere.transform.rotation);*/
        //カラスをインスタンス生成
        GameObject newCrow = Instantiate(_crow, (_point2 + popLine * r), Quaternion.Euler(0, Random.Range(0, 180), 0));
        lb_Crow lbCrow = newCrow.GetComponent<lb_Crow>();
        lbCrow._idleAgitated = r;
        //見やすいように生成したカラスをCrowStorageに格納
        newCrow.transform.parent = _crowStorage.transform;
        //生成したカラスをリストに追加
        _crowList.Add(newCrow);
    }

    void RandomPositionCrow(int a)
    {
        float randomRangeX = Random.Range(_areaMin, _areaMax);
        float randomRangeY = Random.Range(_areaMin, _areaMax);
        float randomRangeZ = Random.Range(_areaMin, _areaMax);
        float xPos = randomRangeX * _areaSize.x;
        float yPos = randomRangeY * _areaSize.y;
        float zPos = randomRangeZ * _areaSize.z;
        Vector3 position = new Vector3(xPos, yPos, zPos) + _offset;
        // Prefabをインスタンス化する
        GameObject newCrow = Instantiate(_crow, position, Quaternion.identity);
        lb_Crow lbCrow = newCrow.GetComponent<lb_Crow>();
        //見やすいように生成したカラスをCrowStorageに格納
        newCrow.transform.parent = _crowStorage.transform;
        //生成したカラスをリストに追加
        _crowList.Add(newCrow);
        lbCrow.SetTargetList(_randomTargetList);
        lbCrow.SetCrowState(birdBehaviors.randomFly);
        for (int i = 1; i < a; i++)
        {
            float randomRangeXn = Random.Range(_areaMin, _areaMax);
            float randomRangeYn = Random.Range(_areaMin, _areaMax);
            float randomRangeZn = Random.Range(_areaMin, _areaMax);
            float xnPos = randomRangeXn * _areaSize.x;
            float ynPos = randomRangeYn * _areaSize.y;
            float znPos = randomRangeZn * _areaSize.z;

            Vector3 positionN = new Vector3(xnPos, ynPos, znPos) + _offset;
            // Prefabをインスタンス化する
            GameObject newCrowN = Instantiate(_crow, positionN, Quaternion.identity);
            lb_Crow lbCrowN = newCrowN.GetComponent<lb_Crow>();
            //見やすいように生成したカラスをCrowStorageに格納
            newCrowN.transform.parent = _crowStorage.transform;
            //生成したカラスをリストに追加
            _crowList.Add(newCrowN);
            lbCrowN.SetTarget(newCrow);
            lbCrowN.SetCrowState(birdBehaviors.flyToTarget2);
        }
    }

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
            // Prefabをインスタンス化する
            GameObject newTarget = Instantiate(_randomTarget, position, Quaternion.identity);
            //見やすいように生成したカラスをCrowStorageに格納
            newTarget.transform.parent = _targetStorage.transform;
            //生成したターゲットをリストに追加
            _randomTargetList.Add(newTarget);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _crow = (GameObject)Resources.Load("lb_crow_target");
        _randomTarget = (GameObject)Resources.Load("Sphere");
        _crowStorage =GameObject.Find("CrowStorage");
        _targetStorage = GameObject.Find("TargetStorage");
        SumOfWeight();
        HaveWeightLsit();
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
            ScaredCrowNumber();
        }
        
    }

    //カラスリスト内にある，鷹によって飛んだカラスを数えるメソッド
    public void ScaredCrowNumber()
    {
        int i = 0;
        int count = 0;
        foreach(GameObject crow in _crowList)
        {
            if (crow.transform.GetChild(2).GetComponent<lb_CrowTrigger>().IsEagleScared)
            {
                count++; //鷹によって飛んだカラスを数える
            }
            else
            {
                _backCrowList.Add(_crowList[i]); //飛ばなかった帰ってくるカラスを格納
            }
            i++;
        }
    }

    //カラスを_crowMaxNumberまで生成するメソッド　スポーンはCenterの位置を中心に正方形に生成
    public void CrowGenerator()
    {
        int num = 8 * _crowMaxNumber /10;
        RandomPositionTarget(5);
        for (int i = 0; i < _crowMaxNumber; i++)
        {
            if (i > num)
            {
                RandomIdle();
            }
            else
            {
                //int j = Random.Range(5,10);
                RandomPositionCrow(1);
                //i += j-1;
            }
        }
    }
}
