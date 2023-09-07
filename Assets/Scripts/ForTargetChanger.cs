using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForTargetChanger : MonoBehaviour
{
    [SerializeField] private Eagle_Navigation _eagleNavigation;
    [SerializeField] private GameObject _target;

    public List<GameObject> _targetlist;
    public List<float> _timeTarget;

    private float _totalTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        _target.transform.position = _targetlist[0].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _totalTime +=  Time.deltaTime;
        
        if (_totalTime < _timeTarget[0])
            ChangeTarget(_targetlist[0]);
        for (int i = 0; i <= 9; i++)
        {
            if (_totalTime >= _timeTarget[i] && _totalTime < _timeTarget[i + 1])
            {
                ChangeTarget(_targetlist[i]);
                Debug.Log(i);
                
            }
        }
        
        for (int i = 0; i <= 9; i++)
        {
            if (_totalTime >= _timeTarget[i] && _totalTime < _timeTarget[i + 1])
            {
                float ratio = (_totalTime  - _timeTarget[i]) / (_timeTarget[i + 1] - _timeTarget[i]);
                _target.transform.position = _targetlist[i].transform.position + (_targetlist[i + 1].transform.position - _targetlist[i].transform.position) * ratio ;
                ChangeTarget(_targetlist[i]);
            }
        }
    }

    private void ChangeTarget(GameObject target)
    {
        //_eagleNavigation.ChangeTarget(target);
    }
}
