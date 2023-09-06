using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameControlScript : MonoBehaviour
{
    // UI Text�w��p\
    public TextMeshPro resultText;
    public TextMeshPro playerScoreText;
    public TextMeshPro playerRankText;
    public TextMeshPro topText;
    public TextMeshPro top3Text;
    public TextMeshPro commentText;

    // �\������ϐ�
    public int playerScore;
    public int playerRank;
    public int top1;
    public int top2;
    public int top3;
    public string comment;

    // Update is called once per frame
    void Update()
    {
        playerScoreText.text = string.Format("得点:{0}", playerScore);
        playerRankText.text = string.Format("順位:{0}", playerRank);
        top3Text.text = string.Format("1位 {0}\n2位 {1}\n3位 {2}", top1, top2, top3);
        commentText.text = string.Format(comment);
    }
}
