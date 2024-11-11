using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;            // Speed of forward movement
    public float turnSpeed = 100f;          // Speed of turning/steering
    public float health = 100f;             // Enemy health
    public float detectionRange = 6f;       // Detection range to trigger the cutscene
    public GameObject cutsceneObject;       // Reference to the cutscene UI or camera
    public Transform playerTransform;       // Reference to the player's transform

    public int missionTime = 60;
    
   

    public TextMeshProUGUI gameOverText;    // Text for the game over screen
    public TextMeshProUGUI TimeNearText;

    public Transform enemyObject;           // Reference to the enemy model
    private Renderer enemyRenderer;
    public Color glowColor = Color.blue;

    private float originalMoveSpeed;        
    private bool isSlowed = false;          
    private bool isCutsceneTriggered = false; 

    private Rigidbody rb;

    private float timeNearPlayer = 0f;       
    public float requiredTimeToTriggerCutscene = 2f; 



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
            
            if (Vector3.Distance(transform.position, playerTransform.position) < detectionRange)
            {
            
                timeNearPlayer += Time.deltaTime;

                if (TimeNearText != null)
                    TimeNearText.text = $"Time Near: {timeNearPlayer:F1}s";

                if (!isCutsceneTriggered && timeNearPlayer >= requiredTimeToTriggerCutscene)
                {
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

    private System.Collections.IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        moveSpeed = originalMoveSpeed; // Restore the original speed
        isSlowed = false; // Reset the slowed state
    }

    // Trigger cutscene when the player is within detection range for the required time
    public void TriggerCutscene()
    {
        isCutsceneTriggered = true;

        // Show the cutscene UI or camera changes
        if (cutsceneObject != null)
            cutsceneObject.SetActive(true);

        // Start the coroutine to wait and then change scenes
        StartCoroutine(WaitAndChangeScene(5f)); // Wait for 5 seconds
    }

    private System.Collections.IEnumerator WaitAndChangeScene(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Change to the previous scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }

    // Optional: Take damage if the enemy gets hit by bullets
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("bullet"))
        {
            ApplySlow(0.1f, 2); // Example: Slow down the enemy when hit by a bullet
            if (enemyRenderer != null)
            {
                // Set the emission color to blue to make the enemy glow
                enemyRenderer.material.SetColor("_EmissionColor", glowColor);
            }
            Destroy(collision.gameObject); // Destroy the bullet after collision
        }
    }

    
}
