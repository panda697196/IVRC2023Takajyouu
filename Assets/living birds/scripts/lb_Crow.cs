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

    [Header("�J���X�̏�Ԃ�\��")]
    public birdBehaviors _crowState;

    [Header("DebugMode �J���X�̏�Ԃ�ς��邱�ƂŃJ���X�𓮂�����")]
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
    float originalAnimSpeed = 1.0f;
    Vector3 originalVelocity = Vector3.zero;
    private Transform _crowTransform;
    private Vector3 _crowPrevPosition;
    float Angle = 0;

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
                if (l > -1.0f)
                {
                    l -= 0.01f;
                    anim.SetFloat("flyingDirectionX", l);
                }
                //Fly(l);
                break;
            case birdBehaviors.flyRight:
                anim.SetBool("flying", true);
                anim.SetBool("idle", false);
                float r = anim.GetFloat("flyingDirectionX");
                if (r < 1.0f)
                {
                    r += 0.01f;
                    anim.SetFloat("flyingDirectionX", r);
                }
                //Fly(r);
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
               // Fly(s);
                break;
            case birdBehaviors.landing:
                anim.SetBool("landing", true);
                anim.SetBool("flying", false);
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

    void Fly(float p)
    {
        // ���[�J�����W����ɁA��]���擾
        Vector3 localAngle = _crowTransform.localEulerAngles;
        Angle += -45.0f * p;//�񂷊p�x���Z�o
        originalAnimSpeed = 1.0f;
        // �p�x�����W�A���ɕϊ�
        float rad = Angle * Mathf.Deg2Rad;
        // ���W�A������i�s������ݒ�
        Vector3 direction = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
        // �����ɑ��x���|�����킹�Ĉړ��x�N�g�������߂�
        originalVelocity = direction * originalAnimSpeed * Time.deltaTime;
        // �i�s�����i�ړ��ʃx�N�g���j�Ɍ����悤�ȃN�H�[�^�j�I�����擾
        var rotation = Quaternion.LookRotation(originalVelocity + localAngle);
        // ���̂��ړ�����
        transform.localPosition += originalVelocity;
        /*// �I�u�W�F�N�g�̉�]�ɔ��f
        _crowTransform.localRotation = rotation;

        
        // ���݃t���[���̃��[���h�ʒu
        var position = _crowTransform.position;

        // �ړ��ʂ��v�Z
        var delta = position - _crowPrevPosition;

        // ����Update�Ŏg�����߂̑O�t���[���ʒu�X�V
        _crowPrevPosition = position;

        // �Î~���Ă����Ԃ��ƁA�i�s���������ł��Ȃ����߉�]���Ȃ�
       // if (delta == Vector3.zero)
       //     return;

        

        


        localAngle.x = 10.0f; // ���[�J�����W����ɁAx�������ɂ�����]��10�x�ɕύX
        localAngle.y = 10.0f; // ���[�J�����W����ɁAy�������ɂ�����]��10�x�ɕύX
        localAngle.z = 10.0f; // ���[�J�����W����ɁAz�������ɂ�����]��10�x�ɕύX
        _crowTransform.localEulerAngles = localAngle; // ��]�p�x��ݒ�*/


        // transform���擾
        Transform myTransform = this.transform;

        // ���[�J�����W����ɁA��]���擾
        localAngle.x = 10.0f; // ���[�J�����W����ɁAx�������ɂ�����]��10�x�ɕύX
        localAngle.y = 10.0f; // ���[�J�����W����ɁAy�������ɂ�����]��10�x�ɕύX
        localAngle.z = 10.0f; // ���[�J�����W����ɁAz�������ɂ�����]��10�x�ɕύX
        myTransform.localEulerAngles = localAngle; // ��]�p�x��ݒ�
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
        _crowTransform = transform;
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