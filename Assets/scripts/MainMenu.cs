using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame(){
        GameManager.Instance.SkipTutorial = true;
        Debug.Log("play instance is passed on)");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
   }

    public void PlayTutorial(){
        GameManager.Instance.SkipTutorial = false;
        Debug.Log("tutorial instance is passed on)");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
   }

   public void QuitGame (){
        Debug.Log("QUIT!)");
        Application.Quit();
   }
}
