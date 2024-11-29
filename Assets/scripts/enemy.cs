using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;            
    public float turnSpeed = 100f;          
    public float health = 100f;             
    public float detectionRange = 6f;
    public GameObject cutsceneObject;
    public GameObject VFX_EasyExplosion;
    public Transform playerTransform;  

    public int missionTime = 120;
    public TextMeshProUGUI gameOverText;    
    public TextMeshProUGUI TimeNearText;

    public Transform enemyObject;          
    private Renderer enemyRenderer;
    public Color glowColor = Color.blue;

    private float originalMoveSpeed;           
    private bool isCutsceneTriggered = false;

    private Rigidbody rb;

    public float slowFactor = 0.5f;

    private float timeNearPlayer = 0f; 
    public bool NearPlayer = false;       
    public float requiredTimeToTriggerCutscene = 4f; 

    public BicycleVehicle bicycleVehicle;
    public AudioSource SoundNear;

    public EnemyNavigationController navigationController;
    public TeleportObject enemyTeleporter;
    public TeleportObject playerTeleporter;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        originalMoveSpeed = moveSpeed;
        enemyRenderer = enemyObject.GetComponent<Renderer>();
        enemyTeleporter = enemyObject.GetComponent<TeleportObject>();
        playerTeleporter = GetComponent<TeleportObject>();

        if (navigationController == null)
            navigationController = GetComponent<EnemyNavigationController>();

        if (cutsceneObject != null)
            cutsceneObject.SetActive(false);

        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;  
        }
    }

    private void Update()
    {
        if (!isCutsceneTriggered)
        {
            
            //int buttonPressed = bicycleVehicle.buttonPressed;
            //if(buttonPressed == 1){
              //  TimeNearText.text = "ButtonPRESSED";
            //}
            

            if (Vector3.Distance(transform.position, playerTransform.position) < detectionRange)
            {
                NearPlayer = true;
                UnityEngine.Debug.Log($"EnemyController: NearPlayer = {NearPlayer}");
                TimeNearText.text = $"PRESS THE CAPTURE BUTTON";

                /*
                timeNearPlayer += Time.deltaTime;

                if (TimeNearText != null)
                    TimeNearText.text = $"Time Near: {timeNearPlayer:F1}s";
                    SoundNear.Play();


                // Win the game if time near player reaches the required time or button is pressed
                if (timeNearPlayer >= requiredTimeToTriggerCutscene )
                {
                    enemyhit();
                }
                */
            }
            else
            {
                NearPlayer = false;
                // Reset the timer if the player is no longer within range
                timeNearPlayer = 0f;

                // Reset the display text
                if (TimeNearText != null)
                    TimeNearText.text = "Come Closer to Enemy";
            }
        }
    }

    public void enemyhit()
    {
        gameOverText.text = "YOU WON";
        TriggerCutscene();
    }

    // Trigger cutscene when the player is within detection range for the required time or button is pressed
    public void TriggerCutscene()
    {
        isCutsceneTriggered = true;
        SceneManager.LoadScene("CutsceneCuffing");

        // Show the cutscene UI or camera changes
        if (cutsceneObject != null)
            cutsceneObject.SetActive(true);

        // Start the coroutine to wait and then change scenes
        //StartCoroutine(WaitAndChangeScene(5f)); // Wait for 5 seconds
        playerTeleporter.Teleport();
        enemyTeleporter.Teleport();
    }

    private IEnumerator WaitAndChangeScene(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Change to the previous scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }

    //Take damage if the enemy gets hit by bullets
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("bullet"))
        {
            // Trigger the slow effect in NavigationController
            if (navigationController != null)
            {
                navigationController.ApplySlow(3f, slowFactor); // Slow for 3 seconds at 50% speed
            }

            Explode();
        }
    }

    void Explode()
    {
        if (VFX_EasyExplosion != null)
        {
            GameObject explosion = Instantiate(VFX_EasyExplosion, transform.position, transform.rotation);
            Destroy(explosion, 2f); 
        }
    }
}    

