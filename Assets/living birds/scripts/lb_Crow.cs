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

    [Header("DebugMode カラスの状態を変えることでカラスを動かせる")]
    public bool _isDebug;

    public enum birdBehaviors
    {
        idle, flyToTarget, flyToTarget2, randomFly, sing,
    }

    BoxCollider birdCollider;
    Vector3 bColCenter;
    Vector3 bColSize;
    SphereCollider solidCollider;
    float distanceToTarget = 0.0f;
    float agitationLevel = .5f;
    float Angle = 0.0f;

    // �����x
    [SerializeField] private float _speed = 0.01f;
    // ���ݑ��x
    private Vector3 _velocity = Vector3.zero;
    // ����
    [SerializeField] private float _hight = 0;

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
        flyingBoolHash = Animator.StringToHash("flying");
        landingBoolHash = Animator.StringToHash("landing");
        singTriggerHash = Animator.StringToHash("sing");
        flyingDirectionHash = Animator.StringToHash("flyingDirectionX");
    }
    void DisplayBehavior(birdBehaviors behavior)
    {
        switch (behavior)
        {
            case birdBehaviors.sing:
                anim.SetBool("idle", false);
                anim.SetTrigger(singTriggerHash);
                break;
            case birdBehaviors.idle:
                anim.SetBool("idle", true);
                anim.SetBool("landing", false);
                anim.SetBool("flying", false);
                //float i = Random.Range(0, 1.0f);
                //anim.SetFloat("IdleAgitated",i);
                _hight = 0.5f;
                break;
            case birdBehaviors.flyToTarget:
                float dis = Vector3.SqrMagnitude(_target.transform.position - transform.position);
                if (dis > 10f)
                {
                    anim.SetBool("flying", true);
                    anim.SetBool("idle", false);
                    Flytest(_target.transform);
                }
                else
                {
                    if (dis < 0.01f)
                    {
                        anim.SetBool("idle", true);
                        anim.SetBool("landing", false);
                        anim.SetBool("flying", false);
                        float j = Random.Range(0, 1);
                        anim.SetFloat("IdleAgitated", j);
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
                _speed = Random.Range(3.0f, 5.0f);
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
                _speed = Random.Range(3.0f, 5.0f);
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
        }
    }

    public void Flytest(Transform target)
    {
        _speed += 0.1f;
        _speed = Mathf.Clamp(_speed, 0, 10f);
        transform.position = Vector3.MoveTowards(transform.position, target.position, _speed * Time.deltaTime);
        gameObject.transform.LookAt(target.transform);
        UnityEngine.Debug.Log("FlyTest");
    }

    void Landtest(Transform target)
    {
        if (transform.position.y == target.position.y + 2f)
        {
            _speed = 0;
        }
        else
        {
            _speed -= 0.1f;
            _speed = Mathf.Clamp(_speed, 0.5f, 10f);
        }
        var direction = transform.forward;
        // 方向に速度を掛け合わせて移動ベクトルを求める
        transform.position = Vector3.MoveTowards(transform.position, target.position, _speed * Time.deltaTime);
        gameObject.transform.LookAt(target.transform);
        UnityEngine.Debug.Log("LandTest");
    }

    void Fly(float t)
    {
        if (_hight > 0)
        {
            _hight -= Random.Range(0.000001f, 0.00001f);
        }
        else
        {
            _hight = 0;
        }
        Angle = 60f * t;
        // �p�x�����W�A���ɕϊ�
        float rad = Angle * Mathf.Deg2Rad;
        _speed += 10f;
        _speed = Mathf.Clamp(_speed, 0, 50f);
        // ���W�A������i�s������ݒ�
        Vector3 direction = new Vector3(Mathf.Sin(rad), _hight, Mathf.Cos(rad));
        // �����ɑ��x���|�����킹�Ĉړ��x�N�g�������߂�
        _velocity = direction * _speed * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;
        UnityEngine.Debug.Log(direction);
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Land()
    {
        if (transform.position.y <= -0.001f)
        {
            _hight = 0;
            _speed = 0;
        }
        else
        {
            _hight = Mathf.Clamp(_hight, -0.5f, 0.1f);
            _hight += Random.Range(0.000001f, 0.00001f);
            _speed -= 0.1f;
            _speed = Mathf.Clamp(_speed, 10f, 50f);
        }
        // �p�x�����W�A���ɕϊ�
        float rad = Angle * Mathf.Deg2Rad;
        // ���W�A������i�s������ݒ�
        Vector3 direction = new Vector3(Mathf.Sin(rad), _hight, Mathf.Cos(rad));
        // �����ɑ��x���|�����킹�Ĉړ��x�N�g�������߂�
        _velocity = direction * _speed * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;
        UnityEngine.Debug.Log(direction);
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
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
        }
    }
}