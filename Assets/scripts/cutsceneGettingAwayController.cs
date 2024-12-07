using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class cutsceneGettingAwayController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ForceCloseCutscene", 100f);
    }

    void ForceCloseCutscene()
    {
        Debug.Log("CutsceneGettingAway unloaded. Returning to main menu.");
        SceneManager.LoadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void closeCutscene(){
        Debug.Log("Signal received: Closing CutsceneGettingAway");
        SceneManager.UnloadSceneAsync(4);
    }
}
