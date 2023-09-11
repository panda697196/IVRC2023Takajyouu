using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCollisionDetection : MonoBehaviour
{
    /*
     * Taransform トラッカー
     * Collider コライダー
     * を用意して引数でisArmCllisionDetectionに与えてください
    */
    //----------------------------test用-----------------------------------------------
    // public Transform tracker; // トラッカーのTransform
    // public Collider desiredPositionCollider; // 所定の位置のコライダー
    //
    // private void Update()
    // {
    //     if (isArmCllisionDetection(tracker, desiredPositionCollider))
    //     {
    //         Debug.Log("Ok");
    //     }
    // }
    //-----------------------------------------------------------------------------------

    public bool isArmCllisionDetection(Transform trackerPos, Collider Collider)
    {
        bool _isArmCllisionDetection = false;
        // トラッカーの位置を取得
        Vector3 trackerPosition = trackerPos.position;

        // トラッカーの位置をデバッグログに表示
        // Debug.Log("トラッカーの位置: " + trackerPosition);
        
        // 所定の位置のコライダー内にトラッカーが入ったかどうかを検知
        if (Collider.bounds.Contains(trackerPosition))
        {
            //Debug.Log("腕が所定の位置に移動しました！");
            // ここに移動が検知されたときの処理を追加する
            _isArmCllisionDetection = true;
        }

        return _isArmCllisionDetection;
    }
}

