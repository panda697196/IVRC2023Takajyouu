using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmMovementDetector : MonoBehaviour
{
    public GameManager gameManager;

    public Transform trackerWaist; // 
    public float detectionDuration = 0.000001f; // 検出時間（秒）
    public float movementThreshold = 0.05f; // 動きの閾値
    public Vector3 averageMove;
    private float _timeOfStopping;

    private Vector3 _currentTrackerPosition;
    private Vector3 _prevTrackerPosition;
    public List<Vector3> _trackerMovements = new List<Vector3>();
    private bool _isArmStopping = false;
    [SerializeField] private float _timeThreshold = 1.0f;

    private void Start()
    {
        _prevTrackerPosition = trackerWaist.position; // トラッカー位置初期化
        _timeOfStopping = 0;
    }

    public void ArmMovementDetect()
    {
        _currentTrackerPosition = trackerWaist.position;
        Vector3 trackerMovement = (_currentTrackerPosition - _prevTrackerPosition) / Time.deltaTime;
        // 現在のトラッカーの位置をリストに追加する
        _trackerMovements.Add(trackerMovement);

        // 記録された位置の数が、フレーム数に持続時間をかけた数以上である場合
        if (_trackerMovements.Count >= 10)
        {
            // 位置変更の平均値を計算する
            float averageMagnitude = 0;
            for (int i = 0; i < _trackerMovements.Count; i++)
            {
                averageMagnitude += _trackerMovements[i].magnitude;
            }
            averageMagnitude /= _trackerMovements.Count;

            //Debug.Log(averageMagnitude);

            // 平均変化が閾値より大きい場合、左手は静止している時間を初期化するとみなされる
            if (averageMagnitude > movementThreshold)
            {
                _timeOfStopping = 0;
                _isArmStopping = false;
            }

            if (_timeOfStopping >= _timeThreshold)
            {
                _isArmStopping = true;
            }

            // ドロップされたロケーションレコードのリストを空にする
            _trackerMovements.Clear();
            
            //Debug.Log(isArmMoving);

            // 腕の状態をGameManagercに渡す
            gameManager.GetArmStatus(_isArmStopping);
            _timeOfStopping += Time.deltaTime;
        }
    }
}

