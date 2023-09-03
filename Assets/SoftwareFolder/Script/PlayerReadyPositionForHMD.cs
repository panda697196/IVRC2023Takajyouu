using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReadyPositionForHMD : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("コライダー検知");
        if (other.CompareTag("PlayerReadyPositionForHMD"))
        {
            Debug.Log("プレイヤー準備完了");
            // ここで別のコライダーが入った際の処理を行う
        }
    }
}
