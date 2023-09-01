using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class CrowGenerater : MonoBehaviour
{
    private GameObject _crow;

    public GameObject _crowStorage;
    public int _crowMaxNumber;
    public int _crowMinNumber;
    
    private List<GameObject> _crowList=new List<GameObject>(1);

    public Vector3 _SpwanCenter;
    // Start is called before the first frame update
    void Start()
    {
         _crow = (GameObject)Resources.Load("lb_crow_target");
         _crowStorage=GameObject.Find("CrowStorage");
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
            //カラスをインスタンス生成
            GameObject newCrow=Instantiate(_crow, new Vector3(Random.Range(-5f,5f)+_SpwanCenter.x,0.0f,Random.Range(-5f,5f)+_SpwanCenter.z), Quaternion.identity);
            //見やすいように生成したカラスをCrowStorageに格納
            newCrow.transform.parent = _crowStorage.transform;
            //生成したカラスをリストに追加
            _crowList.Add(newCrow);
        }
    }
}
