using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowStandby : MonoBehaviour
{
    public bool _isStandbyToThrow;

    // Start is called before the first frame update
    void Start()
    {
        _isStandbyToThrow = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        //�ڐG���Ă���I�u�W�F�N�g�̃^�O��"Tracker"�̂Ƃ�
        if (other.CompareTag("Tracker"))
        {
            _isStandbyToThrow = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //�ڐG���Ă���I�u�W�F�N�g�̃^�O��"Tracker"�̂Ƃ�
        if (other.CompareTag("Tracker"))
        {
            _isStandbyToThrow = false;
        }
    }
}


}

