using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class trackerMiddlePoint : MonoBehaviour
{
    public Transform trackerW; // トラッカー1のTransformコンポーネント
    public Transform trackerS; // トラッカー2のTransformコンポーネント

    private Vector3 middlePointPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Vector3 middlePointPos = (trackerW.position + trackerS.position) / 2;
        Debug.Log("middle:" + middlePointPos);

        this.transform.position = middlePointPos;
        this.transform.rotation = trackerW.rotation;

    }
}
