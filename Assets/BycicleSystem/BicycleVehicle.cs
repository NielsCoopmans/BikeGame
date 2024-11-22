using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class BicycleVehicle : MonoBehaviour
{
    public string portName = "/dev/tty.usbmodem1401"	;
    public int baudRate = 115200;
    public int readTimeout = 1000;
    private SerialPort serialPort;
    private Thread serialThread;
    private bool isSerialRunning = false;
    public int buttonPressed = 0;

    private string lastReceivedData = "";
    private float lastFireTime = -5f;

    float horizontalInput;
    public float VerticalInput;
    public float verticalInput;
    float steeringInput;

    private readonly object lockObject = new object(); // For thread-safe access to data
    private bool arduinoData;

    public Transform handle;
    bool braking;

    public Gun gun;

    public Vector3 COG;

    [SerializeField] internal float movementSpeed = 10f;
    [SerializeField] float brakeSpeed = 10f;

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

    [SerializeField] TrailRenderer fronttrail;
    [SerializeField] TrailRenderer rearttrail;

    public bool frontGrounded;
    public bool rearGrounded;

    private bool usingKeyboardInput = false;

    [Header("Collision Handling")]
    public float rayDistance = 2f; // Raycast distance for collision detection
    public LayerMask collisionLayer; // Layer for detecting collisions
    public float backwardSpeed = 5f; // Speed to move backward upon collision
    public float backwardDuration = 0.1f; // Duration for moving backward
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



    void Start()
    {
        StopEmitTrail();

        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = readTimeout;

        try
        {
            serialPort.Open();
            UnityEngine.Debug.Log("Serial port opened successfully.");

            isSerialRunning = true;
            serialThread = new Thread(SerialReadThread);
            serialThread.Start();
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"Failed to open serial port: {ex.Message}");
        }
    }

    void Update()
    {
        if (isColliding)
        {
            HandleBackwardMovement();
        }
        else
        {
            GetInput();
            HandleEngine();
            HandleSteering();
            UpdateWheels();
            UpdateHandle();
            LayOnTurn();
            CheckForCollision();
        }
    }

    private void SerialReadThread()
    {
        while (isSerialRunning && serialPort.IsOpen)
        {
            try
            {
                string arduinoData = serialPort.ReadLine();
                lock (lockObject)
                {
                    lastReceivedData = arduinoData;
                }
            }
            catch (System.TimeoutException)
            {
                // Timeout occurred, skip handling to avoid blocking
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"Serial read error: {ex.Message}");
            }
        }
    }

    public void GetInput()
    {
        string[] dataParts;
        lock (lockObject)
        {
            if (!string.IsNullOrEmpty(lastReceivedData))
            {
                dataParts = lastReceivedData.Trim().Split(',');
            }
            else
            {
                dataParts = new string[0];
            }
        }
        if (dataParts.Length >= 3)
        {
            arduinoData = true;
            usingKeyboardInput = false;

            // Parse steering input
            if (float.TryParse(dataParts[0], out float parsedSteering))
            {
                steeringInput = -parsedSteering;
            }
            else
            {
                UnityEngine.Debug.LogWarning("Steering data could not be parsed to a float.");
            }

            // Parse horn input and fire bullet if cooldown has passed
            if (float.TryParse(dataParts[1], out float horn))
            {
                if (horn == 1 && (Time.time - lastFireTime) >= 2f)
                {
                    gun.FireBullet();
                    lastFireTime = Time.time;
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("Horn data could not be parsed to a float.");
            }

            // Parse speed input
            if (float.TryParse(dataParts[2], out float parsedSpeed))
            {
                float newSpeed = parsedSpeed / 8f;
                verticalInput = Mathf.Clamp(newSpeed, 0f, 50f);
            }
            else
            {
                verticalInput = Input.GetAxis("Vertical"); // Fallback for speed when serial data is incomplete
                UnityEngine.Debug.LogWarning("Speed data could not be parsed.");
            }

            // Parse Buttion input
            if (int.TryParse(dataParts[3], out int parsedButton))
            {
                buttonPressed = parsedButton;
                if(buttonPressed == 1){
                    //UnityEngine.Debug.Log("ButtonPressed");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("Button data could not be parsed to an integer.");
            }
        }
        else
        {
            // Fallback to keyboard input if data from Arduino is incomplete
            arduinoData = false;
            usingKeyboardInput = true;
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            UnityEngine.Debug.LogWarning($"Incomplete data received: '{lastReceivedData}'");
        }
        braking = Input.GetKey(KeyCode.Space);
    }



    public void HandleEngine()
    {
        float speed = verticalInput * movementSpeed * Time.deltaTime;

        if (braking)
        {
            speed = Mathf.Max(speed - brakeSpeed * Time.deltaTime, 0);
        }

        transform.Translate(Vector3.forward * speed);
    }

    public void HandleSteering()
    {
        if (arduinoData)
        {
            currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, steeringInput, turnSmoothing);
            currentSteeringAngle = Mathf.Clamp(currentSteeringAngle, -maxSteeringAngle, maxSteeringAngle);

            targetlayingAngle = maxlayingAngle * -steeringInput / maxSteeringAngle;

            transform.Rotate(Vector3.up * currentSteeringAngle * Time.deltaTime);
        }
        else
        {
            // Apply steering input based on either serial or keyboard input
            if (horizontalInput < 0) // Left turn (Q)
            {
                currentSteeringAngle = -maxSteeringAngle;
            }
            else if (horizontalInput > 0) // Right turn (D)
            {
                currentSteeringAngle = maxSteeringAngle;
            }
            else // No steering (neither Q nor D pressed)
            {
                currentSteeringAngle = 0f;
            }

            // Calculate the target laying angle based on the steering
            targetlayingAngle = maxlayingAngle * -horizontalInput / maxSteeringAngle;

            // Apply the rotation to the bike's Y-axis to simulate steering
            transform.Rotate(Vector3.up * currentSteeringAngle * Time.deltaTime);
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
        UpdateSingleWheel(backWheeltransform);
    }

    public void UpdateHandle()
    {
        Quaternion sethandleRot = frontWheeltransform.rotation;
        handle.localRotation = Quaternion.Euler(handle.localRotation.eulerAngles.x, currentSteeringAngle, handle.localRotation.eulerAngles.z);
    }

    private void EmitTrail()
    {
        fronttrail.emitting = true;
        rearttrail.emitting = true;
    }

    private void StopEmitTrail()
    {
        fronttrail.emitting = false;
        rearttrail.emitting = false;
    }

    private void UpdateSingleWheel(Transform wheelTransform)
    {
        wheelTransform.localRotation = Quaternion.Euler(new Vector3(0, currentSteeringAngle, 0));
    } 

    public Transform rayOriginObject;  // Reference to the empty GameObject that will act as the ray origin
    public Vector3 boxSize = new Vector3(0.1f, 0.8f, 0.1f); // Size of the box (width, height, depth)
    public Color boxColor = Color.red; // Color for the box visualization

    private void CheckForCollision()
    {
        Vector3 rayOrigin = rayOriginObject.position;

        //DrawBox(rayOrigin);

        RaycastHit hit;
        if (Physics.BoxCast(rayOrigin, boxSize / 2f, transform.forward, out hit, Quaternion.identity, rayDistance, collisionLayer))
        {
            if (!isColliding)
            {
                collisionTimer = backwardDuration;
                isColliding = true;

                PlayCollisionSound();
                StartCoroutine(CameraShake());
            }
            
        }
    }

    private void DrawBox(Vector3 origin)
    {
        // Box's half extents
        Vector3 halfExtents = boxSize / 2f;

        // Calculate corners of the box
        Vector3[] corners = new Vector3[8];
        corners[0] = origin + transform.forward * rayDistance + transform.right * halfExtents.x + transform.up * halfExtents.y;
        corners[1] = origin + transform.forward * rayDistance - transform.right * halfExtents.x + transform.up * halfExtents.y;
        corners[2] = origin + transform.forward * rayDistance + transform.right * halfExtents.x - transform.up * halfExtents.y;
        corners[3] = origin + transform.forward * rayDistance - transform.right * halfExtents.x - transform.up * halfExtents.y;
        corners[4] = origin + transform.forward * 0 + transform.right * halfExtents.x + transform.up * halfExtents.y;
        corners[5] = origin + transform.forward * 0 - transform.right * halfExtents.x + transform.up * halfExtents.y;
        corners[6] = origin + transform.forward * 0 + transform.right * halfExtents.x - transform.up * halfExtents.y;
        corners[7] = origin + transform.forward * 0 - transform.right * halfExtents.x - transform.up * halfExtents.y;

        // Draw lines between the corners to visualize the box
        for (int i = 0; i < 4; i++)
        {
            UnityEngine.Debug.DrawLine(corners[i], corners[(i + 1) % 4], boxColor);
            UnityEngine.Debug.DrawLine(corners[i + 4], corners[((i + 1) % 4) + 4], boxColor);
            UnityEngine.Debug.DrawLine(corners[i], corners[i + 4], boxColor);
        }
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
                // Add shake to the camera's local position
                Vector3 randomOffset = new Vector3(Random.Range(-shakeMagnitude, shakeMagnitude), 0f, 0f);
                mainCamera.transform.localPosition += randomOffset;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the camera's position stabilizes (handled by Smooth Follow)
    }


    void OnApplicationQuit()
    {
        isSerialRunning = false;
        Thread.Sleep(100);
        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }


}