using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ClownController : MonoBehaviour
{
    public float detectionRange = 6f;       // Range to detect the player

    public GameObject VFX_EasyExplosion;
    public Transform playerTransform;

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
                NearPlayer = true;
            }
            else
            {
                NearPlayer = false;
            }
        }
    }

}
