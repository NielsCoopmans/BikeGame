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
    private bool isSlowed = false;          
    private bool isCutsceneTriggered = false; 

    private Rigidbody rb;

    private float timeNearPlayer = 0f;       
    public float requiredTimeToTriggerCutscene = 4f; 

    public BicycleVehicle bicycleVehicle;
    public AudioSource SoundNear;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        originalMoveSpeed = moveSpeed;
        enemyRenderer = enemyObject.GetComponent<Renderer>();

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
                timeNearPlayer += Time.deltaTime;

                if (TimeNearText != null)
                    TimeNearText.text = $"Time Near: {timeNearPlayer:F1}s";
                    SoundNear.Play();


                // Win the game if time near player reaches the required time or button is pressed
                if (timeNearPlayer >= requiredTimeToTriggerCutscene )
                {
                    UnityEngine.Debug.Log("ButtonPressed");
                    gameOverText.text = "YOU WON";
                    TriggerCutscene();
                }
            }
            else
            {
                // Reset the timer if the player is no longer within range
                timeNearPlayer = 0f;

                // Reset the display text
                if (TimeNearText != null)
                    TimeNearText.text = "Time Near: 0s";
            }
        }
    }

    public void ApplySlow(float duration, float slowFactor)
    {
        if (!isSlowed)
        {
            isSlowed = true;
            moveSpeed *= slowFactor; 

            // Start a coroutine to reset the speed after the duration
            StartCoroutine(ResetSpeedAfterDelay(duration));
        }
    }

    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        moveSpeed = originalMoveSpeed; 
        isSlowed = false; 
    }

    // Trigger cutscene when the player is within detection range for the required time or button is pressed
    public void TriggerCutscene()
    {
        isCutsceneTriggered = true;

        // Show the cutscene UI or camera changes
        if (cutsceneObject != null)
            cutsceneObject.SetActive(true);

        // Start the coroutine to wait and then change scenes
        StartCoroutine(WaitAndChangeScene(5f)); // Wait for 5 seconds
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
            ApplySlow(10f, 0.5f); // Example: Slow down the enemy when hit by a bullet
            if (enemyRenderer != null)
            {
                // Set the emission color to blue to make the enemy glow
                enemyRenderer.material.SetColor("_Color", glowColor);
                Explode();
            }
            Destroy(collision.gameObject); // Destroy the bullet after collision
            Explode();
        }
    }

    void Explode()
    {
        void Explode()
    {
        if (VFX_EasyExplosion != null)
        {
            GameObject explosion = Instantiate(VFX_EasyExplosion, transform.position, transform.rotation);
            Destroy(explosion, 2f); 
        }
        Destroy(gameObject); 
    }
    }
}    

