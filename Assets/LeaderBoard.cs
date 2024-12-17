using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For Text or TMP_Text
using TMPro; // If you are using TextMeshPro

public class LeaderBoard : MonoBehaviour
{
    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private GameObject leaderboardEntryPrefab;
    [SerializeField] private int maxLeaderboardEntries = 10;

    private List<LeaderboardEntry> leaderboard;

    void Start()
    {
        Debug.Log("Starting Leaderboard Display...");
        //PlayerPrefs.DeleteKey("Leaderboard");

        LoadLeaderboard();

        DisplayLeaderboard();
    }

    void LoadLeaderboard()
    {
        string json = PlayerPrefs.GetString("Leaderboard", string.Empty);

        if (!string.IsNullOrEmpty(json))
        {
            Debug.Log("Loaded leaderboard data: " + json);
            LeaderboardWrapper wrapper = JsonUtility.FromJson<LeaderboardWrapper>(json);
            leaderboard = wrapper.entries;

            Debug.Log($"Loaded {leaderboard.Count} entries from PlayerPrefs.");
        }
        else
        {
            Debug.LogWarning("No leaderboard data found in PlayerPrefs.");
            leaderboard = new List<LeaderboardEntry>();
        }
    }

    void DisplayLeaderboard()
    {
        Debug.Log("Displaying leaderboard...");

        // Clear existing entries
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        if (leaderboard.Count == 0)
        {
            Debug.LogWarning("No entries to display on the leaderboard.");
        }

        // Loop through each entry and display it
        for (int i = 0; i < leaderboard.Count && i < maxLeaderboardEntries; i++)
        {
            LeaderboardEntry entry = leaderboard[i];

            Debug.Log($"Displaying Entry {i + 1}: {entry.playerName} - {entry.score}");

            // Create a new entry from the prefab
            GameObject newEntry = Instantiate(leaderboardEntryPrefab, leaderboardContainer);

            // Check if the child objects for PlayerName and Score exist
            TMP_Text playerNameText = newEntry.transform.Find("PlayerName").GetComponent<TMP_Text>();
            TMP_Text scoreText = newEntry.transform.Find("Score").GetComponent<TMP_Text>();

            if (playerNameText == null || scoreText == null)
            {
                Debug.LogError("PlayerName or Score text not found in the leaderboard entry prefab.");
            }

            playerNameText.text = $"{i + 1}. {entry.playerName}";
            scoreText.text = entry.score.ToString();
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
