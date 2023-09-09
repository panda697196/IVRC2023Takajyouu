using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmMovementDetector : MonoBehaviour
{
    public GameManager gameManager;

    public Transform trackerWaist; // 
    public float detectionDuration = 1.0f; // 検出時間（秒）
    public float movementThreshold = 0.1f; // 動きの閾値

    private List<Vector3> trackerPositions = new List<Vector3>();
    private bool isArmMoving = false;

    private void Update()
    {
        // 現在のトラッカーの位置をリストに追加する
        trackerPositions.Add(trackerWaist.position);

        // 記録された位置の数が、フレーム数に持続時間をかけた数以上である場合
        if (trackerPositions.Count >= Mathf.FloorToInt(detectionDuration / Time.fixedDeltaTime))
        {
            // 位置変更の平均値を計算する
            Vector3 averageMovement = Vector3.zero;
            for (int i = 1; i < trackerPositions.Count; i++)
            {
                averageMovement += trackerPositions[i] - trackerPositions[i - 1];
            }
            averageMovement /= trackerPositions.Count;

            // 平均変化が閾値より小さい場合、左手は静止しているとみなされる
            if (averageMovement.magnitude < movementThreshold)
            {
                isArmMoving = false;
            }
            else
            {
                isArmMoving = true;
            }

            // ドロップされたロケーションレコードのリストを空にする
            trackerPositions.Clear();

            // 腕の状態をGameManagercに渡す
            gameManager.GetArmStatus(isArmMoving);
        }
    }
    
}
