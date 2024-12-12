using UnityEngine;

public class DifficultySelector : MonoBehaviour
{
    public void SetEasyDifficulty()
    {
        GameStateManager.difficulty = GameStateManager.Difficulty.Easy;
        Debug.Log("Difficulty set to Easy");
    }

    public void SetMediumDifficulty()
    {
        GameStateManager.difficulty = GameStateManager.Difficulty.Medium;
        Debug.Log("Difficulty set to Medium");
    }

    public void SetHardDifficulty()
    {
        GameStateManager.difficulty = GameStateManager.Difficulty.Hard;
        Debug.Log("Difficulty set to Hard");
    }

    public void SetNightmareDifficulty()
    {
        GameStateManager.difficulty = GameStateManager.Difficulty.Nightmare;
        Debug.Log("Difficulty set to Nightmare");
    }
}
