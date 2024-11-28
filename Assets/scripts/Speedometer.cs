using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public GameObject finalBike; // The player's GameObject
    public TextMeshProUGUI speedText;

    private Queue<float> speedReadings = new Queue<float>();
    private float maxWindowDuration = 1f; // Smoothing window duration
    private float totalSpeed = 0f; // Sum of speeds for smoothing
    private Vector3 lastPosition; // To track player's position between frames

    // Start is called before the first frame update
    void Start()
    {
        if (finalBike != null)
        {
            lastPosition = finalBike.transform.position;
        }
        else
        {
            Debug.LogError("FinalBike GameObject is not assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (finalBike != null)
        {
            // Calculate current speed based on the change in position
            Vector3 currentPosition = finalBike.transform.position;
            float distanceTraveled = Vector3.Distance(currentPosition, lastPosition);
            float currentSpeed = (distanceTraveled / Time.deltaTime) * 1.6f; // Convert m/s to km/h

            // Update the smoothing queue
            speedReadings.Enqueue(currentSpeed);
            totalSpeed += currentSpeed;

            while (speedReadings.Count > 0 && speedReadings.Count * Time.deltaTime > maxWindowDuration)
            {
                totalSpeed -= speedReadings.Dequeue();
            }

            // Calculate the smoothed speed
            float smoothedSpeed = totalSpeed / speedReadings.Count;

            // Update the speed text
            if (speedText != null)
            {
                speedText.text = $"Speed: {smoothedSpeed:F1} km/h";
            }

            // Update the last position for the next frame
            lastPosition = currentPosition;
        }
    }
}
