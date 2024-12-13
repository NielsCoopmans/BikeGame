using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class cutsceneIntroController : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public float playbackSpeed = 1f; // 2x speed

    // Start is called before the first frame update
    void Start()
    {
        if(playableDirector != null)
            {
                PlayableGraph graph = playableDirector.playableGraph;
                graph.GetRootPlayable(0).SetSpeed(playbackSpeed);
            }
        Invoke("ForceCloseCutscene", 60f);
    }

    void ForceCloseCutscene()
    {
        Debug.Log("CutsceneIntro unloaded. Going to main game");
        SceneManager.LoadScene(2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void closeCutscene()
    {
        Debug.Log("Signal received: Closing CutsceneIntro");
        SceneManager.UnloadSceneAsync(1);
    }
}
