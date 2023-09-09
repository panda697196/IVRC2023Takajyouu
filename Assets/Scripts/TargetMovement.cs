using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    private float _totalTime;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _radius;
    private Vector3 initPos;


    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.position;
        _totalTime = 0+Random.Range(0,3f);

    }

    // Update is called once per frame
    void Update()
    {
        _totalTime += Time.deltaTime;
        // 位相を計算.
        var phase = (float)(_totalTime * _rotationSpeed / _radius + Mathf.PI / 2);

        // 位相から位置を計算．
        float xPos = _radius * Mathf.Cos(phase);
        float zPos = _radius * Mathf.Sin(phase);
        // ゲームオブジェクトの位置を設定.中心となるオブジェクトから半径分の円運動をする．高さは中心物体の高さ＋_height
        initPos = new Vector3(initPos.x + xPos, initPos.y, initPos.z + zPos);
        transform.position = initPos;
        //_debug.transform.position = pos;
    }
}
