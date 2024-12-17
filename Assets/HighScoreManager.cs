using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class HighScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI HighScoreText;
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] GameObject textPrefab;
    [SerializeField] TextMeshProUGUI leaderboardNotificationText;

    static int score;
    private int previousScore;
    string playerName;

    private CountDown countDownScript;
    private List<LeaderboardEntry> leaderboard;
    private const int maxLeaderboardEntries = 10;

    private Color originalColor = Color.black;
    private Color greenColor = Color.green;
    private Color redColor = Color.red;

    private int penalty1 = -10;
    private int bonus1 = 10;
    private int bonus2 = 20;

    void Start()
    {
        if (GameStateManager.currentLevel == 1)
        {
            PlayerPrefs.SetInt("score", 0);
            previousScore = 0;
            score = 0;
        }
        else if (GameStateManager.currentLevel != 1)
        {
            score = PlayerPrefs.GetInt("score", 0);
            previousScore = score;
        }
        playerName = PlayerPrefs.GetString("playerName", "Guest");

        switch (GameStateManager.difficulty)
        {
            case GameStateManager.Difficulty.Easy:
                penalty1 = -10;
                bonus1 = 10;
                bonus2 = 20;
                break;

            case GameStateManager.Difficulty.Medium:
                penalty1 = -15;
                bonus1 = 15;
                bonus2 = 25;
                break;

            case GameStateManager.Difficulty.Hard:
                penalty1 = -20;
                bonus1 = 20;
                bonus2 = 30;
                break;

            case GameStateManager.Difficulty.Nightmare:
                penalty1 = -25;
                bonus1 = 25;
                bonus2 = 35;
                break;

            default:
                penalty1 = -10;
                bonus1 = 10;
                bonus2 = 20;
                break;
        }


        leaderboard = LoadLeaderboard();
        UpdateHighScoreText();
        UpdateScoreText();

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            countDownScript = canvas.GetComponent<CountDown>();
        }
    }

    void Update()
    {
        UpdateScoreText();
    }

    public void GetBadGuy()
    {
        if (countDownScript != null)
        {
            int missionTime = countDownScript.getMissionTime();
            int bonusScore = (int) Mathf.Round(missionTime * GetDifficultyMultiplier());
            score += bonusScore;
        }
        CheckHighScore();
        UpdateScoreText();
        UpdateHighScoreText();
    }

    public void hitCar(Vector3 hitPosition)
    {
        score += bonus1;
        CheckHighScore();
        UpdateScoreText();
        UpdateHighScoreText();
        ShowHitText(hitPosition, bonus1);
    }

    public void hitPedestrian(Vector3 hitPosition)
    {
        score += penalty1;
        CheckHighScore();
        UpdateScoreText();
        UpdateHighScoreText();
        ShowHitText(hitPosition, penalty1);
    }

    public void hitEnemy(Vector3 hitPosition)
    {
        score += bonus2;
        CheckHighScore();
        UpdateScoreText();
        UpdateHighScoreText();
        ShowHitText(hitPosition, bonus2);
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

    void CheckHighScore()
    {
        // Update the leaderboard if the player's score is higher than someone else's
        UpdateLeaderboard(playerName, score);
    }

    void UpdateLeaderboard(string playerName, int newScore)
    {
        bool playerExists = false;
        int previousRank = -1;
        for (int i = 0; i < leaderboard.Count; i++)
        {
            if (leaderboard[i].playerName == playerName)
            {
                playerExists = true;
                previousRank = i;
                if (newScore > leaderboard[i].score)
                {
                    leaderboard[i].score = newScore;
                    SaveLeaderboard();
                }
                break;
            }
        }

        if (!playerExists && leaderboard.Count < maxLeaderboardEntries)
        {
            leaderboard.Add(new LeaderboardEntry(playerName, newScore));
            SaveLeaderboard();
        }

        leaderboard.Sort((entry1, entry2) => entry2.score.CompareTo(entry1.score));

        if (leaderboard.Count > maxLeaderboardEntries)
        {
            leaderboard.RemoveAt(leaderboard.Count - 1);
            SaveLeaderboard();
        }

        int newRank = leaderboard.FindIndex(entry => entry.playerName == playerName);

        if (newRank < previousRank)
        {
            string beatenPlayerName = leaderboard[previousRank].playerName;
            StartCoroutine(ShowLeaderboardNotification($"You passed {beatenPlayerName}!"));
        }
    }


    void UpdateScoreText()
    {
        ScoreText.text = $"Score: {score}";
        PlayerPrefs.SetInt("score", score);
    }

    void UpdateHighScoreText()
    {
        if (leaderboard.Count > 0)
        {
            HighScoreText.text = $"HighScore: {leaderboard[0].score}";
        }
        else
        {
            HighScoreText.text = "HighScore: 0";
        }
    }

    void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(new LeaderboardWrapper { entries = leaderboard });
        PlayerPrefs.SetString("Leaderboard", json);
    }

    List<LeaderboardEntry> LoadLeaderboard()
    {
        string json = PlayerPrefs.GetString("Leaderboard", string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            LeaderboardWrapper wrapper = JsonUtility.FromJson<LeaderboardWrapper>(json);
            return wrapper.entries;
        }
        return new List<LeaderboardEntry>();
    }

    IEnumerator ShowLeaderboardNotification(string message)
    {
        leaderboardNotificationText.text = message;
        leaderboardNotificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f); // Show for 3 seconds
        leaderboardNotificationText.gameObject.SetActive(false);
    }

    float GetDifficultyMultiplier()
    {
        switch (GameStateManager.difficulty)
        {
            case GameStateManager.Difficulty.Easy: return 1f;
            case GameStateManager.Difficulty.Medium: return 1.25f;
            case GameStateManager.Difficulty.Hard: return 1.5f;
            case GameStateManager.Difficulty.Nightmare: return 2;
            default: return 1;
        }
    }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;

        public LeaderboardEntry(string name, int score)
        {
            this.playerName = name;
            this.score = score;
        }
    }

    [System.Serializable]
    public class LeaderboardWrapper
    {
        public List<LeaderboardEntry> entries;
    }
}
