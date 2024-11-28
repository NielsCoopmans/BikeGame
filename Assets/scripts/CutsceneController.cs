using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ForceCloseCutscene", 15f);
    }

    void ForceCloseCutscene()
    {
        Debug.Log("CutsceneCuffing unloaded. Returning to main menu.");
        SceneManager.LoadScene("Menu");
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void closeCutscene(){
        Debug.Log("Signal received: Closing CutsceneCuffing");
        SceneManager.UnloadSceneAsync("cutsceneCuffing");
    }
}