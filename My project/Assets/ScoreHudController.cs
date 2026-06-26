using TMPro;
using UnityEngine;

public class ScoreHudController : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private int score;

    void Start()
    {
        RefreshScoreText();
    }

    public void AddScore()
    {
        score++;
        RefreshScoreText();
    }

    private void RefreshScoreText()
    {
        scoreText.text = $"Score: {score}";
    }
}