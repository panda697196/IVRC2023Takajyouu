using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyArmReadyDetection : MonoBehaviour
{
    [SerializeField] private Transform WaistTracker;
    [SerializeField] private Collider HMDSideCollider;
    [SerializeField] private ArmCollisionDetection _armCollisionDetection;
    [SerializeField] private bool _isFirstReadyOfArm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_armCollisionDetection.isArmCllisionDetection(WaistTracker, HMDSideCollider))
        {
            Debug.Log("I'm OK");
            _isFirstReadyOfArm = true;
        }
        else
        {
            _isFirstReadyOfArm = false;
        }
        
    }

    public bool GetIsFirstReadyOfArm()
    {
        return _isFirstReadyOfArm;
    }
}
