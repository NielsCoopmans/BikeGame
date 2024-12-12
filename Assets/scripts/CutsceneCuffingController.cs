using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ForceCloseCutscene", 16f);
    }

    void ForceCloseCutscene()
    {
        Debug.Log("CutsceneCuffing unloaded. Returning to main menu.");
        if(GameStateManager.currentLevel == 2){
            SceneManager.LoadScene("Menu");
            GameStateManager.currentLevel = 1;
        }
        else{
            GameStateManager.currentLevel = 2;
            SceneManager.LoadScene("BikeGame 2");
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void closeCutscene(){
        Debug.Log("Signal received: Closing CutsceneCuffing");
        SceneManager.UnloadSceneAsync("CutsceneCuffing");
    }
}
