using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System;

public class HighScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI HighScoreText;
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] GameObject textPrefab;
    static int score;
    private int previousScore; // To track the score change

    private CountDown countDownScript;

    private Color originalColor = Color.black;
    private Color greenColor = Color.green;
    private Color redColor = Color.red;



    // Start is called before the first frame update
    void Start()
    {
        if (GameStateManager.currentLevel == 1) {
            //reset score at beginnig of game
            PlayerPrefs.SetInt("score", 0);
            previousScore = 0;
        }
        else if (GameStateManager.currentLevel != 1)
        {
            //get score from previous level
            score = PlayerPrefs.GetInt("score", 0);
            previousScore = score;
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
        //store original text color
        originalColor = ScoreText.color;
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
        //PlayerPrefs.SetInt("score", score);
    }

    public void ShowHitText(Vector3 position, int number)
    {
        string text = number.ToString();
        //instantiate the text prefab at the hit position
        GameObject hitText = Instantiate(textPrefab, position, Quaternion.identity);

        //set the text value
        TextMeshPro textMeshPro = hitText.GetComponent<TextMeshPro>();
        textMeshPro.text = text;

        //change the color based on the number
        if (number < 0)
        {
            textMeshPro.color = Color.red;
        }
        else
        {
            textMeshPro.color = Color.green;
        }

        //make the text face the camera
        hitText.transform.LookAt(Camera.main.transform);
        hitText.transform.Rotate(0, 180, 0);

        //start the coroutine to scale the text
        StartCoroutine(ScaleText(hitText.transform));
    }

    IEnumerator ScaleText(Transform textTransform)
    {
        float duration = 1.5f; //duration for the scaling effect
        float elapsedTime = 0f;
        Vector3 initialScale = Vector3.zero; //start from very small
        Vector3 finalScale = new Vector3(0.5f, 0.5f, 0.5f); //end at normal size

        // Generate a random direction in the y-plane
        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0.5f, 1.5f), UnityEngine.Random.Range(-0.5f, 0.5f));

        while (elapsedTime < duration)
        {
            textTransform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / duration);
            // Move the text in the random direction
            textTransform.position += randomDirection * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //ensure the final scale is set
        textTransform.localScale = finalScale;

        //destroy the text
        Destroy(textTransform.gameObject);
    }


    int penalty = 0;
    public void hitCar(Vector3 hitPosition)
    {
        penalty = 10;
        UnityEngine.Debug.Log("hitCar called");
        GameObject CanvasGameObject = GameObject.Find("Canvas");
        score += penalty;
        CheckHighScore();
        UpdateScoreText();
        UpdateHighScoreText();
        ShowHitText(hitPosition, penalty);
    }

    public void hitPedestrian(Vector3 hitPosition)
    {
        penalty = -10;
        UnityEngine.Debug.Log("hitPedestrian called");
        GameObject CanvasGameObject = GameObject.Find("Canvas");
        score = score + penalty;
        CheckHighScore();
        UpdateScoreText();
        UpdateHighScoreText();
        ShowHitText(hitPosition, penalty);
    }

    public void hitEnemy(Vector3 hitPosition)
    {
        penalty = 20;
        UnityEngine.Debug.Log("hitEnemy called");
        GameObject CanvasGameObject = GameObject.Find("Canvas");
        score += penalty;
        CheckHighScore();
        UpdateScoreText();
        UpdateHighScoreText();
        ShowHitText(hitPosition, penalty);
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
        int scoreChange = score - previousScore;

        ScoreText.text = $"Score: {score}";
        PlayerPrefs.SetInt("score", score);

        // Start the blink coroutine based on score change
        if (scoreChange > 0)
        {
            StartCoroutine(BlinkTextColor(greenColor));
        }
        else if (scoreChange < 0)
        {
            StartCoroutine(BlinkTextColor(redColor));
        }

        // Update previous score
        previousScore = score;
    }

    void UpdateHighScoreText()
    {
        HighScoreText.text = $"HighScore: {PlayerPrefs.GetInt("HighScore", 0)}";
    }

    IEnumerator BlinkTextColor(Color targetColor)
    {
        // Change to the target color
        ScoreText.color = targetColor;

        // Wait for a short duration
        yield return new WaitForSeconds(0.25f);

        // Revert to the original color
        ScoreText.color = originalColor;
    }
}
