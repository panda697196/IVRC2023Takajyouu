using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float time;

    public GameObject bGame;

    public float _duration;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        var a = this.transform.position;
        var b = bGame.transform.position+new Vector3(0,1.5f,0);

        // 補間位置計算

        // 補間位置を反映
       
        var t = Mathf.PingPong(Time.deltaTime / _duration, 1);
        transform.position = Vector3.Lerp(a, b, t);
    }
}
