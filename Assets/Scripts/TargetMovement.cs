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
        // �ʑ����v�Z.
        var phase = (float)(_totalTime * _rotationSpeed / _radius + Mathf.PI / 2);

        // �ʑ�����ʒu���v�Z�D
        float xPos = _radius * Mathf.Cos(phase);
        float zPos = _radius * Mathf.Sin(phase);
        // �Q�[���I�u�W�F�N�g�̈ʒu��ݒ�.���S�ƂȂ�I�u�W�F�N�g���甼�a���̉~�^��������D�����͒��S���̂̍����{_height
        initPos = new Vector3(initPos.x + xPos, initPos.y, initPos.z + zPos);
        transform.position = initPos;
        //_debug.transform.position = pos;
    }
}
