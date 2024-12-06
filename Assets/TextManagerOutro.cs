using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TextManagerOutro : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI HighScoreText;
    [SerializeField] TextMeshProUGUI ScoreText;
    // Start is called before the first frame update
    void Start()
    {
        GameObject scoreTextObject = GameObject.Find("ScoreText");
        if (scoreTextObject != null)
        {
            ScoreText = scoreTextObject.GetComponent<TextMeshProUGUI>();
            ScoreText.text = $"Score: {PlayerPrefs.GetInt("score", 0)}";
        }
        else
        {
            UnityEngine.Debug.Log("ScoreText GameObject not found in the scene!");
            return;
        }

        GameObject HighScoreTextObject = GameObject.Find("HighScoreText");
        if (HighScoreTextObject != null)
        {
            HighScoreText = HighScoreTextObject.GetComponent<TextMeshProUGUI>();
            HighScoreText.text = $"HighScore: {PlayerPrefs.GetInt("HighScore", 0)}";
        }
        else
        {
            UnityEngine.Debug.Log("HighScoreText GameObject not found in the scene!");
            return;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
