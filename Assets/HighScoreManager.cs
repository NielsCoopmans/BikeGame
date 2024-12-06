using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;

public class HighScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI HighScoreText;
    [SerializeField] TextMeshProUGUI ScoreText;
    static int score;

    private CountDown countDownScript;
    // Start is called before the first frame update
    void Start()
    {
        if (GameStateManager.currentLevel == 1) {
            //reset score at beginnig of game
            PlayerPrefs.SetInt("score", 0);
        }
        else if (GameStateManager.currentLevel != 1)
        {
            //get score from previous level
            score = PlayerPrefs.GetInt("score", 0);
        }
        //UpdateHighScoreText();
        GameObject CanvasGameObject = GameObject.Find("Canvas");
        if (CanvasGameObject != null)
        {
            countDownScript = CanvasGameObject.GetComponent<CountDown>();
            if (countDownScript != null)
            {
                UnityEngine.Debug.Log("found countdownscript");
                UpdateScoreText();
                UpdateHighScoreText();
            }
        }
        // Attempt to find and assign ScoreText if it's null
        if (ScoreText == null)
        {
            GameObject scoreTextObject = GameObject.Find("ScoreText");
            if (scoreTextObject != null)
            {
                ScoreText = scoreTextObject.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                UnityEngine.Debug.Log("ScoreText GameObject not found in the scene!");
                return;
            }
        }
        if (HighScoreText == null)
        {
            GameObject HighScoreTextObject = GameObject.Find("HighScoreText");
            if (HighScoreTextObject != null)
            {
                HighScoreText = HighScoreTextObject.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                UnityEngine.Debug.Log("HighScoreText GameObject not found in the scene!");
                return;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateHighScoreText();
        UpdateScoreText();
        if (Input.GetKeyUp(KeyCode.H))
        {
            GetBadGuy();
        }
    }

    public void GetBadGuy()
    {
        UnityEngine.Debug.Log("GetBadGuy called");
        GameObject CanvasGameObject = GameObject.Find("Canvas");
        if (countDownScript != null)
        {
            int missionTime = countDownScript.getMissionTime();
            UnityEngine.Debug.Log("Mission Time: " + missionTime);
            score += missionTime;
        }
        CheckHighScore();
        UpdateScoreText();
        UpdateHighScoreText();
        PlayerPrefs.SetInt("score", score);
    }

    public void hitCar()
    {
        UnityEngine.Debug.Log("hitCar called");
        GameObject CanvasGameObject = GameObject.Find("Canvas");
        score += 10;
        CheckHighScore();
        UpdateScoreText();
        UpdateHighScoreText();
        PlayerPrefs.SetInt("score", score);
    }

    public void hitPedestrian()
    {
        UnityEngine.Debug.Log("hitPedestrian called");
        GameObject CanvasGameObject = GameObject.Find("Canvas");
        score = score - 10;
        CheckHighScore();
        UpdateScoreText();
        UpdateHighScoreText();
        PlayerPrefs.SetInt("score", score);
    }

    public void hitEnemy()
    {
        UnityEngine.Debug.Log("hitEnemy called");
        GameObject CanvasGameObject = GameObject.Find("Canvas");
        score += 20;
        CheckHighScore();
        UpdateScoreText();
        UpdateHighScoreText();
        PlayerPrefs.SetInt("score", score);
    }

    void CheckHighScore() //update highscore
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }

    void UpdateScoreText()
    {
        

        // Update score text
        //if (score > PlayerPrefs.GetInt("score", 0))
        //{
            ScoreText.text = $"Score: {score}";
            PlayerPrefs.SetInt("score", score);
        //}
        //else
        //{
        //    ScoreText.text = $"Score: {PlayerPrefs.GetInt("score")}";
        //    score = PlayerPrefs.GetInt("score", 0);
        //}
    }

    void UpdateHighScoreText()
    {
        HighScoreText.text = $"HighScore: {PlayerPrefs.GetInt("HighScore", 0)}";
    }
}
