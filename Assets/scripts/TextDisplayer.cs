using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroTextController : MonoBehaviour
{
    public GameObject controlsObject; // Reference to the Controls GameObject
    public GameObject objectiveObject; // Reference to the Objective GameObject

    private void Start()
    {
        StartCoroutine(DisplayIntroSequence());
    }

    private IEnumerator DisplayIntroSequence()
    {
        controlsObject.SetActive(true);
        objectiveObject.SetActive(false);

        // Wait for 10 seconds
        yield return new WaitForSeconds(10f);

    
        controlsObject.SetActive(false);
        objectiveObject.SetActive(true);

        // Wait for 10 seconds
        yield return new WaitForSeconds(15f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }
}
