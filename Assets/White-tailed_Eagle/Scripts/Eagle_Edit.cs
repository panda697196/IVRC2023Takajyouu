using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle_Edit : MonoBehaviour
{
    private Animator eagle;
    public GameObject MainCamera;

    [Header("鷹の状態を表示")]
    public EagleState _eagleState;

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
                 eagle.SetBool("takeoff", false);
                 eagle.SetBool("fly", false);
                 eagle.SetBool("landing", false);
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
                test();
                TakeOff();
            }
            if (_eagleState.ToString()=="TurnL")
            {
                IdleFlyMode();
                test();
                TurnLeft();
            }
            if (_eagleState.ToString()=="TurnR")
            {
                IdleFlyMode();
                test();
                TurnRight();
            }
            if (_eagleState.ToString()=="Lauding")
            {
                IdleFlyMode();
                test();
                Lauding();
            }
            if (_eagleState.ToString()=="Walk")
            {
                IdleFlyMode();
                test();
                Walk();
            }
            if (_eagleState.ToString()=="Walkend")
            {
                WalkEnd();
            }
            if (_eagleState.ToString()=="Glide")
            {
                IdleFlyMode();
                test();
                Glide();
            }
            if (_eagleState.ToString()=="Attack")
            {
                IdleFlyMode();
                test();
                Attack();
            }
            if (_eagleState.ToString()=="Hunt")
            {
                IdleFlyMode();
                test();
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
        
    }

    public void TakeOff()
    {
        eagle.SetBool("idle", false);
        eagle.SetBool("takeoff", true);
    }

    public void TurnLeft()
    {
        eagle.SetBool("fly", false);
        eagle.SetBool("walk", false);
        eagle.SetBool("flyleft", true);
        eagle.SetBool("turnleft", true);
        eagle.SetBool("idle", false);
    }

    public void TurnRight()
    {
        eagle.SetBool("fly", false);
        eagle.SetBool("walk", false);
        eagle.SetBool("flyright", true);
        eagle.SetBool("turnright", true);
        eagle.SetBool("idle", false);
    }

    public void Lauding()
    {
        eagle.SetBool("landing", true);
        eagle.SetBool("fly", false);
    }

    public void Walk()
    {
        eagle.SetBool("walk", true);
        eagle.SetBool("idle", false);
    }

    public void Glide()
    {
        eagle.SetBool("glide", true);
        eagle.SetBool("fly", false);
        eagle.SetBool("flyleft", false);
        eagle.SetBool("flyright", false);
    }
    
    public void Attack()
    {
        eagle.SetBool("attack", true);
        eagle.SetBool("fly", false);
        eagle.SetBool("glide", false);
    }
    public void Hunt()
    {
        eagle.SetBool("hunt", true);
        eagle.SetBool("fly", false);
        eagle.SetBool("glide", false);
    }

    public void WalkEnd()
    {
        eagle.SetBool("idle", true);
        eagle.SetBool("walk", false);
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
}
