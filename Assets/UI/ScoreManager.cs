using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameControlScript : MonoBehaviour
{
    // UI TextéwíËóp\
    public TextMeshPro resultText;
    public TextMeshPro playerScoreText;
    public TextMeshPro playerRankText;
    public TextMeshPro topText;
    public TextMeshPro top3Text;
    public TextMeshPro commentText;

    // ï\é¶Ç∑ÇÈïœêî
    public int playerScore;
    public int playerRank;
    public int top1;
    public int top2;
    public int top3;
    public string comment;

    // Update is called once per frame
    void Update()
    {
        playerScoreText.text = string.Format("Score:{0}", playerScore);
        playerRankText.text = string.Format("Rank:{0}", playerRank);
        top3Text.text = string.Format("1st {0}\n2nd {0}\n3rd {0}", top1, top2, top3);
        commentText.text = string.Format(comment);
    }
}
