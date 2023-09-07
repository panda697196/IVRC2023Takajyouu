using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreReceiver : MonoBehaviour
{
    [SerializeField] private ScoreManager _scoreManager;

    [Header("ランクの閾値")]
    [SerializeField] private int _thresholdOfSS = 90;
    [SerializeField] private int _thresholdOfS = 85;
    [SerializeField] private int _thresholdOfA = 75;
    [SerializeField] private int _thresholdOfB = 60;
    [SerializeField] private int _thresholdOfC = 40;
    [SerializeField] private string _nameOfRankSS = "ハラショー";
    [SerializeField] private string _nameOfRankS = "秀";
    [SerializeField] private string _nameOfRankA = "優";
    [SerializeField] private string _nameOfRankB = "良";
    [SerializeField] private string _nameOfRankC = "可";
    [SerializeField] private string _nameOfRankD = "不可";
    
    private int _score = 0;
    private int[] _rankingScore = {0, 0, 0};
    
    // Start is called before the first frame update
    void Start()
    {
        //ランキングデータの取得
        _rankingScore[0] = PlayerPrefs.GetInt("1stScore", 3);
        _rankingScore[1] = PlayerPrefs.GetInt("2ndScore", 2);
        _rankingScore[2] = PlayerPrefs.GetInt("3rdScore", 1);
    }

    private string DecideRank(int score) //ランクの決定
    {
        if (score >= _thresholdOfSS)
            return (_nameOfRankSS);
        else if (score >= _thresholdOfS)
            return (_nameOfRankS);
        else if (score >= _thresholdOfA)
            return (_nameOfRankA);
        else if (score >= _thresholdOfB)
            return (_nameOfRankB);
        else if (score >= _thresholdOfC)
            return _nameOfRankC;
        else return _nameOfRankD;
    }

    public void GetScore(int score)
    {
        _score = score;
        ReloadRanking(score);
        string rank = DecideRank(score);
        _scoreManager.GetScoreData();//スコア送信
    }

    public void ReloadRanking(int score) //ランキングの更新
    {
        /*ランキング配列の決定*/
        if(score >= _rankingScore[0])
        {
            _rankingScore[2] = _rankingScore[1];
            _rankingScore[1] = _rankingScore[0];
            _rankingScore[0] = score;
        }
        else if(score < _rankingScore[0] && score >= _rankingScore[1])
        {
            _rankingScore[2] = _rankingScore[1];
            _rankingScore[1] = score;
        }
        else if (score < _rankingScore[1] && score >= _rankingScore[2])
        {
            _rankingScore[2] = score;
        }
        else return;

        //Playerprefsに保存
        PlayerPrefs.SetInt("1stScore", _rankingScore[0]);
        PlayerPrefs.SetInt("2ndScore", _rankingScore[1]);
        PlayerPrefs.SetInt("3rdScore", _rankingScore[2]);
        PlayerPrefs.Save();
    }
}
