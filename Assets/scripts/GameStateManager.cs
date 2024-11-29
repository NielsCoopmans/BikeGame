using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance; // Singleton instance
    public int currentLevel; // Global variable to track the level

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}
