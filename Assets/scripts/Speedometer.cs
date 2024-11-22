using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public GameObject finalBike; 
    private Rigidbody bikeRigidbody;
    public TextMeshProUGUI speedText;

    private Queue<float> speedReadings = new Queue<float>();
    private float maxWindowDuration = 1f; 
    private float totalSpeed = 0f; 

    // Start is called before the first frame update
    void Start()
    {
        if (finalBike != null)
        {
            bikeRigidbody = finalBike.GetComponent<Rigidbody>();

            if (bikeRigidbody == null)
            {
                Debug.LogError("FinalBike does not have a Rigidbody component.");
            }
        }
        else
        {
            Debug.LogError("FinalBike GameObject is not assigned.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bikeRigidbody != null)
        {
            float currentSpeed = bikeRigidbody.velocity.magnitude * 36f;

            speedReadings.Enqueue(currentSpeed);
            totalSpeed += currentSpeed;

            while (speedReadings.Count > 0 && speedReadings.Count * Time.deltaTime > maxWindowDuration)
            {
                totalSpeed -= speedReadings.Dequeue();
            }
            float smoothedSpeed = totalSpeed / speedReadings.Count;
            if (speedText != null)
            {
                speedText.text = $"Speed: {smoothedSpeed:F1} km/h";
            }
        }
    }
}
