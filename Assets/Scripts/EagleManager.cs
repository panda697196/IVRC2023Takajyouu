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
    [SerializeField] private Vector3 _targetOfset;
    private Quaternion _handTargetInitialRotation;

    [SerializeField] private GameObject _tracker;
    [SerializeField] private GameObject _debug;

    [SerializeField] private int _crowCount;

    public int GetSetCrowCount
    {
        get
        {
            return _crowCount;
            
        }
        set
        {
            _crowCount = value;
        }
    }
    
    void Start()
    {
        
        _edit = gameObject.GetComponent<Eagle_Edit>();
        _navi = gameObject.GetComponent<Eagle_Navigation>();
        _navi.SetHandPosition(_handTargetPosition);
    }

    // Update is called once per frame
    void Update()
    {
        TargetProject(_tracker.transform);
        if (Input.GetKeyDown(KeyCode.A))
        {

            EagleTarget2Around(_target);
            
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            _navi.SetFlyState(Eagle_Navigation.FlyState.targetAround); 
        }
        
        // if (Input.GetKeyDown(KeyCode.D))
        // {            
        //     // ForTargetChanger._isStart = true;//あとでけす
        //     _navi.SetTarget(_target);
        //     _edit.SetEagleState(Eagle_Edit.EagleState.Takeoff);
        //     _navi.SetFlyState(Eagle_Navigation.FlyState.onlyTarget); 
        // }
        if (Input.GetKeyDown(KeyCode.X))
        {
            _navi.SetTarget(_handTargetPosition);
            _navi.SetFlyState(Eagle_Navigation.FlyState.getOnArm); 
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

    public bool IsEagleHandLauding()
    {
        
        if (_edit.GetEagleCurrentAnimState().ToString() == "Landing")
        {
            return true;
        }
        return false;
    }

    public bool IsEagleTakeOff()
    {
        if (_edit.GetEagleCurrentAnimState().ToString() == "Takeoff")
        {
            return true;
        }

        return false;
    }

    private void TargetProject(Transform tracker)
    {
        var pos = Vector3.ProjectOnPlane(-tracker.forward, Vector3.up);
        pos = pos.normalized * 1.92f;
        pos += tracker.position;
        pos.y += 0.82f;
        _debug.transform.position = pos;
    }
}
