using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleTargetMaker : MonoBehaviour
{
    [SerializeField] private GameObject _eagleTargetReferance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = _eagleTargetReferance.transform.position;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);
    }
}
