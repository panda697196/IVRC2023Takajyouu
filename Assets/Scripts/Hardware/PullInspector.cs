using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullInspector : MonoBehaviour
{
    [SerializeField] private float _timeThreshold = 5f;
    private bool _isPull = false;

    private float _time = 0;

    // Update is called once per frame
    void Update()
    {
        if (_time > 0)
            _time -= Time.deltaTime;

        if (_time < 0)
            _time = 0;

        if (_time == 0 && _isPull)
            _isPull = false;

    }

    public void OffPullStatus()
    {
        _isPull = false;
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
        OnPullStatus();
    }

}
