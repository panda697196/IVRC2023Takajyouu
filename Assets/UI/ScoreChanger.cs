using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreChanger : MonoBehaviour
{
    [SerializeField] private ScoreReceiver _scoreReceiver;
    
    [Header("スコア参照")]
    [SerializeField] private bool _isShowRanking = false;

    [Header(("スコア変更"))]
    [SerializeField] private bool _isChangeAllScore = false;
    [SerializeField] private bool _isChange2nd3rdScore = false;
    [SerializeField] private int _score1st = 90;
    [SerializeField] private int _score2nd = 80;
    [SerializeField] private int _score3rd = 70;

    private int score1stOriginal = 0;
    private int score2ndOriginal = 0;
    private int score3rdOriginal = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        bool isScoreManage = _isShowRanking || _isChangeAllScore || _isChange2nd3rdScore;

        if (isScoreManage)
        {
            score1stOriginal = _scoreReceiver.GetRankingScore(1);
            score2ndOriginal = _scoreReceiver.GetRankingScore(2);
            score3rdOriginal = _scoreReceiver.GetRankingScore(3);
        }

        if (_isChange2nd3rdScore)
        {
            score2ndOriginal = _score2nd;
            score3rdOriginal = _score3rd;
        }

        if (_isChangeAllScore)
        {
            score1stOriginal = _score1st;
            score2ndOriginal = _score2nd;
            score3rdOriginal = _score3rd;
        }
            
        if (isScoreManage)
        {
            Debug.Log("1st : " + score1stOriginal + "\n2nd : " + score2ndOriginal + "\n3rd : " + score3rdOriginal);
            
            PlayerPrefs.SetInt("1stScore", score1stOriginal);
            PlayerPrefs.SetInt("2ndScore", score2ndOriginal);
            PlayerPrefs.SetInt("3rdScore", score3rdOriginal);
            PlayerPrefs.Save();
            
            Debug.LogWarning("ScoreChangerにより停止されました");
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
        }
    }
}
