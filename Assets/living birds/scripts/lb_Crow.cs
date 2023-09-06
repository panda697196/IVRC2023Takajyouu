using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Eagle_Edit;
using static UnityEngine.GraphicsBuffer;


public class lb_Crow : MonoBehaviour
{
    public GameObject MainCamera;
    Animator anim;
    public float _idleAgitated;
    private List<GameObject> _randomTargetList = new List<GameObject>(1);

    [SerializeField] private birdBehaviors _crowState;
    //target
    [SerializeField] private GameObject _target;
    //�q�ǉ��Z�b�g�v�f 
    public void SetTarget(GameObject newTarget)
    {
        _target = newTarget;
    }

    public void SetTargetList(List<GameObject> randomTargetList)
    {
        _randomTargetList = randomTargetList;
    }

    public void SetCrowState(birdBehaviors state)
    {
        _crowState = state;
    }
    public birdBehaviors CrowCurrentState => _crowState;

    [Header("DebugMode �J���X�̏�Ԃ�ς��邱�ƂŃJ���X�𓮂�����")]
    public bool _isDebug;

    public AudioClip song1;
    public AudioClip song2;
    public AudioClip flyAway1;
    public AudioClip flyAway2;

    public enum birdBehaviors
    {
        idle, flyToTarget, flyToTarget2, randomFly, sing, /*preen, ruffle, peck,
        flyLeft, flyRight, flyStraight, landing,
        hopForward, hopBackward, hopLeft, hopRight,*/
    }

    BoxCollider birdCollider;
    Vector3 bColCenter;
    Vector3 bColSize;
    SphereCollider solidCollider;
    float distanceToTarget = 0.0f;
    float agitationLevel = .5f;
    float Angle = 0.0f;

    // �����x
    [SerializeField] private float _speed2 = 0.01f;
    // ���ݑ��x
    private Vector3 _velocity = Vector3.zero;
    // ����
    [SerializeField] private float _hight2 = 0;

    //hash variables for the animation states and animation properties
    int idleAnimationHash;
    int singAnimationHash;
    int ruffleAnimationHash;
    int preenAnimationHash;
    int peckAnimationHash;
    int hopForwardAnimationHash;
    int hopBackwardAnimationHash;
    int hopLeftAnimationHash;
    int hopRightAnimationHash;
    int worriedAnimationHash;
    int landingAnimationHash;
    int flyAnimationHash;
    int hopIntHash;
    int flyingBoolHash;
    int peckBoolHash;
    int ruffleBoolHash;
    int preenBoolHash;
    int landingBoolHash;
    int singTriggerHash;
    int flyingDirectionHash;
    int dieTriggerHash;

    void OnEnable()
    {
        idleAnimationHash = Animator.StringToHash("Base Layer.Idle");
        flyAnimationHash = Animator.StringToHash("Base Layer.fly");
        hopIntHash = Animator.StringToHash("hop");
        flyingBoolHash = Animator.StringToHash("flying");
        peckBoolHash = Animator.StringToHash("peck");
        ruffleBoolHash = Animator.StringToHash("ruffle");
        preenBoolHash = Animator.StringToHash("preen");
        landingBoolHash = Animator.StringToHash("landing");
        singTriggerHash = Animator.StringToHash("sing");
        flyingDirectionHash = Animator.StringToHash("flyingDirectionX");
        dieTriggerHash = Animator.StringToHash("die");
    }
    void DisplayBehavior(birdBehaviors behavior)
    {
        switch (behavior)
        {
            case birdBehaviors.sing:
                anim.SetBool("idle", false);
                anim.SetTrigger(singTriggerHash);
                PlaySong();
                break;
            case birdBehaviors.idle:
                anim.SetBool("idle", true);
                anim.SetBool("landing", false);
                anim.SetBool("flying", false);
                //float i = Random.Range(0, 1.0f);
                //anim.SetFloat("IdleAgitated",i);
                _hight2 = 0.5f;
                break;
            case birdBehaviors.flyToTarget:
                float dis = Vector3.SqrMagnitude(_target.transform.position - transform.position);
                if(dis > 49f)
                {
                    anim.SetBool("flying", true);
                    anim.SetBool("idle", false);
                    Flytest(_target.transform);
                }
                else
                {
                    if (dis < 25f)
                    {
                        anim.SetBool("idle", true);
                        anim.SetBool("landing", false);
                        anim.SetBool("flying", false);
                        _crowState = birdBehaviors.idle;
                    }
                    else
                    {
                        anim.SetBool("landing", true);
                        anim.SetBool("flying", false);
                        Landtest(_target.transform);
                    }
                }
                break;
            case birdBehaviors.flyToTarget2://target�ɋ߂Â��Ă������Ɣ�s���
                float dis3 = Vector3.SqrMagnitude(_target.transform.position - transform.position);
                _speed2 = Random.Range(3.0f, 5.0f);
                anim.SetBool("flying", true);
                anim.SetBool("idle", false);
                Flytest(_target.transform);
                break;
            case birdBehaviors.randomFly:
                if (_target == null)
                {
                    _target = _randomTargetList[Random.Range(0, _randomTargetList.Count - 1)];
                }
                float dis2 = Vector3.SqrMagnitude(_target.transform.position - transform.position);
                _speed2 = Random.Range(3.0f, 5.0f);
                if (dis2 > 10f)
                {
                    anim.SetBool("flying", true);
                    anim.SetBool("idle", false);
                    Flytest(_target.transform);
                }
                else
                {
                    anim.SetBool("flying", true);
                    anim.SetBool("idle", false);
                    _target = _randomTargetList[Random.Range(0, _randomTargetList.Count - 1)];
                    Flytest(_target.transform);
                }
                break;
                /*case birdBehaviors.ruffle:
                anim.SetBool("idle", false);
                anim.SetTrigger(ruffleBoolHash);
                break;
            case birdBehaviors.preen:
                anim.SetBool("idle", false);
                anim.SetTrigger(preenBoolHash);
                break;
            case birdBehaviors.peck:
                anim.SetBool("idle", false);
                anim.SetTrigger(peckBoolHash);
                break;
            case birdBehaviors.hopForward:
                anim.SetBool("idle", false);
                anim.SetInteger(hopIntHash, 1);
                break;
            case birdBehaviors.hopLeft:
                anim.SetBool("idle", false);
                anim.SetInteger(hopIntHash, -2);
                break;
            case birdBehaviors.hopRight:
                anim.SetBool("idle", false);
                anim.SetInteger(hopIntHash, 2);
                break;
            case birdBehaviors.hopBackward:
                anim.SetBool("idle", false);
                anim.SetInteger(hopIntHash, -1);
                break;
            case birdBehaviors.flyLeft:
                anim.SetBool("flying", true);
                anim.SetBool("idle", false);
                float l = anim.GetFloat("flyingDirectionX");
                if (l > -0.5f)
                {
                    l -= 0.01f;
                    anim.SetFloat("flyingDirectionX", l);
                }
                Fly(l);
                break;
            case birdBehaviors.flyRight:
                anim.SetBool("flying", true);
                anim.SetBool("idle", false);
                float r = anim.GetFloat("flyingDirectionX");
                if (r < 0.5f)
                {
                    r += 0.01f;
                    anim.SetFloat("flyingDirectionX", r);
                }
                Fly(r);
                break;
            case birdBehaviors.flyStraight:
                anim.SetBool("flying", true);
                anim.SetBool("idle", false);
                float s = anim.GetFloat("flyingDirectionX");
                if (s > 0.0f)
                {
                    s -= 0.01f;
                    anim.SetFloat("flyingDirectionX", s);
                }
                if (s < 0.0f)
                {
                    s += 0.01f;
                    anim.SetFloat("flyingDirectionX", s);
                }
                Fly(s);
                break;
            case birdBehaviors.landing:
                if (transform.position.y >= -0.001f)
                {
                    if (_hight2 > 0)
                    {
                        _hight2 = -_hight2;
                    }
                    anim.SetBool("landing", true);
                    anim.SetBool("flying", false);
                    Land();
                }
                else
                {
                    anim.SetBool("idle", true);
                    anim.SetBool("landing", false);
                    anim.SetBool("flying", false);
                    float j = Random.Range(0, 1);
                    anim.SetFloat("IdleAgitated", j);
                }
                break;*/
                }
        }

    public void Flytest(Transform target)
    {
        _speed2 += 0.1f;
        _speed2 = Mathf.Clamp(_speed2, 0, 3f);
        transform.position = Vector3.MoveTowards(transform.position, target.position, _speed2 * Time.deltaTime);
        gameObject.transform.LookAt(target.transform);
        UnityEngine.Debug.Log("FlyTest");
    }

    void Landtest(Transform target)
    {
        if (transform.position.y <= target.position.y + 2f)
        {
            _speed2 = 0;
        }
        else
        {
            _speed2 -= 0.1f;
            _speed2 = Mathf.Clamp(_speed2, 0.5f, 3f);
        }
        var direction = transform.forward;
        // �����ɑ��x���|�����킹�Ĉړ��x�N�g�������߂�
        transform.position = Vector3.MoveTowards(transform.position, target.position, _speed2 * Time.deltaTime);
        gameObject.transform.LookAt(target.transform);
        UnityEngine.Debug.Log("LandTest");
    }

    void Fly(float t)
    {
        if (_hight2 > 0)
        {
            _hight2 -= Random.Range(0.000001f, 0.00001f);
        }
        else
        {
            _hight2 = 0;
        }
        Angle = 60f * t;
        // �p�x�����W�A���ɕϊ�
        float rad = Angle * Mathf.Deg2Rad;
        _speed2 += 10f;
        _speed2 = Mathf.Clamp(_speed2, 0, 50f);
        // ���W�A������i�s������ݒ�
        Vector3 direction = new Vector3(Mathf.Sin(rad), _hight2, Mathf.Cos(rad));
        // �����ɑ��x���|�����킹�Ĉړ��x�N�g�������߂�
        _velocity = direction * _speed2 * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;
        UnityEngine.Debug.Log(direction);
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Land()
    {
        if (transform.position.y <= -0.001f)
        {
            _hight2 = 0;
            _speed2 = 0;
        }
        else
        {
            _hight2 = Mathf.Clamp(_hight2, -0.5f, 0.1f);
            _hight2 += Random.Range(0.000001f, 0.00001f);
            _speed2 -= 0.1f;
            _speed2 = Mathf.Clamp(_speed2, 10f, 50f);
        }
        // �p�x�����W�A���ɕϊ�
        float rad = Angle * Mathf.Deg2Rad;
        // ���W�A������i�s������ݒ�
        Vector3 direction = new Vector3(Mathf.Sin(rad), _hight2, Mathf.Cos(rad));
        // �����ɑ��x���|�����킹�Ĉړ��x�N�g�������߂�
        _velocity = direction * _speed2 * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;
        UnityEngine.Debug.Log(direction);
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }


    void PlaySong()
    {
        if (Random.value < .5)
        {
            GetComponent<AudioSource>().PlayOneShot(song1, 1);
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(song2, 1);
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat("IdleAgitated", _idleAgitated);
    }

    void Update()
    {
        if (_isDebug)
        {
            if (_crowState.ToString() == "idle")
            {
                DisplayBehavior(birdBehaviors.idle);
            }
            if (_crowState.ToString() == "flyToTarget")
            {
                DisplayBehavior(birdBehaviors.flyToTarget);
            }
            if (_crowState.ToString() == "flyToTarget2")
            {
                DisplayBehavior(birdBehaviors.flyToTarget2);
            }
            if (_crowState.ToString() == "randomFly")
            {
                DisplayBehavior(birdBehaviors.randomFly);
            }
                if (_crowState.ToString() == "sing")
            {
                DisplayBehavior(birdBehaviors.sing);
            }
            /*if (_crowState.ToString() == "flyLeft")
            {
                DisplayBehavior(birdBehaviors.flyLeft);
            }
            if (_crowState.ToString() == "flyRight")
            {
                DisplayBehavior(birdBehaviors.flyRight);
            }
            if (_crowState.ToString() == "flyStraight")
            {
                DisplayBehavior(birdBehaviors.flyStraight);
            }
            if (_crowState.ToString() == "landing")
            {
                DisplayBehavior(birdBehaviors.landing);
            }
            if (_crowState.ToString() == "preen")
            {
                DisplayBehavior(birdBehaviors.preen);
            }
            if (_crowState.ToString() == "ruffle")
            {
                DisplayBehavior(birdBehaviors.ruffle);
            }
            if (_crowState.ToString() == "peck")
            {
                DisplayBehavior(birdBehaviors.peck);
            }
            if (_crowState.ToString() == "hopForward")
            {
                DisplayBehavior(birdBehaviors.hopForward);
            }
            if (_crowState.ToString() == "hopLeft")
            {
                DisplayBehavior(birdBehaviors.hopLeft);
            }
            if (_crowState.ToString() == "hopRight")
            {
                DisplayBehavior(birdBehaviors.hopRight);
            }
            if (_crowState.ToString() == "hopBackward")
            {
                DisplayBehavior(birdBehaviors.hopBackward);
            }*/
        }
    }
}