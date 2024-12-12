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
    public string portName = "/dev/tty.usbmodem11101"	;
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
    public TextMeshProUGUI NearInfo;

    private bool arduinoData;

    public Transform handle;

    public Gun gun;

    [SerializeField] internal float movementSpeed = 8f;
    private float baseSpeed;

    float steeringAngle;
    [SerializeField] float currentSteeringAngle;
    [SerializeField] float maxSteeringAngle;

    [SerializeField] Transform frontWheeltransform;
    [SerializeField] Transform backWheeltransform;

    [Range(0.000001f, 1)][SerializeField] float turnSmoothing;

    [SerializeField] float maxlayingAngle = 45f;
    public float targetlayingAngle;
    [Range(-40, 40)] public float layingammount;
    [Range(0.000001f, 1)][SerializeField] float leanSmoothing;

    public bool frontGrounded;
    public bool rearGrounded;

    [Header("Collision Handling")]
    public float rayDistance = 2f; 
    public LayerMask collisionLayer; 
    public float backwardSpeed = 2f; 
    public float backwardDuration = 0.01f; 
    private bool isColliding = false;
    private float collisionTimer = 0f;

    [Header("Camera Shake")]
    public Camera mainCamera; 
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.2f;

    [Header("Sound Effects")]
    public AudioSource audioSource; 
    public AudioClip collisionSound;
    public AudioClip bushCollisionSound;
    public AudioClip poleCollisionSound;
    public AudioClip carAlarmSound;

    [Header("Camera Settings")]
    public Transform bikeTransform; 
    public Vector3 cameraOffset = new Vector3(0f, 2f, -5f); 
    public float smoothFollowSpeed = 0.1f; 

    public EnemyController enemyController;
    public EnemyNavigationController navigationController;

    public Slider reloadBar;
    public Transform playStartPosition;
    public Transform tutorialStartPosition;
    public CountDown countdown;

    public Transform rayOriginObject; 
    public Vector3 boxSize = new Vector3(0.1f, 0.8f, 0.1f); 
    public Color boxColor = Color.red; 

    private HighScoreManager highScoreManager;


    void Start()
    {
        GameObject highScoreManagerObject = GameObject.Find("HighScoreManager");

        if (highScoreManagerObject != null)
            highScoreManager = highScoreManagerObject.GetComponent<HighScoreManager>();
        if (enemyController == null)
            enemyController = GetComponent<EnemyController>();
        if (navigationController == null)
            navigationController = GetComponent<EnemyNavigationController>();

        baseSpeed = movementSpeed;
        InitializeVehiclePosition();
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

    private void InitializeVehiclePosition()
    {
        if (GameStateManager.currentLevel == 1)
        {
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.SkipTutorial)
                {
                    NearInfo.text = "Get closer to the enemy!";
                    bikeTransform.position = playStartPosition.position;
                }
                else
                {
                    NearInfo.text = "Leave The Garage!";
                    bikeTransform.position = tutorialStartPosition.position;
                }
            }
            else
            {
                bikeTransform.position = tutorialStartPosition.position;
            }
        }
    }

    private void HandleSerialInput()
    {
        string[] dataParts = serialManager.GetLatestData();

        if (dataParts.Length >= 4)
        {
            ParseInputData(dataParts);
            arduinoData = true;
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

    public void HandleEngine()
    {
        float targetSpeed = verticalInput * movementSpeed * Time.deltaTime;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 0.1f);
        transform.Translate(Vector3.forward * currentSpeed);
    }

    public void HandleSteering()
    {
        if (arduinoData)
        {
            currentSteeringAngle = steeringInput;
            transform.Rotate(1.60f * currentSteeringAngle * Time.deltaTime * Vector3.up);
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
                else if (hitCollider.CompareTag("portal"))
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
                    if (!calledCountdown)
                    {
                        countdown.startMissionTimeCountdown();
                        calledCountdown = true;
                        NearInfo.text = "Get closer to the enemy!";
                        InfoButton.enabled = false;
                        InfoGun.enabled = false;
                        TimeLeft.enabled = true;
                    }
                }
                else
                {
                    collisionTimer = backwardDuration;
                    isColliding = true;

                    PlayCollisionSound(hitCollider); 
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
    private void PlayCollisionSound(Collider hitCollider)
    {
        if (audioSource != null)
        {
            if (hitCollider.CompareTag("Bush"))
            {
                if (bushCollisionSound != null)
                {
                    audioSource.PlayOneShot(bushCollisionSound);
                }
            }
            else if (hitCollider.CompareTag("Pole"))
            {
                if (bushCollisionSound != null)
                {
                    audioSource.PlayOneShot(poleCollisionSound);
                }
            }
            else
            {
                if (collisionSound != null)
                {
                    audioSource.PlayOneShot(collisionSound);
                     if (hitCollider.CompareTag("car")){
                        AudioSource tempAudioSource = hitCollider.gameObject.AddComponent<AudioSource>();
                        tempAudioSource.spatialBlend = 1.0f;          
                        tempAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
                        tempAudioSource.minDistance = 1f;            
                        tempAudioSource.maxDistance = 50f;           
                        tempAudioSource.playOnAwake = false;         
                        tempAudioSource.PlayOneShot(carAlarmSound);
                        tempAudioSource.Play();
                    }
                }
            }
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