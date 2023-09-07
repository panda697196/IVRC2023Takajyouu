using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class ArmAngle_v2 : MonoBehaviour
{
    public GameManager gameManager;
    // [SerializeField] private GameObject CanFlyArea;

    public FlyArmReadyDetection _flyArmReadyDetection;
    [SerializeField] private ArmCollisionDetection _armCollisionDetection; 
    
    public Transform trackerWaist; // トラッカー1のTransformコンポーネント
    // public Transform tracker2; // トラッカー2のTransformコンポーネント

    public float baseAngle = 65f;
    public float span = 0.01f;

    public Transform goalPosition;
    public Vector3 inputGoalPosition;
    public Transform tmpGoalPosObj;

    private Vector3 prevPosition1;
    // private Vector3 prevPosition2;
    // private Vector3 prevDiffLine;
    private float angle;

    private bool _isFirstReadyOfArm;
    [SerializeField] private bool _isFirstReadyOfArmForFlyFlag;

    private int sceneTarans;

    private float delta = 0;

    public bool flyFlag = false;

    private bool resetFlyFlag;

    //改良版製作用変数
    [SerializeField] private float FlyAngle = 70; //とびたちの角度
    private float kakeru; //spanにかける数
    [SerializeField] private float DeleyTime; //フライフラグをだす遅延時間

    //改良版2
    [SerializeField] private float TrackerSpeed;
    private List<float> queueArray = new List<float>();
    [SerializeField] private int maxSize = 10;//キューの最大サイズ
    [SerializeField] private float BaseSpeed = 0.8f;
    [SerializeField] private float AvarageSpeed;
    private bool IsInFlyArea;
    private bool IsCanFlyAreaOn = false;
    // [SerializeField] private Collider CanFlyAreaCllider;
    [SerializeField] private Transform NewParentsObj;
    [SerializeField] private Vector3 localOffset = new Vector3(0.180000007f,0,1.39999998f);

    [SerializeField] private float DeleyTimeForMan = 1.5f;



    private void Start()
    {
        sceneTarans = gameManager.GetComponent<GameManager>().GetgameSceneState();

        prevPosition1 = trackerWaist.position;
        // prevPosition2 = tracker2.position;
        // prevDiffLine = prevPosition2 - prevPosition1;
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
            Vector3 currentPosition1 = trackerWaist.position;
            // Vector3 currentPosition2 = tracker2.position;

            // 前フレームからの位置変化を計算
            Vector3 positionDiff1 = (currentPosition1 - prevPosition1) / span;

            TrackerSpeed = positionDiff1.magnitude;
            
            // Debug.Log(TrackerSpeed);
            
            Enqueue(TrackerSpeed);
            // Debug.Log("きゅーのかず："+queueArray.Count);

            if (queueArray.Count >= maxSize)
            {
                // Debug.Log("ok?");
                float tmpSum = 0.0f;
                
                for (int i = 0; i < maxSize; i++)
                {
                    tmpSum += queueArray[i];
                }

                AvarageSpeed = tmpSum / maxSize;
                Debug.Log("AvarageSpeed : " + AvarageSpeed);
            }

            if ((sceneTarans == 2 || sceneTarans == 5) && _isFirstReadyOfArmForFlyFlag == true && IsCanFlyAreaOn == false)
            {
                // CanFlyArea.transform.parent = null;
                // CanFlyArea.SetActive(true);
                IsCanFlyAreaOn = true;
            }
            
            // if (CanFlyAreaCllider.bounds.Contains(trackerWaist.position)) //FlyAreaにいるならばTrue
            // {
            //     IsInFlyArea = true;
            //     Debug.Log("InFlyArea");
            // }
            // else
            // {
            //     IsInFlyArea = false;
            // }
            
            // if (AvarageSpeed >= BaseSpeed && (sceneTarans == 2 || sceneTarans == 5) && _isFirstReadyOfArmForFlyFlag == true && 
                // IsInFlyArea)
            if (AvarageSpeed >= BaseSpeed && (sceneTarans == 2 || sceneTarans == 5) && _isFirstReadyOfArmForFlyFlag == true)
            {
                // StartCoroutine(WaitTime(DeleyTimeForMan));
                flyFlag = true;
                Invoke(nameof(SetGoal), 1f);
                _isFirstReadyOfArmForFlyFlag = false;
                // CanFlyArea.SetActive(false);
                // CanFlyArea.transform.SetParent(NewParentsObj);
                // CanFlyArea.transform.localPosition = localOffset;
            }

            // Vector3 positionDiff2 = currentPosition2 - prevPosition2;
            // Vector3 axisLineDirection = positionDiff2 - positionDiff1;
        
                    
        
            // Vector3 currrentDiffLine = currentPosition1 - currentPosition2;
        

            // 2点を結ぶ直線の向きベクトルを計算
            // Vector3 lineDirection = currentPosition2 - currentPosition1;

            // 直線の角速度を計算
            // float angle = Vector3.SignedAngle(positionDiff1, positionDiff2, lineDirection) / delta;
            // angle = Vector3.SignedAngle(prevDiffLine, currrentDiffLine, axisLineDirection);

            // Debug.Log(Vector3.SignedAngle(prevDiffLine, currrentDiffLine, axisLineDirection));//Debug用
            // Debug.Log("angle:"+angle);//Debug用
    
            // 条件をチェックしてデバッグメッセージを表示
            // if (Mathf.Abs(angle) >= 30f && Time.deltaTime <= 0.01f)
            //-----------------------------------------------------------------------
            // if (Mathf.Abs(angle) >= baseAngle && (sceneTarans == 2 || sceneTarans == 5) && _isFirstReadyOfArmForFlyFlag)
            // {
            //     Debug.Log("fly   angle:"+Mathf.Abs(angle));
            //     // inputGoalPosition = new Vector3(0.0f, 10.0f, -10.0f); //テスト用仮ゴール座標
            //     // goalPosition.position = inputGoalPosition;//テスト用のゴール位置設定
            //     // Vector3 tmpGoalPos = tmpGoalPosObj.position;//一時ゴール（赤の球）の位置を変数に代入
            //
            //     kakeru = FlyAngle / Mathf.Abs(angle);
            //     DeleyTime = span * kakeru - span;
            //     flyFlag = true;
            //     setGoal();//ゴール位置を変更するメソッドに一時ゴール位置を与える
            //     _isFirstReadyOfArmForFlyFlag = false;
            // }
            //----------------------------------------------------------------------------------

            // 現在の位置情報を保存
            prevPosition1 = currentPosition1;
            // prevPosition2 = currentPosition2;
            // prevDiffLine = currrentDiffLine;
        
        
            this.delta = 0; //deltaの初期化
        }
    }
    
    private void SetGoal()
    {
        //StartCoroutine(WaitForPointTwoSeconds());//何秒か待つ
        gameManager.SetEagleTarget(tmpGoalPosObj.position);
        //goalPosition.position = tmpGoalPosObj.position;//EagleTargetの位置を変更
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
    
    public void Enqueue(float item)//キューに追加するメソッド
    {
        // Debug.Log("院キュー:"+item);
        queueArray.Add(item);

        // キューの最大サイズを超えた場合、古い要素を削除
        if (queueArray.Count > maxSize)
        {
            Dequeue();
        }
    }

    // 先頭の要素を取り出すメソッド（Dequeue相当）
    public float Dequeue()
    {
        // Debug.Log("deキュー:");
        if (queueArray.Count == 0)
        {
            Debug.LogWarning("キューが空です。");
            return -1; // エラー値またはデフォルト値を返す
        }

        var item = queueArray[0];
        queueArray.RemoveAt(0);
        return item;
    }
    

}

