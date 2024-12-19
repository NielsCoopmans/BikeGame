using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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
    public ClownController clown;
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
    public GameObject arrow;

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

    public float targetlayingAngle;
    [Range(-40, 40)] public float layingammount;
    [Range(0.000001f, 1)][SerializeField] float leanSmoothing;

    public bool frontGrounded;
    public bool rearGrounded;
    public bool enemyStarted =false;

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
    public AudioClip shootCar;
    public AudioClip followArrow;
    public AudioClip captureClown;
    public AudioClip goOutside;
    public AudioClip goDown;
    public AudioClip woodenBoxDestroyCollisionSound;
    public AudioClip barrelDestroyCollisionSound;

    private bool shootatcarcollided = false;
    private bool getclowncollided = false;
    private bool goOutsidecollided = false;
    private bool followArrowSpoken = false;
    private bool godowncollided = false;

    [Header("Camera Settings")]
    public Transform bikeTransform; 
    public Vector3 cameraOffset = new Vector3(0f, 2f, -5f); 
    public float smoothFollowSpeed = 0.1f; 

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
        NearInfo.text = "                   ";

        if (highScoreManagerObject != null)
            highScoreManager = highScoreManagerObject.GetComponent<HighScoreManager>();
        
        if (navigationController == null)
            navigationController = GetComponent<EnemyNavigationController>();

        baseSpeed = movementSpeed;
        InitializeVehiclePosition();
        serialManager = new SerialManager(portName, baudRate, readTimeout);
    }

    void Update()
    {
        HandleSerialInput();
        if (Input.GetKeyDown(KeyCode.L))
        {
            OnApplicationQuit();
            SceneManager.LoadScene("Menu");
        }
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
                    bikeTransform.position = playStartPosition.position;
                }
                else
                {
                    bikeTransform.position = tutorialStartPosition.position;
                }
            }
            else
            {
                bikeTransform.position = tutorialStartPosition.position;
            }
        }
        if (GameStateManager.currentLevel == 2){
            NearInfo.text = "Welcome To Level 2!";
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
        float speedDifference = targetSpeed - currentSpeed;
        currentSpeed += speedDifference * Mathf.Exp(-10f * Time.deltaTime);
        transform.Translate(Vector3.forward * currentSpeed);
    }    

    /* old function
    public void HandleEngine()
    {
        float targetSpeed = verticalInput * movementSpeed * Time.deltaTime;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 0.1f);
        transform.Translate(Vector3.forward * currentSpeed);
    } */

    public void HandleSteering()
    {
        if (arduinoData)
        {
            currentSteeringAngle = steeringInput;
            transform.Rotate(1.8f * currentSteeringAngle * Time.deltaTime * Vector3.up);
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
                    //UnityEngine.Debug.Log("Portal activated for " + hitCollider.gameObject.name);

                    RampPortal portalScript = hitCollider.GetComponent<RampPortal>();
                    if (portalScript != null)
                    {
                        portalScript.ActivatePortal();
                    }
                }
                else if (hitCollider.CompareTag("startEnemy"))
                {
                    enemyStarted = true;
                    navigationController.StartMoving();
                    if (!calledCountdown)
                    {
                        countdown.startMissionTimeCountdown();
                        calledCountdown = true;
                        
                        InfoButton.enabled = false;
                        InfoGun.enabled = false;
                        TimeLeft.enabled = true;
                        arrow.SetActive(true);
                    }
                    if (!followArrowSpoken)
                    {
                        audioSource.PlayOneShot(followArrow);
                        followArrowSpoken = true;
                    }
                    NearInfo.text = "Get closer to the enemy!";
                }
                else if (hitCollider.CompareTag("carblock"))
                {
                    NearInfo.text = "Clear the car to go down!";
                    if (!shootatcarcollided)
                    {
                        AudioSource tempAudioSource = hitCollider.gameObject.AddComponent<AudioSource>();
                        tempAudioSource.PlayOneShot(shootCar);
                        shootatcarcollided = true;
                    }     

                }
                else if (hitCollider.CompareTag("clownblock"))
                {
                    NearInfo.text = "Capture the clown!";
                    if (!getclowncollided)
                    {
                        AudioSource tempAudioSource = hitCollider.gameObject.AddComponent<AudioSource>();
                        tempAudioSource.PlayOneShot(captureClown);
                        getclowncollided = true;
                        godowncollided = false;
                    }

                }
                else if (hitCollider.CompareTag("goOutsideblock"))
                {
                    NearInfo.text = "Go Outside!";
                    if (!goOutsidecollided)
                    {
                        AudioSource tempAudioSource = hitCollider.gameObject.AddComponent<AudioSource>();
                        tempAudioSource.PlayOneShot(goOutside);
                        goOutsidecollided = true;
                    }

                }
                else if (hitCollider.CompareTag("goDownblock"))
                {
                    NearInfo.text = "Go Down!";
                    if (!godowncollided)
                    {
                        AudioSource tempAudioSource = hitCollider.gameObject.AddComponent<AudioSource>();
                        tempAudioSource.PlayOneShot(goDown);
                        godowncollided = true;
                    }
                }
                else if (hitCollider.CompareTag("explodableAndSmall"))
                {
                    PlayCollisionSound(hitCollider);
                    Transform parent = hitCollider.transform.parent; // Get the parent of the collided object

                    // Check if the parent exists and also has the same tag
                    if (parent != null && parent.CompareTag("explodableAndSmall"))
                    {
                        // Destroy the parent, which will also destroy the child
                        Destroy(parent.gameObject);
                    }
                    else
                    {
                        // No valid parent; just destroy the collided object
                        Destroy(hitCollider.gameObject);
                    }
                    //Destroy(hitCollider.gameObject);
                    break;
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
                if (poleCollisionSound != null)
                {
                    audioSource.PlayOneShot(poleCollisionSound);
                }
            }
            else if (hitCollider.CompareTag("explodableAndSmall"))
            {
                if (hitCollider.gameObject.name.Contains("Wood") || hitCollider.gameObject.name.Contains("Palet"))
                {
                    if (woodenBoxDestroyCollisionSound != null)
                    {
                        audioSource.PlayOneShot(woodenBoxDestroyCollisionSound);
                    }
                }
                else if (hitCollider.gameObject.name.Contains("Barrel"))
                {
                    if (barrelDestroyCollisionSound != null)
                    {
                        audioSource.PlayOneShot(barrelDestroyCollisionSound);
                    }
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
            else if (clown != null && clown.NearPlayer)
            {
                clown.Capture();
            }
        }
    }

    public void OnApplicationQuit()
    {
        serialManager.ClosePort();
    }
}