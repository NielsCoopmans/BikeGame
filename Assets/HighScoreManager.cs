using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI HighScoreText;
    [SerializeField] TextMeshProUGUI ScoreText;
    int score;
    // Start is called before the first frame update
    void Start()
    {
        UpdateHighScoreText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void IncreaseScore(int amount)
    {
        score = amount;
        CheckHighScore();
        UpdateHighScoreText();
    }

    void CheckHighScore() //update highscore
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }

    void UpdateHighScoreText()
    {
        HighScoreText.text = $"HighScore: {PlayerPrefs.GetInt("HighScore", 0)}";
    }
}
