using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowCount : MonoBehaviour
{
    private int _count;
    public int Count => _count;

    // Start is called before the first frame update
    void Start()
    {
        _count = 0;
    }

    public void CountUp()
    {
        _count++;
        Debug.Log(_count);
    }

    public void CountReset()
    {
        _count = 0;
        Debug.Log("ResetCount");
    }
}

