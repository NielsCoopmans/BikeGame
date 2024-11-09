using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class BicycleVehicle : MonoBehaviour
{
    public string portName = "/dev/tty.usbmodem11401";
    public int baudRate = 115200;
    public int readTimeout = 1000;
    private SerialPort serialPort;
    private Thread serialThread;
    private bool isSerialRunning = false;

    private string lastReceivedData = "";
    private float lastFireTime = -5f;

    float horizontalInput;
    float VerticalInput;
    float verticalInput;
    float steeringInput;

    private readonly object lockObject = new object();

    public Transform handle;
    bool braking;

    public Gun gun;

    public Vector3 COG;

    [SerializeField] internal float movementSpeed = 10f;
    [SerializeField] float brakeSpeed = 5f;

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

    void Start()
    {
        StopEmitTrail();

        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = readTimeout;

        try
        {
            serialPort.Open();
            Debug.Log("Serial port opened successfully.");

            isSerialRunning = true;
            serialThread = new Thread(SerialReadThread);
            serialThread.Start();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to open serial port: {ex.Message}");
        }
    }

    void Update()
    {
        GetInput();
        HandleEngine();
        HandleSteering();
        UpdateWheels();
        UpdateHandle();
        LayOnTurn();
        EmitTrail();
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
                Debug.LogError($"Serial read error: {ex.Message}");
            }
        }
    }

    public void GetInput()
    {
        string[] dataParts;

        lock (lockObject)
        {
            dataParts = lastReceivedData.Trim().Split(',');
        }

        if (dataParts.Length >= 3)
        {
            usingKeyboardInput = false;

            // Parse steering input
            if (float.TryParse(dataParts[0], out float parsedSteering))
                steeringInput = -parsedSteering;
            else
                Debug.LogWarning("Steering data could not be parsed to a float.");

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
                Debug.LogWarning("Horn data could not be parsed to a float.");

            // Parse speed input for serial data
            if (float.TryParse(dataParts[2], out float parsedSpeed))
            {
                float newSpeed = parsedSpeed / 8f;
                verticalInput = Mathf.Clamp(newSpeed, 0f, 15f);
            }
            else
            {
                verticalInput = Input.GetAxis("Vertical"); // Fallback for speed when serial data is incomplete
                Debug.LogWarning("Speed data could not be parsed.");
            }
        }
        else
        {
            usingKeyboardInput = true;
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical"); // Use vertical input from keyboard as fallback
            Debug.LogWarning($"Incomplete data received: '{lastReceivedData}'");
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
        if (usingKeyboardInput)
        {
            // Apply keyboard-based steering
            currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, horizontalInput * maxSteeringAngle, turnSmoothing);
        }
        else
        {
            // Apply serial-based steering
            currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, steeringInput, turnSmoothing);
        }

        currentSteeringAngle = Mathf.Clamp(currentSteeringAngle, -maxSteeringAngle, maxSteeringAngle);
        targetlayingAngle = maxlayingAngle * -currentSteeringAngle / maxSteeringAngle;

        transform.Rotate(Vector3.up * currentSteeringAngle * Time.deltaTime);
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
