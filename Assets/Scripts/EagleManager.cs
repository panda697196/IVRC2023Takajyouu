using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EagleManager : MonoBehaviour
{
    // [SerializeField] private ForTargetChanger ForTargetChanger;//あとでけす
    // Start is called before the first frame update
    private Eagle_Navigation _navi;
    private Eagle_Edit _edit;
    public GameObject _target;
    public GameObject _userHand;
    private bool _isHardOK =false;
    [SerializeField]private GameObject _handTargetPosition;
    public GameObject GetHandTargetPosition =>_handTargetPosition;
    private bool _isEagleArounding;
    void Start()
    {
        _edit = gameObject.GetComponent<Eagle_Edit>();
        _navi = gameObject.GetComponent<Eagle_Navigation>();
        //鷹が手に着地する際に移動先となるオブジェクトを生成
        //  _handTargetPosition = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // // //移動先オブジェクトのコライダーとメッシュレンダラーをオフに
        //  _handTargetPosition.GetComponent<BoxCollider>().enabled=false;
        //  //_handTargetPosition.GetComponent<MeshRenderer>().enabled = false;
        // // //生成してオブジェクトをまず手の位置と同期
        //  _handTargetPosition.transform.position = _userHand.transform.position;
        // // //生成したオブジェクトを手のターゲット位置に配置
        //  _handTargetPosition.transform.parent = _userHand.transform;   
        // // //ターゲット位置をローカル座標をずらして設定
        //  _handTargetPosition.transform.Translate(0,1,1.5f);
        //

        _navi.SetHandPosition(_handTargetPosition);
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
            // ForTargetChanger._isStart = true;//あとでけす
            _navi.SetTarget(_target);
            _edit.SetEagleState(Eagle_Edit.EagleState.Takeoff);
            _navi.SetFlyState(Eagle_Navigation.FlyState.onlyTarget); 
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
            StartGetOnHand();
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
            _navi.SetIsHardGetOnStandby(true);
        }
    }

    public bool EagleOnHand()
    {
        return _navi.GetIsOnHand;
    }

    public bool IsEagleAround()
    {
        return _navi.IsRotating();
    }

    public void StartGetOnHand()
    {
        _navi.SetTarget(_handTargetPosition);
        _edit.TakeOff();
        _navi.SetFlyState(Eagle_Navigation.FlyState.getOnArm);
    }
}
