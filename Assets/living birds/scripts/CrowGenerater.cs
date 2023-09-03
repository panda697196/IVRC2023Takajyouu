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

    public GameObject _crowStorage;
    public int _crowMaxNumber;
    public int _crowMinNumber;
    public Vector3 _SpwanCenter;

    private List<GameObject> _crowList = new List<GameObject>(1);
    [SerializeField] private List<GameObject> _popUpPlaceList1;
    [SerializeField] private List<GameObject> _popUpPlaceList2;

    [SerializeField] private int[] _weights;
    private int _totalWeight;

    private List<GameObject> _popUpPlaceList1W = new List<GameObject>(1);
    private List<GameObject> _popUpPlaceList2W = new List<GameObject>(1);
    private int _chooseNum;
    private Vector3 _point1;
    private Vector3 _point2;

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

    // Start is called before the first frame update
    void Start()
    {
        _crow = (GameObject)Resources.Load("lb_crow_target");
        _crowStorage=GameObject.Find("CrowStorage");
        SumOfWeight();
        HaveWeightLsit();
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
        for (int i = 0; i < _crowMaxNumber; i++)
        {
            //カラスの出現場所のリストの番号をランダムで選択
            _chooseNum = Random.Range(0, _totalWeight-1);
            //UnityEngine.Debug.Log(_popUpPlaceList1W[_chooseNum].name);
            _point1 = _popUpPlaceList1W[_chooseNum].transform.position;
            _point2 = _popUpPlaceList2W[_chooseNum].transform.position;
            Vector3 popLine = _point1 - _point2;
            float r = Random.Range(0, 1.0f);
            //カラスをインスタンス生成
            GameObject newCrow=Instantiate(_crow, (_point2 + popLine * r), Quaternion.Euler(0, Random.Range(0, 180), 0));
            lb_Crow lbCrow = newCrow.GetComponent<lb_Crow>();
            lbCrow._idleAgitated = r;
            Debug.Log(lbCrow._idleAgitated);
            //見やすいように生成したカラスをCrowStorageに格納
            newCrow.transform.parent = _crowStorage.transform;
            //生成したカラスをリストに追加
            _crowList.Add(newCrow);
        }
    }
}
