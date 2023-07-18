using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eagle : MonoBehaviour
{
    private Animator eagle;
    public GameObject MainCamera;

	void Start ()
    {
        eagle = GetComponent<Animator>();
	}
	
	void Update ()
    {
        if ((Input.GetKeyUp(KeyCode.F))||(Input.GetKeyUp(KeyCode.V))||(Input.GetKeyUp(KeyCode.E))||(Input.GetKeyUp(KeyCode.A))||(Input.GetKeyUp(KeyCode.D)))
        {
            eagle.SetBool("fly", true);
            eagle.SetBool("glide", false);
            eagle.SetBool("attack", false);
            eagle.SetBool("hunt", false);
            eagle.SetBool("flyleft", false);
            eagle.SetBool("flyright", false);
            eagle.SetBool("turnleft", false);
            eagle.SetBool("turnright", false);
            eagle.SetBool("idle", true);
        }
        if (eagle.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            eagle.SetBool("takeoff", false);
            eagle.SetBool("fly", false);
            eagle.SetBool("landing", false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            eagle.SetBool("idle", false);
            eagle.SetBool("takeoff", true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            eagle.SetBool("fly", false);
            eagle.SetBool("walk", false);
            eagle.SetBool("flyleft", true);
            eagle.SetBool("turnleft", true);
            eagle.SetBool("idle", false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            eagle.SetBool("fly", false);
            eagle.SetBool("walk", false);
            eagle.SetBool("flyright", true);
            eagle.SetBool("turnright", true);
            eagle.SetBool("idle", false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            eagle.SetBool("landing", true);
            eagle.SetBool("fly", false);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            eagle.SetBool("walk", true);
            eagle.SetBool("idle", false);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            eagle.SetBool("idle", true);
            eagle.SetBool("walk", false);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            eagle.SetBool("glide", true);
            eagle.SetBool("fly", false);
            eagle.SetBool("flyleft", false);
            eagle.SetBool("flyright", false);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            eagle.SetBool("attack", true);
            eagle.SetBool("fly", false);
            eagle.SetBool("glide", false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            eagle.SetBool("hunt", true);
            eagle.SetBool("fly", false);
            eagle.SetBool("glide", false);
        }
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            MainCamera.GetComponent<CameraFollow>().enabled = false;
        }
        if (Input.GetKeyUp(KeyCode.RightControl))
        {
            MainCamera.GetComponent<CameraFollow>().enabled = true;
        }
	}
}
