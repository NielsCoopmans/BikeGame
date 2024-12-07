using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = UnityEngine.Debug;

public class BicycleVehicle : MonoBehaviour
{
    [Header("Serial Handling")]
    public string portName = "COM3"	;
    public int baudRate = 115200;
    public int readTimeout = 1000;
    public int buttonPressed = 0;
    private SerialManager serialManager;

    public EnemyController enemy;
    public bool isCountdownComplete = false;

    private float lastFireTime = -5f;

    float horizontalInput;
    public float VerticalInput;
    public float verticalInput;
    float steeringInput;

    public TextMeshProUGUI InfoGun; 
    public TextMeshProUGUI InfoButton; 
    public TextMeshProUGUI TimeLeft;

    private readonly object lockObject = new object();
    private bool arduinoData;

    public Transform handle;

    public Gun gun;

    public Vector3 COG;

    [SerializeField] internal float movementSpeed = 8f;
    private float baseSpeed;

    float steeringAngle;
    [SerializeField] float currentSteeringAngle;
    [Range(0f, 0.1f)][SerializeField] float speedteercontrolTime;
    [SerializeField] float maxSteeringAngle;
    [Range(0.000001f, 1)][SerializeField] float turnSmoothing;

    [SerializeField] float maxlayingAngle = 45f;
    public float targetlayingAngle;
    [Range(-40, 40)] public float layingammount;
    [Range(0.000001f, 1)][SerializeField] float leanSmoothing;

    [SerializeField] Transform frontWheeltransform;
    [SerializeField] Transform backWheeltransform;

    public bool frontGrounded;
    public bool rearGrounded;

    [Header("Collision Handling")]
    public float rayDistance = 2f; // Raycast distance for collision detection
    public LayerMask collisionLayer; // Layer for detecting collisions
    public float backwardSpeed = 5f; // Speed to move backward upon collision
    public float backwardDuration = 0.2f; // Duration for moving backward
    private bool isColliding = false;
    private float collisionTimer = 0f;

    [Header("Camera Shake")]
    public Camera mainCamera; // Main camera for shake effect
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.2f;
    private Vector3 originalCameraPosition;

    [Header("Sound Effects")]
    public AudioSource audioSource; // Audio source for sound effects
    public AudioClip collisionSound;

    [Header("Camera Settings")]
    public Transform bikeTransform; // The bike's transform to follow
    public Vector3 cameraOffset = new Vector3(0f, 2f, -5f); // Offset from the bike
    public float smoothFollowSpeed = 0.1f; // Speed of smooth following
    private Vector3 smoothDampVelocity; // For smooth damp calculations

    public EnemyController enemyController;
    public EnemyNavigationController navigationController;

    public Slider reloadBar;
    public Transform playStartPosition;
    public Transform tutorialStartPosition;
    public CountDown countdown;

    public Transform rayOriginObject;  // Reference to the empty GameObject that will act as the ray origin
    public Vector3 boxSize = new Vector3(0.1f, 0.8f, 0.1f); // Size of the box (width, height, depth)
    public Color boxColor = Color.red; // Color for the box visualization

    private HighScoreManager highScoreManager;


    void Start()
    {
        GameObject highScoreManagerObject = GameObject.Find("HighScoreManager");
        if (highScoreManagerObject != null)
        {
            highScoreManager = highScoreManagerObject.GetComponent<HighScoreManager>();
            if (highScoreManager != null)
            {
                UnityEngine.Debug.Log("found higscoremanager in bicycleVehicle");
            }
        }

        baseSpeed = movementSpeed;
        if (GameManager.Instance != null)
        {
            UnityEngine.Debug.Log("GameManager.instance is: " + GameManager.Instance.SkipTutorial);
            UnityEngine.Debug.Log("PlayStartPosition is: " + playStartPosition);
            UnityEngine.Debug.Log("tutorialStartPosition is: " + tutorialStartPosition);

            if (GameManager.Instance.SkipTutorial){
                UnityEngine.Debug.Log("skip tutorial instance, play position is: " + playStartPosition.position);
                bikeTransform.position = playStartPosition.position;
            }
            else
            {
                UnityEngine.Debug.Log("tutorial instance, tutorial position is: " + tutorialStartPosition.position);
                bikeTransform.position = tutorialStartPosition.position;
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("GameManager.Instance is null! Defaulting to tutorialStartPosition.");
            bikeTransform.position = tutorialStartPosition.position;
        }
        if (enemyController == null)
            enemyController = GetComponent<EnemyController>();
        if (navigationController == null)
            navigationController = GetComponent<EnemyNavigationController>();

        serialManager = new SerialManager(portName, baudRate, readTimeout);
    }

    void Update()
    {
        HandleSerialInput();
        if (isColliding)
        {
            HandleBackwardMovement();
            HandleSteering();
            UpdateWheels();
            UpdateHandle();
        }
        else if (isCountdownComplete) 
        {
            HandleEngine();
            HandleSteering();
            UpdateWheels();
            UpdateHandle();
            LayOnTurn();
            CheckForCollision();
            CaptureEnemy();
        }
    }

    private void HandleSerialInput()
    {
        string[] dataParts = serialManager.GetLatestData();

        if (dataParts.Length >= 4)
        {
            ParseInputData(dataParts);
        }
        else
        {
            arduinoData = false;
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            if (Input.GetKeyDown(KeyCode.B))
            {
                gun.ReloadBullets();
                if (enemy != null && enemy.NearPlayer)
                {
                    highScoreManager.GetBadGuy();
                    enemy.TriggerCutscene();
                }
            }
        }
    }

    private void ParseInputData(string[] dataParts)
    {
        if (float.TryParse(dataParts[0], out float parsedSteering))
        {
            steeringInput = -parsedSteering;
        }
        if (float.TryParse(dataParts[1], out float horn))
        {
            if (horn == 1 && (Time.time - lastFireTime) >= 2f)
            {
                gun.FireBullet();
                lastFireTime = Time.time;
                StartCoroutine(ShowCooldown());
            }
        }
        if (float.TryParse(dataParts[2], out float parsedSpeed))
        {
            verticalInput = Mathf.Clamp(parsedSpeed / 8f, 0f, 50f);
        }
        if (int.TryParse(dataParts[3], out int parsedButton))
        {
            buttonPressed = parsedButton;
        }
    }

    public float getBaseSpeed()
    {
        return baseSpeed;
    }

    private float currentSpeed = 0f;
    private float bleedOffSpeed = 1f;

    public void HandleEngine()
    {
        float targetSpeed = verticalInput * movementSpeed * Time.deltaTime;


        if (Mathf.Abs(verticalInput) < 0.01f && currentSpeed > 0)
        {
            currentSpeed = Mathf.Max(currentSpeed - bleedOffSpeed * Time.deltaTime, 0);
        }
        else
        {
            currentSpeed = targetSpeed;
        }

        transform.Translate(Vector3.forward * currentSpeed);
    }

    public void HandleSteering()
    {
        if (arduinoData)
        {
            currentSteeringAngle = steeringInput * 1.25f;

            targetlayingAngle = maxlayingAngle * -steeringInput / maxSteeringAngle;

            transform.Rotate(Vector3.up * currentSteeringAngle * Time.deltaTime);
        }
        else
        {
            if (horizontalInput < 0) 
            {
                currentSteeringAngle = -maxSteeringAngle;
            }
            else if (horizontalInput > 0) 
            {
                currentSteeringAngle = maxSteeringAngle;
            }
            else 
            {
                currentSteeringAngle = 0f;
            }

            targetlayingAngle = maxlayingAngle * -horizontalInput / maxSteeringAngle;

            transform.Rotate(currentSteeringAngle * Time.deltaTime * Vector3.up);
        }
    }
    private void LayOnTurn()
    {
        Vector3 currentRot = transform.rotation.eulerAngles;

        if (Mathf.Abs(currentSteeringAngle) < 0.5f)
        {
            layingammount = Mathf.LerpAngle(layingammount, 0f, leanSmoothing);
        }
        else
        {
            layingammount = Mathf.LerpAngle(layingammount, targetlayingAngle, leanSmoothing);
        }

        transform.rotation = Quaternion.Euler(currentRot.x, currentRot.y, layingammount);
    }

    public void UpdateWheels()
    {
        UpdateSingleWheel(frontWheeltransform);
    }

    public void UpdateHandle()
    {
        Quaternion sethandleRot = frontWheeltransform.rotation;
        handle.localRotation = Quaternion.Euler(handle.localRotation.eulerAngles.x, currentSteeringAngle, handle.localRotation.eulerAngles.z);
    }

    private void UpdateSingleWheel(Transform wheelTransform)
    {
        wheelTransform.localRotation = Quaternion.Euler(new Vector3(0, currentSteeringAngle, 0));
    } 


    private IEnumerator ShowCooldown()
    {
        if (reloadBar != null)
        {
            reloadBar.gameObject.SetActive(true); 
            reloadBar.value = 0;                 

            float cooldownDuration = 2f;         // Same as the cooldown time
            float elapsedTime = 0f;

            while (elapsedTime < cooldownDuration)
            {
                elapsedTime += Time.deltaTime;
                reloadBar.value = elapsedTime / cooldownDuration; // Update slider value
                yield return null;
            }

            reloadBar.gameObject.SetActive(false);
        }
    }

    public bool calledCountdown = false;

    private void CheckForCollision()
    {
        Vector3 rayOrigin = rayOriginObject.position;
        Collider[] hitColliders = Physics.OverlapBox(rayOrigin, boxSize / 2f, transform.rotation, collisionLayer);

        if (hitColliders.Length > 0 && !isColliding)
        {
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("enemy")) 
                {
                    break; 
                }
                else if(hitCollider.CompareTag("portal"))
                {
                    UnityEngine.Debug.Log("Portal activated for " + hitCollider.gameObject.name);
                    
                    RampPortal portalScript = hitCollider.GetComponent<RampPortal>();
                    if (portalScript != null)
                    {
                        portalScript.ActivatePortal();              
                    }
                }
                else if (hitCollider.CompareTag("startEnemy"))
                {
                    navigationController.StartMoving();
                    if(!calledCountdown){
                      countdown.startMissionTimeCountdown();
                      calledCountdown = true;
                      InfoButton.enabled = false;
                      InfoGun.enabled = false;
                      TimeLeft.enabled = true;
                    }
                }
                else
                {
                    collisionTimer = backwardDuration;
                    isColliding = true;

                    PlayCollisionSound();
                    StartCoroutine(CameraShake());
                    break;
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = boxColor;
        Gizmos.matrix = Matrix4x4.TRS(rayOriginObject.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }

    private void HandleBackwardMovement()
    {
        if (collisionTimer > 0)
        {
            transform.Translate(-Vector3.forward * backwardSpeed * Time.deltaTime);
            collisionTimer -= Time.deltaTime;
        }
        else
        {
            isColliding = false;
        }
    }
    private void PlayCollisionSound()
    {
        if (audioSource != null && collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound);
        }
    }
    private IEnumerator CameraShake()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            if (mainCamera != null)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-shakeMagnitude, shakeMagnitude), 0f, 0f);
                mainCamera.transform.localPosition += randomOffset;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    private void CaptureEnemy()
    {          
        if (buttonPressed == 1)
        {
            if (enemy != null && enemy.NearPlayer)
            {
                highScoreManager.GetBadGuy();
                enemy.enemyhit();
            }
        }
    }
    public void OnApplicationQuit()
    {
        serialManager.ClosePort();
    }
}