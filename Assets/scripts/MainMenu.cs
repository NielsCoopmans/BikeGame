using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame(){
        GameManager.Instance.SkipTutorial = true;
        Debug.Log("play instance is passed on)");
        SceneManager.LoadScene("BikeGame2");
   }

    public void PlayTutorial(){
        GameManager.Instance.SkipTutorial = false;
        Debug.Log("tutorial instance is passed on)");
        SceneManager.LoadScene("cutsceneIntro");
   }

   public void QuitGame (){
        Debug.Log("QUIT!)");
        Application.Quit();
   }
}
