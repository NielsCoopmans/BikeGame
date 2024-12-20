using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;


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
            HighScoreText.text = $"HighScore: {GetTopScoreFromLeaderboard()}";
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

    private int GetTopScoreFromLeaderboard()
    {
        //fetch the leaderboard from PlayerPrefs
        string json = PlayerPrefs.GetString("Leaderboard", string.Empty);

        if (!string.IsNullOrEmpty(json))
        {
            //deserialize the leaderboard
            LeaderboardWrapper wrapper = JsonUtility.FromJson<LeaderboardWrapper>(json);

            if (wrapper.entries != null && wrapper.entries.Count > 0)
            {
                //return the highest score
                return wrapper.entries.OrderByDescending(entry => entry.score).First().score;
            }
        }

        // Return 0 if the leaderboard is empty or not found
        return 0;
    }

    // Wrapper class to deserialize leaderboard data
    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;
    }

    [System.Serializable]
    public class LeaderboardWrapper
    {
        public List<LeaderboardEntry> entries;
    }

}
