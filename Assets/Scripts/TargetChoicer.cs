using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetChoicer : MonoBehaviour
{
    [SerializeField] private RaycastToPlane _raycastTo1stPlane;
    [SerializeField] private GameObject _objectOf1stThrow;
    
    [SerializeField] private RaycastToPlane _raycastTo2ndPlane;
    [SerializeField] private GameObject _objectOf2ndThrow;

    private Vector3 _target;
    
    // Start is called before the first frame update
    void Start()
    {
        _objectOf1stThrow.SetActive(false);
        _objectOf2ndThrow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OffTargetChoicePlane()
    {
        _objectOf1stThrow.SetActive(false);
        _objectOf2ndThrow.SetActive(false);
    }

    public void On1stTarget()
    {
        _objectOf1stThrow.SetActive(true);
    }
    public void On2ndTarget()
    {
        _objectOf2ndThrow.SetActive(true);
    }

    public Vector3 SetTarget()
    {
        return _target;
    }

    public void DecideTarget()
    {
        if (_objectOf1stThrow)
            _target = _raycastTo1stPlane.GetTarget();
        else
            _target = _raycastTo2ndPlane.GetTarget();
    }
}
