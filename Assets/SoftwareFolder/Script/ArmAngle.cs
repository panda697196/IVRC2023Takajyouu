using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class ArmAngle : MonoBehaviour
{
    public GameObject gameManager;
    
    public Transform tracker1; // トラッカー1のTransformコンポーネント
    public Transform tracker2; // トラッカー2のTransformコンポーネント

    public float baseAngle = 65f;
    public float span = 0.3f;

    public Transform goalPosition;
    public Vector3 inputGoalPosition;
    public Transform tmpGoalPosObj;

    private Vector3 prevPosition1;
    private Vector3 prevPosition2;
    private Vector3 prevDiffLine;
    private float angle;

    private int sceneTarans;

    private float delta = 0;

    public bool flyFlag;

    private bool resetFlyFlag;

    private void Start()
    {
        sceneTarans = gameManager.GetComponent<GameManager>().GetgameSceneState();

        prevPosition1 = tracker1.position;
        prevPosition2 = tracker2.position;
        prevDiffLine = prevPosition2 - prevPosition1;
        inputGoalPosition = new Vector3(0.0f, 0.0f, 0.0f);
        goalPosition.position = inputGoalPosition;
        flyFlag = false;
    }

    private void Update()
    {
        Debug.Log("Pre;"+prevPosition1+"now"+tracker1.position);
        sceneTarans = gameManager.GetComponent<GameManager>().GetgameSceneState();
        Debug.Log(sceneTarans);
        // if (sceneTarans == 2 || sceneTarans == 5)
        delta += Time.deltaTime;
            if (delta > span)
            {
                // トラッカーの現在の位置情報を取得
                Vector3 currentPosition1 = tracker1.position;
                Vector3 currentPosition2 = tracker2.position;

                // 前フレームからの位置変化を計算
                Vector3 positionDiff1 = currentPosition1 - prevPosition1;
                Vector3 positionDiff2 = currentPosition2 - prevPosition2;
                Vector3 axisLineDirection = positionDiff2 - positionDiff1;
            
            
            
                Vector3 currrentDiffLine = currentPosition1 - currentPosition2;
            

                // 2点を結ぶ直線の向きベクトルを計算
                Vector3 lineDirection = currentPosition2 - currentPosition1;

                // 直線の角速度を計算
                // float angle = Vector3.SignedAngle(positionDiff1, positionDiff2, lineDirection) / delta;
                angle = Vector3.SignedAngle(prevDiffLine, currrentDiffLine, axisLineDirection);

                Debug.Log(Vector3.SignedAngle(prevDiffLine, currrentDiffLine, axisLineDirection));
                Debug.Log("angle:"+angle);
        
                // 条件をチェックしてデバッグメッセージを表示
                // if (Mathf.Abs(angle) >= 30f && Time.deltaTime <= 0.01f)
                if (Mathf.Abs(angle) >= baseAngle && (sceneTarans == 2 || sceneTarans == 5))
                {
                    flyFlag = true;
                    Debug.Log("fly");
                    // inputGoalPosition = new Vector3(0.0f, 10.0f, -10.0f); //テスト用仮ゴール座標
                    goalPosition.position = inputGoalPosition;//
                    Vector3 tmpGoalPos = tmpGoalPosObj.position;
                    setGoal(tmpGoalPos);
                }

                // 現在の位置情報を保存
                prevPosition1 = currentPosition1;
                prevPosition2 = currentPosition2;
                prevDiffLine = currrentDiffLine;
            
            
                this.delta = 0; //deltaの初期化
            }
    }

    private void setGoal(Vector3 tmpGoalPos)
    {
        Debug.Log("一時ゴールの位置："+tmpGoalPos);
        goalPosition.position = tmpGoalPos;
    }
    

}

