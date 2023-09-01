using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCtr : MonoBehaviour
{
    public Transform cameraPos;

    public float fixXPos=0;
    public float fixYPos=0;
    public float fixZPos=0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<Transform>().position = new Vector3(cameraPos.position.x+fixXPos, cameraPos.position.y+fixYPos, cameraPos.position.z+fixZPos);
    }
}
