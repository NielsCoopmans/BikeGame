using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;



public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI playGame;
    void Start()
    {
        if (GameStateManager.currentLevel == 2)
            playGame.text = "Level 2";
    }
   public void PlayGame(){
        GameManager.Instance.SkipTutorial = true;
        Debug.Log("play instance is passed on)");
        SceneManager.LoadScene(2);
   }

    public void PlayTutorial(){
        GameManager.Instance.SkipTutorial = false;
        Debug.Log("tutorial instance is passed on)");
        SceneManager.LoadScene(1);
   }

   public void QuitGame()
     {
     Debug.Log("QUIT!");
          #if UNITY_EDITOR
               UnityEditor.EditorApplication.isPlaying = false;
          #else
               Application.Quit();
          #endif
     }
}
