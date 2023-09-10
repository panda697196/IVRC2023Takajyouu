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

    public void ChangeSkyBoxToDaytime() // skyboxを昼間に変化させる
    {
        _hasStateSkyBox = 0;//skyboxを昼間に変更
        SetSkyBox(_hasStateSkyBox);//skyboxの適応
    }

    public void ChangeSkyBoxToSunset() //skyboxを夕日に変化させる
    {
        _hasStateSkyBox = 1;//skyboxを夕日に変更
        SetSkyBox(_hasStateSkyBox);//skyboxの適応
    }

    private void SetSkyBox(int stateSkyBox)//skyboxを変更する
    {
        switch (stateSkyBox)
        {
            case 0://昼間のskybox
                RenderSettings.skybox = _daytimeSkybox;//skyboxの設定
                
                break;
            case 1://夕日のskybox
                RenderSettings.skybox = _sunsetSkybox;//skyboxの設定
                
                break;
        }
    }
}
