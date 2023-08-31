using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class lb_Crow : MonoBehaviour
{
    public GameObject MainCamera;
    Animator anim;

    [Header("カラスの状態を表示")]
    public birdBehaviors _crowState;

    [Header("DebugMode カラスの状態を変えることでカラスを動かせる")]
    public bool _isDebug;

    public AudioClip song1;
    public AudioClip song2;
    public AudioClip flyAway1;
    public AudioClip flyAway2;

    public enum birdBehaviors
    {
        idle, sing, preen, ruffle, peck,
        flyLeft, flyRight, flyStraight, landing,
        hopForward, hopBackward, hopLeft, hopRight,
    }

    BoxCollider birdCollider;
    Vector3 bColCenter;
    Vector3 bColSize;
    SphereCollider solidCollider;
    float distanceToTarget = 0.0f;
    float agitationLevel = .5f;
    float Angle = 0.0f;
    int sp = 0;


    //飛行関連
    // 加速度
    [SerializeField] private Vector3 _acceleration = new Vector3();
    // 初速度
    [SerializeField] private float _speed = 0.01f;
    // 現在速度
    private Vector3 _velocity = Vector3.zero;

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
                break;
            case birdBehaviors.ruffle:
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
                if (_speed > 0)
                {
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
                break;
            case birdBehaviors.idle:
                anim.SetBool("idle", true);
                anim.SetBool("landing", false);
                anim.SetBool("flying", false);
                float i = Random.Range(0, 1);
                anim.SetFloat("IdleAgitated",i);
                break;
        }
    }

    void Fly(float t)
    {
        Angle = 45f * t;
        // 角度をラジアンに変換
        float rad = Angle * Mathf.Deg2Rad;
        _speed += 1f;
        _speed = Mathf.Clamp(_speed, 0, 100f);
        // ラジアンから進行方向を設定
        Vector3 direction = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
        // 方向に速度を掛け合わせて移動ベクトルを求める
        _velocity = direction * _speed * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Land()
    {
        // 角度をラジアンに変換
        float rad = Angle * Mathf.Deg2Rad;
        _speed -= 1f;
        _speed = Mathf.Clamp(_speed, 0, 100f);
        // ラジアンから進行方向を設定
        Vector3 direction = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
        // 方向に速度を掛け合わせて移動ベクトルを求める
        _velocity = direction * _speed * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;
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
    }

    void Update()
    {
        if (_isDebug)
        {
            if (_crowState.ToString() == "idle")
            {
                DisplayBehavior(birdBehaviors.idle);
            }
            if (_crowState.ToString() == "flyLeft")
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
            if (_crowState.ToString() == "sing")
            {
                DisplayBehavior(birdBehaviors.sing);
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
            }
        }
    }
}