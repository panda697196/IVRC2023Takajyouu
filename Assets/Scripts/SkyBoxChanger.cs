using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;

public class SkyBoxChanger : MonoBehaviour
{
    [SerializeField, ReadOnly] private int _hasStateSkyBox = 0; //skyboxの状態　0:通常、1:夕日

    [SerializeField] private Material _daytimeSkybox; //昼間のスカイボックス
    [SerializeField] private Material _sunsetSkybox;　//夕日にskybox

    void Update()
    {
        // //---------------------Debug-----------------------------------
        // if (Input.GetKeyDown(KeyCode.E)) ChangeSkyBoxToSunset();
        // if (Input.GetKeyDown(KeyCode.Comma)) ChageSkyBoxToDaytime();
        // //-------------------------------------------------------------
    }

    public void ChageSkyBoxToDaytime() // skyboxを昼間に変化させる
    {
        _hasStateSkyBox = 0;
        SetSkyBox(_hasStateSkyBox);
    }

    public void ChangeSkyBoxToSunset() //skyboxを夕日に変化させる
    {
        _hasStateSkyBox = 1;
        SetSkyBox(_hasStateSkyBox);
    }

    private void SetSkyBox(int stateSkyBox)//skyboxを変更する
    {
        switch (stateSkyBox)
        {
            case 0:
                RenderSettings.skybox = _daytimeSkybox;
                
                break;
            case 1:
                RenderSettings.skybox = _sunsetSkybox;
                
                break;
            
        }
    }
}
