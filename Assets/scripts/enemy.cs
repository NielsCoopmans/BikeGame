using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;             // Default move speed
    public float turnSpeed = 100f;
    public float health = 100f;
    public float detectionRange = 6f;       // Range to detect the player

    public GameObject VFX_EasyExplosion;
    public Transform playerTransform;

    public int missionTime = 120;
    
    public TextMeshProUGUI TimeNearText;

    public Transform enemyObject;
    private Renderer enemyRenderer;
    public Color glowColor = Color.blue;

    private float originalMoveSpeed;
    private bool isCutsceneTriggered = false;

    private Rigidbody rb;

    public float slowFactor = 0.5f;         // Factor by which enemy slows down when far
    public float slowDistance = 30f;       // Distance at which the enemy starts slowing

    public bool NearPlayer = false;
    public float requiredTimeToTriggerCutscene = 4f;

    public BicycleVehicle bicycleVehicle;
    public AudioSource SoundNear;

    public Waypoint waypointLevel2;

    public EnemyNavigationController navigationController;
    public TeleportObject enemyTeleporter;
    public TeleportObject playerTeleporter;

    private void Start()
    {

        if (GameStateManager.currentLevel == 2)
        {
            playerTeleporter.Teleport();
            enemyTeleporter.Teleport();
            navigationController.changeWaypoint(waypointLevel2);
        }

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        originalMoveSpeed = navigationController.movementSpeed;
        enemyRenderer = enemyObject.GetComponent<Renderer>();
        enemyTeleporter = enemyObject.GetComponent<TeleportObject>();
        playerTeleporter = GetComponent<TeleportObject>();

        if (navigationController == null)
            navigationController = GetComponent<EnemyNavigationController>();

        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if (!isCutsceneTriggered)
        {
            AdjustMoveSpeedBasedOnDistance();

            if (Vector3.Distance(transform.position, playerTransform.position) < detectionRange)
            {
                NearPlayer = true;
                TimeNearText.text = $"PRESS THE CAPTURE BUTTON";
            }
            else
            {
                NearPlayer = false;
                TimeNearText.text = $"Get closer to the enemy!";
            }
        }
    }

    private void AdjustMoveSpeedBasedOnDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        originalMoveSpeed = navigationController.originalSpeed;
        if (distanceToPlayer > slowDistance)
        {
            moveSpeed = originalMoveSpeed * slowFactor; 
        }
        else if (navigationController.isSlowed)
        {
            moveSpeed = originalMoveSpeed * slowFactor;
        }
        else
        {
            moveSpeed = originalMoveSpeed; 
        }

        if (navigationController != null)
        {
            navigationController.UpdateMoveSpeed(moveSpeed);
        }
    }

    public void enemyhit()
    {
        TriggerCutscene();
    }

    public void TriggerCutscene()
    {
        bicycleVehicle.OnApplicationQuit();
        isCutsceneTriggered = true;
        
        SceneManager.LoadScene(3);
    }
    public void TriggerGameOverCutscene()
    {
        bicycleVehicle.OnApplicationQuit();
        isCutsceneTriggered = true;
        GameStateManager.currentLevel = 1;
        SceneManager.LoadScene("cutsceneGettingAway");
    }

    public void TriggerVictoryCutscene()
    {
        isCutsceneTriggered = true;
        SceneManager.LoadScene(4);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("bullet"))
        {
            if (navigationController != null)
            {
                navigationController.ApplySlow(3f, slowFactor);
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
