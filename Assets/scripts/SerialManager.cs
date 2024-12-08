using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialManager
{
    private SerialPort serialPort;
    private Thread serialThread;
    private bool isRunning;
    private readonly object lockObject = new();
    private string lastReceivedData = "";

    public SerialManager(string portName, int baudRate, int readTimeout = 1000)
    {
        serialPort = new SerialPort(portName, baudRate)
        {
            ReadTimeout = readTimeout
        };
        TryOpenSerialPort();
    }

    void TryOpenSerialPort()
    {
        if (serialPort == null)
        {
            UnityEngine.Debug.LogError("SerialPort is null. Cannot open port.");
            return;
        }

        if (!serialPort.IsOpen)
        {
            try
            {
                serialPort.Open();
                UnityEngine.Debug.Log("Serial port opened successfully.");
                isRunning = true;
                if (serialThread == null || !serialThread.IsAlive)
                {
                    serialThread = new Thread(ReadSerialData);
                    serialThread.Start();
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to open serial port: {ex.Message}");
            }
        }
        else
        {
            UnityEngine.Debug.Log("Serial port is already open.");
        }
    }

    private void ReadSerialData()
    {
        while (isRunning && serialPort.IsOpen)
        {
            try
            {
                lastReceivedData = serialPort.ReadLine();
            }
            catch (System.TimeoutException) { }
            catch (System.Exception ex)
            {
                Debug.LogError($"Serial read error: {ex.Message}");
            }
        }
    }

    public string[] GetLatestData()
    {

        string[] dataParts = lastReceivedData.Split(',');
        lock (lockObject)
        {
            return dataParts;
        }
    }

    public void ClosePort()
    {
        isRunning = false;
        if (serialThread != null && serialThread.IsAlive)
        {
            serialThread.Join();
        }

        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed.");
        }
    }

    ~SerialManager()
    {
        ClosePort();
    }
}
