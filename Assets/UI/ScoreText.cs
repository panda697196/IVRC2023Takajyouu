using UnityEngine;

public class OverHeadMsg : MonoBehaviour
{
    public Transform targetTran;

    private Vector3 YourScoreP;
    private Vector3 YourRankingP;
    private Vector3 Ranking1P;
    private Vector3 Ranking2P;
    private Vector3 Ranking3P;

    void Update()
    {
        transform.position = RectTransformUtility.WorldToScreenPoint(
             Camera.main,
             targetTran.position + Vector3.up)+new Vector2(0,0);
    }
}
