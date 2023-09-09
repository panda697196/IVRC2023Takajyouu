using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static lb_Crow;
using static UnityEngine.GraphicsBuffer;

public class CrowGenerater : MonoBehaviour
{
    private GameObject _crow;
    private GameObject _randomStayTarget;
    private GameObject _randomMoveTarget;
    //debug用
    //private GameObject _sphere;

    public GameObject _crowStorage;
    public GameObject _targetStorage;

    private List<GameObject> _crowList = new List<GameObject>(1);
    public List<GameObject> CrowList => _crowList;

    private List<GameObject> _randomTargetList = new List<GameObject>(1);


    [SerializeField] private CrowCount _crowCount;

    //出現可能の場所候補
    [SerializeField] private GameObject _flyArea;
    [SerializeField] private int _flyAreaTarget = 0;
    [SerializeField] private int _crowNum1;
    [SerializeField] private int _crowNum2;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private float _radiusMax; //カラスが出現するエリアの最大半径
    [SerializeField] private float _radiusMin; //カラスが出現するエリアの最小半径
    [SerializeField] private List<GameObject> _popUpPlaceList1;
    [SerializeField] private List<GameObject> _popUpPlaceList2;

    //重み付け
    [SerializeField] private int[] _weights;
    private int _totalWeight;
    //重みを含めたリスト
    private List<GameObject> _popUpPlaceList1W = new List<GameObject>(1);
    private List<GameObject> _popUpPlaceList2W = new List<GameObject>(1);

    private int _chooseNum;
    private Vector3 _point1;
    private Vector3 _point2;
    private Vector3 _areaSize;
    private Vector3 _offset;
    private float _areaMin = -0.5f;
    private float _areaMax = 0.5f;
    AudioSource _crowSound;
    private bool _isFadeOut = true;
    private float _fadingTime;
    private int _count;
    private float _crowSoundlevel;



    void SumOfWeight()
    {
        _totalWeight = 0;
        // 重みの総和計算
        for (var i = 0; i < _weights.Length; i++)
        {
            _totalWeight += _weights[i]+1;
        }
    }

    void HaveWeightLsit()
    {
        _popUpPlaceList1W.Clear();
        _popUpPlaceList2W.Clear();
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

    /*void RandomPopIdleCrow()
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
        GameObject newCrow = Instantiate(_crow, newSphere.transform.position, newSphere.transform.rotation);
        //カラスをインスタンス生成
        GameObject newCrow = Instantiate(_crow, (_point2 + popLine * r), Quaternion.Euler(0, Random.Range(0, 180), 0));
        lb_Crow lbCrow = newCrow.GetComponent<lb_Crow>();
        lbCrow._idleAgitated = r;
        //見やすいように生成したカラスをCrowStorageに格納
        newCrow.transform.parent = _crowStorage.transform;
        //生成したカラスをリストに追加
        _crowList.Add(newCrow);
    }

    

    /*void RandomPopFlyCrow()
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

        //生成した1羽のカラスについていく子カラスたち
        /*for (int i = 1; i < a; i++)
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
    }*/

    void RandomFlyToPopIdleCrow()
    {
        var radius = Random.Range(_radiusMin, _radiusMax);
        var angle = Random.Range(0, 360);
        var rad = angle * Mathf.Deg2Rad;
        var px = Mathf.Cos(rad) * radius;
        var py = Mathf.Sin(rad) * radius;
        var spawnPos = new Vector3(px, 0, py) + _offset;
        // Prefabをインスタンス化する
        GameObject newCrow = Instantiate(_crow, spawnPos, Quaternion.identity);
        lb_Crow lbCrow = newCrow.GetComponent<lb_Crow>();
        //見やすいように生成したカラスをCrowStorageに格納
        newCrow.transform.parent = _crowStorage.transform;
        //生成したカラスをリストに追加
        _crowList.Add(newCrow);

        //カラスの出現場所のリストの番号をランダムで選択
        _chooseNum = Random.Range(0, _totalWeight);
        Debug.Log(_popUpPlaceList1W[_chooseNum].name);
        _point1 = _popUpPlaceList1W[_chooseNum].transform.position;
        _point2 = _popUpPlaceList2W[_chooseNum].transform.position;
        Vector3 popLine = _point1 - _point2;
        float r = Random.Range(0, 1.0f);
        //Debug用
        /*GameObject newSphere = Instantiate(_sphere, (_point2 + popLine * r), Quaternion.Euler(0, Random.Range(0, 180), 0));
        GameObject newCrow = Instantiate(_crow, newSphere.transform.position, newSphere.transform.rotation);*/
        //カラスをインスタンス生成
        GameObject newTarget = Instantiate(_randomStayTarget, (_point2 + popLine * r), Quaternion.Euler(0, Random.Range(0, 180), 0));
        lbCrow._idleAgitated = r;//Idle時の動作を差を付けるため
        //見やすいように生成したTargetをTargetStorageに格納
        newTarget.transform.parent = _targetStorage.transform;
        lbCrow.SetTarget(newTarget);
        lbCrow.SetCrowState(birdBehaviors.flyToTarget);
        BoxCollider crowCollider = newCrow.GetComponent<BoxCollider>();
        lb_CrowTrigger trigger = newCrow.GetComponentInChildren<lb_CrowTrigger>();
        trigger._crowCount = _crowCount;
        crowCollider.enabled = false;
    }
    void RandomFlyToPopFlyCrow()
    {
        var radius = Random.Range(_radiusMin, _radiusMax);
        var angle = Random.Range(0, 360);
        var rad = angle * Mathf.Deg2Rad;
        var px = Mathf.Cos(rad) * radius;
        var py = Mathf.Sin(rad) * radius;
        var spawnPos = new Vector3(px, 0, py) + _offset;
        // Prefabをインスタンス化する
        GameObject newCrow = Instantiate(_crow, spawnPos, Quaternion.identity);
        lb_Crow lbCrow = newCrow.GetComponent<lb_Crow>();
        BoxCollider crowCollider= newCrow.GetComponent<BoxCollider>();
        crowCollider.enabled = false;
        //見やすいように生成したカラスをCrowStorageに格納
        newCrow.transform.parent = _crowStorage.transform;
        //生成したカラスをリストに追加
        _crowList.Add(newCrow);
        lb_CrowTrigger trigger = newCrow.GetComponentInChildren<lb_CrowTrigger>();
        trigger._crowCount = _crowCount;
        lbCrow.SetTargetList(_randomTargetList);
        lbCrow.SetCrowState(birdBehaviors.randomFly);
        

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
            GameObject newTarget = Instantiate(_randomMoveTarget, position, Quaternion.identity);
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
        _randomStayTarget = (GameObject)Resources.Load("StaySphere");
        _randomMoveTarget = (GameObject)Resources.Load("MoveSphere");
        _crowStorage =GameObject.Find("CrowStorage");
        _targetStorage = GameObject.Find("TargetStorage");
        _crowSound = GetComponent<AudioSource>();
        _crowSound.Stop();
        
        _areaSize = _flyArea.transform.localScale;
        _offset = _flyArea.transform.position;
    }

    void Update()//Debug用
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CrowGenerator1();
            _fadingTime = _fadeDuration;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            CrowGenerator2();
            _fadingTime = _fadeDuration;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            DestoryCrowAndTarget();
        }
        _count = _crowCount.Count;
        //_isFadeOut = false;
        _crowSound.volume = _crowSoundlevel - 1f / (float)_crowNum2 * (float)_count;
        

        /*if (_isFadeOut)
        {
            _fadingTime -= Time.deltaTime;
            _crowSound.volume = _fadingTime / _fadeDuration;
        }
        if (_fadingTime <= 0)
        {
            _isFadeOut = false;
        }*/
    }

    //カラスを_crowMaxNumberまで生成するメソッド　スポーンはCenterの位置を中心に正方形に生成
    public void CrowGenerator1()
    {
        _popUpPlaceList1W.Add(_popUpPlaceList1[0]);
        _popUpPlaceList2W.Add(_popUpPlaceList2[0]);
        _popUpPlaceList1W.Add(_popUpPlaceList1[46]);
        _popUpPlaceList2W.Add(_popUpPlaceList2[46]);
        _totalWeight = _popUpPlaceList1W.Count;
        for(int i = 0; i<_crowNum1; i++)
        {
            RandomFlyToPopIdleCrow();
        }
        //_isFadeOut = false;
        //AudioSource crowSound = GetComponent<AudioSource>();
        _crowSound.volume = 1f / (float)_crowNum2 * (float)_crowNum1;
        _crowSoundlevel = _crowSound.volume;
        _crowSound.Play();
    }

    public void CrowGenerator2()
    {
        _crowCount.CountReset();
        SumOfWeight();
        HaveWeightLsit();
        int num = 8 * _crowNum2 / 10;
        RandomPositionTarget(_flyAreaTarget);
        for (int i = 0; i < _crowNum2; i++)
        {
            if (i < num)
            {
                RandomFlyToPopFlyCrow();
            }
            else
            {
                RandomFlyToPopIdleCrow();
            }
        }
        //_isFadeOut = false;
        _crowSound.volume = 1f / (float)_crowNum2 * (float)_crowNum2;
        _crowSoundlevel = _crowSound.volume;
        _crowSound.Play();
    }

    public void DestoryCrowAndTarget()
    {
        _crowList.Clear();
        _randomTargetList.Clear();
        foreach (Transform child in _crowStorage.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _targetStorage.transform)
        {
            Destroy(child.gameObject);
        }
        //_isFadeOut = true;
        _crowSound.Stop();
    }

}
