using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EagleManager : MonoBehaviour
{
    // Start is called before the first frame update
    private Eagle_Navigation _navi;
    private Eagle_Edit _edit;
    public GameObject _target;
    public GameObject _userHand;
    private bool _isHardOK =false;
    private GameObject _handTargetPosition;
    public GameObject GetHandTargetPosition =>_handTargetPosition;
    void Start()
    {
        _edit = gameObject.GetComponent<Eagle_Edit>();
        _navi = gameObject.GetComponent<Eagle_Navigation>();
        //カラスの移動先となるオブジェクトを生成
         _handTargetPosition = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // //移動先オブジェクトのコライダーとメッシュレンダラーをオフに
         _handTargetPosition.GetComponent<BoxCollider>().enabled=false;
         //_handTargetPosition.GetComponent<MeshRenderer>().enabled = false;
        // //生成してオブジェクトをまず手の位置と同期
         _handTargetPosition.transform.position = _userHand.transform.position;
        // //生成したオブジェクトを手のターゲット位置に配置
         _handTargetPosition.transform.parent = _userHand.transform;   
        // //ターゲット位置をローカル座標をずらして設定
         _handTargetPosition.transform.Translate(0,1,1.5f);
        //
        // _navi.SetHandPosition(_handTargetPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EagleTarget2Around(_target);
            
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            _navi.SetFlyState(Eagle_Navigation.FlyState.targetAround); 
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            _navi.SetFlyState(Eagle_Navigation.FlyState.laud); 
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            _navi.SetTarget(_handTargetPosition);
            _navi.SetFlyState(Eagle_Navigation.FlyState.getOnArm); 
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            _isHardOK = true;
        }
        

        if (Input.GetKeyDown(KeyCode.S))
        {
            EagleAround2GetOn(_isHardOK);
        }

    }
    public void EagleTarget2Around(GameObject target)
    {
        _navi.SetTarget(_target);
        _navi.SetFlyState(Eagle_Navigation.FlyState.target);
        _edit.SetEagleState(Eagle_Edit.EagleState.Takeoff);
       
    }

    public void EagleAround2GetOn(bool hardok)
    {
        if (hardok)
        {
            _navi._isOn=true;
        }
    }
}
