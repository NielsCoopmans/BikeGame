using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroTextController : MonoBehaviour
{
    public GameObject controlsObject; // Reference to the Controls GameObject
    public GameObject objectiveObject; // Reference to the Objective GameObject
    public GameObject storyObject;

    private void Start()
    {
        StartCoroutine(DisplayIntroSequence());
    }

    private IEnumerator DisplayIntroSequence()
    {
        // Step 1: Show storyObject
        storyObject.SetActive(true);
        controlsObject.SetActive(false);
        objectiveObject.SetActive(false);
        yield return new WaitForSeconds(5f);

        // Step 2: Show objectiveObject
        storyObject.SetActive(false);
        controlsObject.SetActive(false);
        objectiveObject.SetActive(true);
        yield return new WaitForSeconds(10f);

        // Step 3: Show controlsObject
        storyObject.SetActive(false);
        controlsObject.SetActive(true);
        objectiveObject.SetActive(false);
        yield return new WaitForSeconds(5f);


        // Step 4: Load the next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    /*
    private bool skipToNext = false; // Tracks whether the player clicked to skip

    private void Start()
    {
        StartCoroutine(DisplayIntroSequence());
    }

    private IEnumerator DisplayIntroSequence()
    {
        // Step 1: Show storyObject
        storyObject.SetActive(true);
        controlsObject.SetActive(false);
        objectiveObject.SetActive(false);
        yield return WaitOrSkip(10f);

        // Step 2: Show objectiveObject
        storyObject.SetActive(false);
        controlsObject.SetActive(false);
        objectiveObject.SetActive(true);
        yield return WaitOrSkip(10f);

        // Step 3: Show controlsObject
        storyObject.SetActive(false);
        controlsObject.SetActive(true);
        objectiveObject.SetActive(false);
        yield return WaitOrSkip(10f);
       

        // Step 4: Load the next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator WaitOrSkip(float seconds)
    {
        skipToNext = false;
        float elapsedTime = 0f;

        while (elapsedTime < seconds)
        {
            if (skipToNext)
                break;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void Continue()
    {
        skipToNext = true;
    }

    private void Start()
    {
        // Step 1: Show storyObject
        storyObject.SetActive(true);
        controlsObject.SetActive(false);
        objectiveObject.SetActive(false);
    }

    public void continue1()
    {
        // Step 2: Show objectiveObject
        storyObject.SetActive(false);
        controlsObject.SetActive(false);
        objectiveObject.SetActive(true);
    }

    public void continue2()
    {
        // Step 3: Show controlsObject
        storyObject.SetActive(false);
        controlsObject.SetActive(true);
        objectiveObject.SetActive(false);
        yield return WaitOrSkip(10f);
    }

    public void contineu3()
    {
        // Step 4: Load the next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    */
}
