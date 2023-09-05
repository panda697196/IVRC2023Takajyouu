using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class ArmAngle : MonoBehaviour
{
    public GameObject gameManager;

    public FlyArmReadyDetection _flyArmReadyDetection;
    
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

    private bool _isFirstReadyOfArm;
    [SerializeField] private bool _isFirstReadyOfArmForFlyFlag;

    private int sceneTarans;

    private float delta = 0;

    public bool flyFlag;

    private bool resetFlyFlag;
    
    //改良版製作用変数
    [SerializeField] private float FlyAngle = 70; //とびたちの角度
    private float kakeru; //spanにかける数
    [SerializeField] private float DeleyTime; //フライフラグをだす遅延時間


    private void Start()
    {
        sceneTarans = gameManager.GetComponent<GameManager>().GetgameSceneState();

        prevPosition1 = tracker1.position;
        prevPosition2 = tracker2.position;
        prevDiffLine = prevPosition2 - prevPosition1;
        // inputGoalPosition = new Vector3(0.0f, 0.0f, 0.0f);
        // goalPosition.position = inputGoalPosition;
        flyFlag = false;
        _isFirstReadyOfArmForFlyFlag = false;
    }

    private void Update()
    {
        _isFirstReadyOfArm = _flyArmReadyDetection.GetIsFirstReadyOfArm();

        if ((sceneTarans == 2 || sceneTarans == 5) && _isFirstReadyOfArm == true)
        {
            _isFirstReadyOfArmForFlyFlag = true;
        }
        // Debug.Log("Pre;"+prevPosition1+"now"+tracker1.position); //Debug用
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

            // Debug.Log(Vector3.SignedAngle(prevDiffLine, currrentDiffLine, axisLineDirection));//Debug用
            // Debug.Log("angle:"+angle);//Debug用
    
            // 条件をチェックしてデバッグメッセージを表示
            // if (Mathf.Abs(angle) >= 30f && Time.deltaTime <= 0.01f)
            if (Mathf.Abs(angle) >= baseAngle && (sceneTarans == 2 || sceneTarans == 5) && _isFirstReadyOfArmForFlyFlag)
            {
                Debug.Log("fly   angle:"+Mathf.Abs(angle));
                // inputGoalPosition = new Vector3(0.0f, 10.0f, -10.0f); //テスト用仮ゴール座標
                // goalPosition.position = inputGoalPosition;//テスト用のゴール位置設定
                // Vector3 tmpGoalPos = tmpGoalPosObj.position;//一時ゴール（赤の球）の位置を変数に代入

                kakeru = FlyAngle / Mathf.Abs(angle);
                DeleyTime = span * kakeru - span;
                flyFlag = true;
                setGoal();//ゴール位置を変更するメソッドに一時ゴール位置を与える
                _isFirstReadyOfArmForFlyFlag = false;
            }

            // 現在の位置情報を保存
            prevPosition1 = currentPosition1;
            prevPosition2 = currentPosition2;
            prevDiffLine = currrentDiffLine;
        
        
            this.delta = 0; //deltaの初期化
        }
    }

    private void setGoal()
    {
        StartCoroutine(WaitForPointTwoSeconds());//何秒か待つ
        goalPosition.position = tmpGoalPosObj.position;//EagleTargetの位置を変更
        Debug.Log("一時ゴールの位置："+tmpGoalPosObj.position);
    }
    
    private IEnumerator WaitForPointTwoSeconds()
    {
        // deley待機
        if (DeleyTime > 0)
        {
            yield return new WaitForSeconds(DeleyTime);
            Debug.Log(DeleyTime+"秒が経過しました。");
        }
        else
        {
            Debug.Log("はやくふりましたね");
        }
        // 時間経過後実行したいコードをここに追加
    }

    public bool GetIsFirstReadyOfArm()
    {
        return _isFirstReadyOfArm;
    }

    public bool GetFlyFlag()
    {
        return flyFlag;
    }
    

}

