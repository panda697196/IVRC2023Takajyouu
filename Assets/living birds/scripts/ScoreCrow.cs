using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCrow : MonoBehaviour
{
    private GameObject _randomScoreTarget;

    public GameObject _randomScoreTargetStorage;
    public Vector3 _spwanCenter;

    private List<GameObject> _backCrowList = new List<GameObject>(1);
    private List<GameObject> _randomScoreTargetList = new List<GameObject>(1);

    //�o���\�̏ꏊ���
    [SerializeField] private float radius; //�J���X���o������G���A�̔��a
    [SerializeField] private int _comebackCrow; //�A���ė���J���X�̐�
    [SerializeField] private GameObject lookObject; //�J���X�̌������������߂�I�u�W�F�N�g
    private GameObject CrowManager;
    CrowGenerater Crowgene;



    void RandomCirclePos(int a)
    {
        for (int i = 0; i < a; i++)
        {
            
            // �w�肳�ꂽ���a�̉~���̃����_���ʒu
            var circlePos = radius * Random.insideUnitCircle;
            // XZ���ʂŎw�肳�ꂽ���a�A���S�_�̉~���̃����_���ʒu���v�Z
            var spawnPos = new Vector3(circlePos.x, 0, circlePos.y) + _spwanCenter;
            // �^�[�Q�b�g�����̃x�N�g�����擾
            Vector3 relativePos = lookObject.transform.position - spawnPos;
            // Prefab���C���X�^���X������
            GameObject newTarget = Instantiate(_randomScoreTarget, spawnPos, Quaternion.LookRotation(relativePos));
            //���₷���悤�ɐ�������Target��RandomScoreTargetStorage�Ɋi�[
            newTarget.transform.parent = _randomScoreTargetStorage.transform;
            //��������Target�����X�g�ɒǉ�
            _randomScoreTargetList.Add(newTarget);
            lb_Crow lbCrow = _backCrowList[i].GetComponent<lb_Crow>();
            lbCrow.SetTargetList(_randomScoreTargetList);
            lbCrow.SetCrowState(lb_Crow.birdBehaviors.flyToTarget);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Target��Pregfab�̓ǂݎ��
        _randomScoreTarget = (GameObject)Resources.Load("Sphere");
        //Traget��Pregfab���琶�������I�u�W�F�̊i�[
        _randomScoreTargetStorage = GameObject.Find("RandomScoreTargetStorage");
        CrowManager = GameObject.Find("CrowManager");
        lb_CrowTrigger lbTrigger = new lb_CrowTrigger();
        Crowgene = CrowManager.GetComponent<CrowGenerater>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ScaredCrowNumber();
            ScoreCrowPos();
        }

    }

    //�J���X���X�g���ɂ���C��ɂ���Ĕ�񂾃J���X�𐔂��郁�\�b�h
    public void ScaredCrowNumber()
    {
        int i = 0;
        int count = 0;
        List<GameObject> _crowList = Crowgene.CrowList;
        foreach (GameObject crow in _crowList)
        {
            if (crow.transform.GetChild(2).GetComponent<lb_CrowTrigger>().IsEagleScared)
            {
                count++; //��ɂ���Ĕ�񂾃J���X�𐔂���
            }
            else
            {
                _backCrowList.Add(_crowList[i]); //�ǂ������Ȃ������J���X���i�[
            }
            i++;
        }
        _comebackCrow = _backCrowList.Count;
    }

    //�J���X��_crowMaxNumber�܂Ő������郁�\�b�h�@�X�|�[����Center�̈ʒu�𒆐S�ɐ����`�ɐ���
    public void ScoreCrowPos()
    {
        RandomCirclePos(_comebackCrow);
    }
}
