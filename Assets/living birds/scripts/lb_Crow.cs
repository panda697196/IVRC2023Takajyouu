using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static crow_Edit;

public class lb_Crow : MonoBehaviour
{
    private Animator crow;
    public GameObject MainCamera;

    [Header("カラスの状態を表示")]
    public CrowState _crowState;

    [Header("DebugMode カラスの状態を変えることでカラスを動かせる")]
    public bool _isDebug;

    public AudioClip song1;
    public AudioClip song2;
    public AudioClip flyAway1;
    public AudioClip flyAway2;

    public enum CrowState
    {
        sing,
        preen,
        ruffle,
        peck,
        hopForward,
        hopBackward,
        hopLeft,
        hopRight,
    }

    // Start is called before the first frame update
    void Start()
    {
        crow = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (crow.GetCurrentAnimatorStateInfo(0).IsName("Sing"))
        {
            crow.SetBool("takeoff", false);
            crow.SetBool("fly", false);
            crow.SetBool("landing", false);
        }

        if (_isDebug)
        {
            if (_crowState.ToString() == "sing")
            {
                SingFlyMode();
            }
            if (_crowState.ToString() == "preen")
            {
                SingFlyMode();
                test();
                Preen();
            }
            if (_crowState.ToString() == "ruffle")
            {
                SingFlyMode();
                test();
                Ruffle();
            }
            if (_crowState.ToString() == "peck")
            {
                SingFlyMode();
                test();
                Peck();
            }
            if (_crowState.ToString() == "hopForward")
            {
                SingFlyMode();
                test();
                HopForward();
            }
            if (_crowState.ToString() == "hopBackward")
            {
                SingFlyMode();
                test();
                HopBackward();
            }
            if (_crowState.ToString() == "hopLeft")
            {
                SingFlyMode();
                test();
                HopLeft();
            }
            if (_crowState.ToString() == "hopRight")
            {
                SingFlyMode();
                test();
                HopRight();
            }
        }
    }

    public void SingFlyMode()
    {
        crow.SetBool("sing", true);
        crow.SetBool("preen", false);
        crow.SetBool("ruffle", false);
        crow.SetBool("peck", false);
        crow.SetBool("hopForward", false);
        crow.SetBool("hopBackward", false);
        crow.SetBool("hopLeft", false);
        crow.SetBool("HopRight", false);

    }

    public void Preen()
    {
        crow.SetBool("Sing", false);
        crow.SetBool("preen", true);
    }

    public void Ruffle()
    {
        crow.SetBool("fly", false);
        crow.SetBool("walk", false);
        crow.SetBool("flyleft", true);
        crow.SetBool("turnleft", true);
        crow.SetBool("Sing", false);
    }

    public void TurnRight()
    {
        crow.SetBool("fly", false);
        crow.SetBool("walk", false);
        crow.SetBool("flyright", true);
        crow.SetBool("turnright", true);
        crow.SetBool("Sing", false);
    }

    public void Lauding()
    {
        crow.SetBool("landing", true);
        crow.SetBool("fly", false);
    }

    public void Walk()
    {
        crow.SetBool("walk", true);
        crow.SetBool("Sing", false);
    }

    public void Glide()
    {
        crow.SetBool("glide", true);
        crow.SetBool("fly", false);
        crow.SetBool("flyleft", false);
        crow.SetBool("flyright", false);
    }

    public void Attack()
    {
        crow.SetBool("attack", true);
        crow.SetBool("fly", false);
        crow.SetBool("glide", false);
    }
    public void Hunt()
    {
        crow.SetBool("hunt", true);
        crow.SetBool("fly", false);
        crow.SetBool("glide", false);
    }

    public void WalkEnd()
    {
        crow.SetBool("Sing", true);
        crow.SetBool("walk", false);
    }

    public void test()
    {
        if (crow.GetCurrentAnimatorStateInfo(0).IsName("Sing"))
        {
            crow.SetBool("takeoff", false);
            crow.SetBool("fly", false);
            crow.SetBool("landing", false);
        }
    }
}
