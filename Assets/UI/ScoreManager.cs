using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // UI Text�w��p\
    public TextMeshPro resultText;
    public TextMeshPro playerScoreText;
    public TextMeshPro playerRankText;
    public TextMeshPro topText;
    public TextMeshPro top3Text;
    public TextMeshPro commentText;

    // �\������ϐ�
    private int _playerScore;
    private string _playerRank;
    private int _top1;
    private int _top2;
    private int _top3;
    private string _comment;

    private void WriteScore()
    {
        playerScoreText.text = string.Format("　得点:{0}", _playerScore);
        playerRankText.text = string.Format("ランク:{0}", _playerRank);
        top3Text.text = string.Format("1st {0}\n2nd {1}\n3rd {2}", _top1, _top2, _top3);
        commentText.text = string.Format(_comment);
    }

    public void GetScoreData(int score, string rank, int rank1st, int rank2nd, int rank3rd)
    {
        _playerScore = score;
        _playerRank = rank;
        _top1 = rank1st;
        _top2 = rank2nd;
        _top3 = rank3rd;
        WriteScore();
    }

}
