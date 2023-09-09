using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleTargetGetter : MonoBehaviour
{
    [SerializeField] private GameObject _eagleTargetOnHead;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = _eagleTargetOnHead.transform.position;
    }
}
