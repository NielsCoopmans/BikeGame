using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public GameObject finalBike; // Reference to the FinalBike GameObject
    private Rigidbody bikeRigidbody;
    public TextMeshProUGUI speedText; // UI Text element to display the speed (optional)

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
           
            float speed = bikeRigidbody.velocity.magnitude * 3.6f; // Convert from m/s to km/h

            if (speedText != null)
            {
                speedText.text = $"Speed: {speed:F1} km/h";
            }

            // Alternatively, you could log or use the speed for other purposes
            // Debug.Log("Current Speed: " + speed);
        }
    }
}
