using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPlayer : MonoBehaviour
{
    public GameObject BigBloodSplash;
    private AudioSource audioSource;
    public AudioClip clip;

    private HighScoreManager highScoreManager;


    // Start is called before the first frame update
    void Start()
    {
        GameObject highScoreManagerObject = GameObject.Find("HighScoreManager");
        if (highScoreManagerObject != null)
        {
            highScoreManager = highScoreManagerObject.GetComponent<HighScoreManager>();
            if (highScoreManager != null)
            {
                //UnityEngine.Debug.Log("found higscoremanager in bicycleVehicle");
            }
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Collision(other));
        }
    }

    IEnumerator Collision(Collider player)
    {
        highScoreManager.hitPedestrian(player.transform.position);

        UnityEngine.Debug.Log("pedestrian collision player");
        BicycleVehicle bicycle = player.GetComponent<BicycleVehicle>();

        float baseSpeed = bicycle.getBaseSpeed();
        float penalty = 5f;

        GameObject splash = Instantiate(BigBloodSplash, transform.position, transform.rotation);
        audioSource.PlayOneShot(clip);

        //dissable meshes and collider, so that only once
        foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            renderer.enabled = false;
        }
        GetComponent<Collider>().enabled = false;
        
        float duration = 2f; 
        
        bicycle.movementSpeed = penalty;
        
        yield return new WaitForSeconds(duration);
        Destroy(splash);
        
        // Restore the speed to its base value
        bicycle.movementSpeed = baseSpeed;
        
        Destroy(gameObject);
        yield break;
    }
}
