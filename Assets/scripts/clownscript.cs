using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class ClownController : MonoBehaviour
{
    public float detectionRange = 6f;       // Range to detect the player

    public Transform playerTransform;
    public bool NearPlayer;
    public GameObject BloodSplash;
    public AudioSource audioSource;
    public AudioClip clip;

    private void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) < detectionRange)
        {
            NearPlayer = true;
        }
        else
        {
            NearPlayer = false;
        }
    }

    public void Capture()
    {
        // Create a blood splash effect
        Instantiate(BloodSplash, transform.position, transform.rotation);

        // Play the capture sound
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }

        // Destroy the GameObject after a delay (if you want to wait for the audio to finish playing)
        Destroy(gameObject, clip.length); // Destroys the clown after the clip finishes playing
    }


}
