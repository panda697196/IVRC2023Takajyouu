using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullInspector : MonoBehaviour
{
    [SerializeField] private float _timeThreshold = 0.3f;
    private bool _isPull = false;

    private float _time = 0;

    // Update is called once per frame
    void Update()
    {
        if (_time > 0)
            _time -= Time.deltaTime;

        if (_time < 0)
            _time = 0;
    }

    public void OffPullStatus()
    {
        _isPull = !_isPull;
    }

    public void OnPullStatus()
    {
        _isPull = true;
    }
    
    
    public bool GetPullStatus()
    {
        return _isPull;
    }

    public void DetectPull()
    {
        _time = _timeThreshold;
        if (_time > 0)
            return; //押されたばかりの場合は何もしない

        OnPullStatus();
    }

}
