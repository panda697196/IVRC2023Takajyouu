using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Eagle_Moving : MonoBehaviour
{
    [Header(("鷹をアタッチ"))] public GameObject _eagle;

    [SerializeField] private GameObject _target;

    public bool _isMove2Target;

    public GameObject _centerObject;

    public GameObject _rotationTargetObject;
    
    public float _theta;
    // 円の半径を設定します。
    public float radius = 10f;
    
    private Vector3 _beforePosi;

    private Vector3 _nowPosi;

    private bool _isStartTimer;
    // 初期位置を取得し、高さを保持します。
    Vector3 initPos;
    private int i;
    float Total_time = 0;
    public float fryTime = 0;
    public Transform _center;

    public bool _isLaudingAssist;

    public GameObject _laudingSpot;

    public Animator _eagleAnimator;

    public Eagle_Edit _eagleEdit;

    private List<GameObject> list;

    public float _targetDistance;
    [SerializeField] private float _duration = 1000;

    public bool _isZon;

    public bool _isfirst;

    public Vector3 _startPosition;
    public Vector3 _endPosition;
    // Start is called before the first frame update
    void Start()
    {
        _center = _centerObject.transform;
        // 初期位置を保持します。
        initPos = _center.position;

        _eagleAnimator = _eagle.GetComponent<Animator>();
        _eagleEdit = _eagle.GetComponent<Eagle_Edit>();
        list = GetAll (_eagle);
// 子オブジェクトを全て取得する
        
    }

    // Update is called once per frame
    void Update()
    {
     
        if (_isMove2Target)
        {
            _eagle.transform.LookAt(_target.transform);
        }

        if (_isLaudingAssist)
        {
            //_eagleAnimator.applyRootMotion=false; s
            _eagleEdit.Lauding();
            _eagle.transform.position = _laudingSpot.transform.position +  new Vector3(0,0.5f,0);
            _eagle.transform.rotation = Quaternion.Euler(0, 0, 0); 
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            _eagleAnimator.applyRootMotion=false; 
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _eagleAnimator.applyRootMotion=true; 
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            _eagleAnimator.applyRootMotion=false; 
            _eagle.transform.position = _laudingSpot.transform.position;
            foreach (GameObject obj in list)
            {
                obj.transform.position = _laudingSpot.transform.position;
            }
            _eagleAnimator.applyRootMotion=true; 
        }
        CalcRotationPosition();

        _targetDistance = (_laudingSpot.transform.position - _eagle.transform.position).magnitude;

        if (_targetDistance < 3.5)
        {
            //_eagleAnimator.applyRootMotion=true;
            _isLaudingAssist = true;
        }

        if (_isfirst &&_eagleAnimator.GetCurrentAnimatorStateInfo(0).IsName("fly"))
        {
            _isZon = true;
            _startPosition = _eagle.transform.position;
            _endPosition=_target.transform.position+new Vector3(0,2.0f,0);
        }
        if (_isZon &&_targetDistance > 3.5 &&_eagleAnimator.GetCurrentAnimatorStateInfo(0).IsName("fly"))
        {
            fryTime += Time.deltaTime;
           
            
            if (_isfirst)
            {
                _isfirst = false;
            }
            
            // 補間位置計算
        
            // 補間位置を反映
             //_eagleAnimator.applyRootMotion=false;
            float t = (fryTime / _duration);
            Debug.Log(t);
            transform.position = Vector3.Lerp(_startPosition, _endPosition, t);
             
        }

        
        

  
        // 始点・終点の位置取得

    
    }
    //以下はメソッドーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
    void CalcRotationPosition()
    {
        
        Total_time += Time.deltaTime;
        // 位相を計算します。
        var phase = (float)(Total_time * 3.905 / radius);
        
        // 現在の位置を計算します。
        float xPos = radius * Mathf.Cos(phase) ;
        float zPos = radius * Mathf.Sin(phase);

        // ゲームオブジェクトの位置を設定します。
        Vector3 pos = new Vector3(initPos.x+ xPos, initPos.y,initPos.z + zPos);
        _rotationTargetObject.transform.position = pos;
    }
    
    public static List<GameObject>  GetAll ( GameObject obj)
    {
        List<GameObject> allChildren = new List<GameObject> ();
        GetChildren (obj, ref allChildren);
        return allChildren;
    }

//子要素を取得してリストに追加
    public static void GetChildren (GameObject obj, ref List<GameObject> allChildren)
    {
        Transform children = obj.GetComponentInChildren<Transform> ();
        //子要素がいなければ終了
        if (children.childCount == 0) {
            return;
        }
        foreach (Transform ob in children) {
            allChildren.Add (ob.gameObject);
            GetChildren (ob.gameObject, ref allChildren);
        }
    }

    // public void RootMotionOnOff()
    // {
    //     _eagleAnimator.applyRootMotion = false;
    // }
}
