using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCrow : MonoBehaviour
{
    private GameObject _crow;
    private GameObject _randomTarget;

    public GameObject _crowStorage;
    public GameObject _targetStorage;
    public Vector3 _spwanCenter;

    private List<GameObject> _crowList = new List<GameObject>(1);
    private List<GameObject> _randomTargetList = new List<GameObject>(1);

    //�o���\�̏ꏊ���
    [SerializeField] private float radius;
    [SerializeField] private GameObject _scoreArea; //�J���X���o��������G���A
    [SerializeField] private GameObject lookObject; // �����������I�u�W�F�N�g��Inspector�������Ă���

    private Vector3 _areaSize;
    private Vector3 _offset;
    private float _areaMin = -0.5f;
    private float _areaMax = 0.5f;



    void RandomCirclePos(int a)
    {
        for (int i = 0; i <= a; i++)
        {
            /*float randomRangeX = Random.Range(_areaMin, _areaMax);
            float randomRangeY = Random.Range(_areaMin, _areaMax);
            float randomRangeZ = Random.Range(_areaMin, _areaMax);
            float xPos = randomRangeX * _areaSize.x;
            float yPos = randomRangeY * _areaSize.y;
            float zPos = randomRangeZ * _areaSize.z;
            Vector3 position = new Vector3(xPos, yPos, zPos) + _offset;*/

            // �w�肳�ꂽ���a�̉~���̃����_���ʒu
            var circlePos = radius * Random.insideUnitCircle;
            // XZ���ʂŎw�肳�ꂽ���a�A���S�_�̉~���̃����_���ʒu���v�Z
            var spawnPos = new Vector3(circlePos.x, 0, circlePos.y) + _spwanCenter;
            // �^�[�Q�b�g�����̃x�N�g�����擾
            Vector3 relativePos = lookObject.transform.position - spawnPos;
            // Prefab���C���X�^���X������
            GameObject newCrow = Instantiate(_crow, spawnPos, Quaternion.LookRotation(relativePos));
            lb_Crow lbCrow = newCrow.GetComponent<lb_Crow>();
            //���₷���悤�ɐ��������J���X��CrowStorage�Ɋi�[
            newCrow.transform.parent = _crowStorage.transform;
            //���������J���X�����X�g�ɒǉ�
            _crowList.Add(newCrow);
            lbCrow.SetTargetList(_randomTargetList);
            lbCrow.SetCrowState(lb_Crow.birdBehaviors.sing);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //�J���X��Pregfab�̓ǂݎ��
        _crow = (GameObject)Resources.Load("lb_crow_target");
        //Target��Pregfab�̓ǂݎ��
        _randomTarget = (GameObject)Resources.Load("Sphere");
        //�J���X��Pregfab���琶�������I�u�W�F�̊i�[
        _crowStorage = GameObject.Find("CrowStorage");
        //Traget��Pregfab���琶�������I�u�W�F�̊i�[
        _targetStorage = GameObject.Find("TargetStorage");
        _areaSize = _scoreArea.transform.localScale;
        _offset = _scoreArea.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            CrowGenerator();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            int flyCrow = ScaredCrowNumber();

            Debug.Log("��񂾃J���X" + flyCrow);
        }

    }

    //�J���X���X�g���ɂ���C��ɂ���Ĕ�񂾃J���X�𐔂��郁�\�b�h
    public int ScaredCrowNumber()
    {
        int count = 0;
        foreach (GameObject crow in _crowList)
        {
            if (crow.transform.GetChild(2).GetComponent<lb_CrowTrigger>().IsEagleScared)
            {
                count++;
            }
        }

        return count;
    }

    //�J���X��_crowMaxNumber�܂Ő������郁�\�b�h�@�X�|�[����Center�̈ʒu�𒆐S�ɐ����`�ɐ���
    public void CrowGenerator()
    {
        RandomCirclePos(_crowMaxNumber);
    }
}
