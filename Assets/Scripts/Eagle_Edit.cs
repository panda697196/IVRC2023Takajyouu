using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle_Edit : MonoBehaviour
{
    private Animator eagle;
    public GameObject MainCamera;
    
    [Header("鷹の状態を表示")]
    [SerializeField] private EagleState _eagleState;

    public void SetEagleState(EagleState state)
    {
        _eagleState = state;
        
    }
    public EagleState EagleCurrentState => _eagleState;

    [Header("DebugMode 鷹の状態を変えることで鷹を動かせる")]
    public bool _isDebug;
    public enum EagleState
    {
        Idle,Takeoff,TurnR,TurnL,Lauding,Walk,Walkend,Glide,Attack,Hunt
    }
	void Start ()
    {
        eagle = GetComponent<Animator>();
	}
	
	void Update ()
    {
     
        if (eagle.GetCurrentAnimatorStateInfo(0).IsName("idle"))
             {
                 
                 Debug.Log("Idle");
                 //RootMotionOnOff(false);
                 eagle.SetBool("takeoff", false);
                 eagle.SetBool("fly", false);
                 eagle.SetBool("landing", false);
             }
        else if(eagle.GetCurrentAnimatorStateInfo(0).IsName("fly"))
        {
            Debug.Log("fly");
            //RootMotionOnOff(false);
        }
        else
        {
            Debug.Log("else");
            //RootMotionOnOff(true);
        }
            
        if (_isDebug)
        {
            if (_eagleState.ToString()=="Idle")
            {
                IdleFlyMode();
            }
        
       
        
            if (_eagleState.ToString()=="Takeoff")
            {
                IdleFlyMode();
                
               // RootMotionOnOff(true);
                TakeOff();
            }
            if (_eagleState.ToString()=="TurnL")
            {
                IdleFlyMode();
                
               // RootMotionOnOff(true);
                TurnLeft();
            }
            if (_eagleState.ToString()=="TurnR")
            {
                IdleFlyMode();
                
               // RootMotionOnOff(true);
                TurnRight();
            }
            if (_eagleState.ToString()=="Lauding")
            {
                IdleFlyMode();
                
                //RootMotionOnOff(true);
                Lauding();
            }
            if (_eagleState.ToString()=="Walk")
            {
                IdleFlyMode();
                
                //RootMotionOnOff(true);
                Walk();
            }
            if (_eagleState.ToString()=="Walkend")
            {
                WalkEnd();
            }
            if (_eagleState.ToString()=="Glide")
            {
                IdleFlyMode();
                
                //RootMotionOnOff(true);
                Glide();
            }
            if (_eagleState.ToString()=="Attack")
            {
                IdleFlyMode();
                
               // RootMotionOnOff(true);
                Attack();
            }
            if (_eagleState.ToString()=="Hunt")
            {
                IdleFlyMode();
                
              //  RootMotionOnOff(true);
                Hunt();
            
            }
        }

    }

    public void IdleFlyMode()
    {   
        eagle.SetBool("fly", true);
        eagle.SetBool("glide", false);
        eagle.SetBool("attack", false);
        eagle.SetBool("hunt", false);
        eagle.SetBool("flyleft", false);
        eagle.SetBool("flyright", false);
        eagle.SetBool("turnleft", false);
        eagle.SetBool("turnright", false);
        eagle.SetBool("walk", false);
        eagle.SetBool("idle", true);
        //RootMotionOnOff(false);
    }

    public void TakeOff()
    {
        eagle.SetBool("idle", false);
        eagle.SetBool("takeoff", true);
        _eagleState = EagleState.Idle;
    }

    public void TurnLeft()
    {
        eagle.SetBool("fly", false);
        eagle.SetBool("walk", false);
        eagle.SetBool("flyleft", true);
        eagle.SetBool("turnleft", true);
        eagle.SetBool("idle", false);
        _eagleState = EagleState.TurnL;
    }

    public void TurnRight()
    {
        eagle.SetBool("fly", false);
        eagle.SetBool("walk", false);
        eagle.SetBool("flyright", true);
        eagle.SetBool("turnright", true);
        eagle.SetBool("idle", false);
        _eagleState = EagleState.TurnR;
    }

    public void Lauding()
    {
        eagle.SetBool("landing", true);
        eagle.SetBool("fly", false);
        _eagleState = EagleState.Idle;
    }

    public void Walk()
    {
        eagle.SetBool("walk", true);
        eagle.SetBool("idle", false);
        _eagleState = EagleState.Walk;
    }

    public void Glide()
    {
        eagle.SetBool("glide", true);
        eagle.SetBool("fly", false);
        eagle.SetBool("flyleft", false);
        eagle.SetBool("flyright", false);
        _eagleState = EagleState.Glide;
    }
    
    public void Attack()
    {
        eagle.SetBool("attack", true);
        eagle.SetBool("fly", false);
        eagle.SetBool("glide", false);
        _eagleState = EagleState.Attack;
    }
    public void Hunt()
    {
        eagle.SetBool("hunt", true);
        eagle.SetBool("fly", false);
        eagle.SetBool("glide", false);
        _eagleState = EagleState.Idle;
    }

    public void WalkEnd()
    {
        eagle.SetBool("idle", true);
        eagle.SetBool("walk", false);
        _eagleState = EagleState.Idle;
    }

    public void test()
    {
        if (eagle.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            eagle.SetBool("takeoff", false);
            eagle.SetBool("fly", false);
            eagle.SetBool("landing", false);
        }
    }

    // public void ToFry()
    // {
    //     IdleFlyMode();
    //     eagle.SetBool("takeoff", false);
    //     eagle.SetBool("fly", false);
    //     eagle.SetBool("landing", false);
    //     TakeOff();
    // }
    //
    // public void RootMotionOnOff(bool tf)
    // {
    //     this.gameObject.GetComponent<Animator>().applyRootMotion = tf;
    // }
}
