using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;            // Speed of forward movement
    public float turnSpeed = 100f;          // Speed of turning/steering
    public float health = 100f;             // Enemy health
    public float detectionRange = 3f;       // Detection range to trigger the cutscene
    public GameObject cutsceneObject;       // Reference to the cutscene UI or camera
    public Transform playerTransform;       // Reference to the player's transform

    public TextMeshProUGUI gameOverText;    // Text for the game over screen
    public TextMeshProUGUI TimeNearText;

    public Transform enemyObject;           // Reference to the enemy model
    private Renderer enemyRenderer;
    public Color glowColor = Color.blue;

    private float originalMoveSpeed;        // Store the original movement speed
    private bool isSlowed = false;          // Track if the enemy is slowed
    private bool isCutsceneTriggered = false; // Track if the cutscene has been triggered

    private Rigidbody rb;

    // Time-related variables for tracking proximity
    private float timeNearPlayer = 0f;       // Time the player has been near the enemy
    private float requiredTimeToTriggerCutscene = 5f; // Time required for the cutscene to trigger

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
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;  // Assuming the player is tagged as "Player"
        }
    }

    private void Update()
    {
        if (!isCutsceneTriggered)
        {
            // Check if the player is within detection range
            if (Vector3.Distance(transform.position, playerTransform.position) < detectionRange)
            {
                // Increment the time the player stays close
                timeNearPlayer += Time.deltaTime;

                // Display the time on the screen
                if (TimeNearText != null)
                    TimeNearText.text = $"Time Near: {timeNearPlayer:F1}s";

                // Trigger the cutscene if the required time is reached
                if (!isCutsceneTriggered && timeNearPlayer >= requiredTimeToTriggerCutscene)
                {
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

    // Function to apply slow effect
    public void ApplySlow(float duration, float slowFactor)
    {
        if (!isSlowed)
        {
            isSlowed = true;
            moveSpeed *= slowFactor; // Reduce the move speed

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
    private void TriggerCutscene()
    {
        isCutsceneTriggered = true;

        // Show the cutscene UI or camera changes
        if (cutsceneObject != null)
            cutsceneObject.SetActive(true);
        else
        {
            gameOverText.text = "GAME OVER";
        }

        // Start the coroutine to wait and then change scenes
        StartCoroutine(WaitAndChangeScene(5f)); // Wait for 5 seconds
    }

    private System.Collections.IEnumerator WaitAndChangeScene(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Change to the previous scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
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
