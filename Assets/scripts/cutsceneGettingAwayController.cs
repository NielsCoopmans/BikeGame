using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class cutsceneGettingAwayController : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        
        Invoke("ForceCloseCutscene", 20f);
        
        // Start playing the audio after 3 seconds
        StartCoroutine(PlayAudioAfterDelay(3f));
    }

    IEnumerator PlayAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Play();
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

    public void closeCutscene()
    {
        Debug.Log("Signal received: Closing CutsceneGettingAway");
        SceneManager.UnloadSceneAsync(4);
    }
}
