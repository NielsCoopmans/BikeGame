using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Permissions;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float delay = 3f;
    bool hasExploded = false;
    private float countdown;
    public float duration = 4f;
    public Spawner spawner; // Reference to the spawner
    public AudioSource audioSource;

    void Start()
    {
        countdown = delay;
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded) // Changed to <= to trigger when countdown reaches zero
        {
            Explode();
            hasExploded = true;
            countdown = delay; // Reset countdown
        }
    }

    // Method to set the spawner reference from the Spawner class
    public void SetSpawner(Spawner spawnerReference)
    {
        spawner = spawnerReference;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine( PickUp(other) );
        }
    }

    IEnumerator PickUp(Collider player)
    {
        if (gameObject.CompareTag("SpeedBoost"))
        {
            audioSource.Play();
            UnityEngine.Debug.Log("Speedboost Picked Up");

            BicycleVehicle bicycle = player.GetComponent<BicycleVehicle>();

            float baseSpeed = bicycle.movementSpeed;

            // Notify the spawner to remove this power-up from the list
            spawner.RemoveObject(gameObject);

            GetComponent<ParticleSystem>().gameObject.SetActive(false);

            foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = false;
            }
            GetComponent<Collider>().enabled = false;

            float duration = 2f; // Total time for the speed boost effect
            float peakBoost = 15f; // Maximum speed boost
            float halfDuration = duration / 2f; // Time to reach peak boost

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                // Calculate the speed adjustment using a parabola
                float timeFactor = elapsedTime / halfDuration; // Normalize to 0-2
                if (timeFactor > 1f) timeFactor = 2f - timeFactor; // Mirror for second half

                float speedBoost = peakBoost * timeFactor; // Apply parabola

                bicycle.movementSpeed = baseSpeed + speedBoost; // Update speed

                yield return null; // Wait for the next frame
            }

            // Restore the speed to its base value
            bicycle.movementSpeed = 10;

            Destroy(gameObject);
        }
        else
        {
            UnityEngine.Debug.Log("Ammo Picked Up");

            // Notify the spawner to remove this power-up from the list
            spawner.RemoveObject(gameObject);

            foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = false;
            }
            GetComponent<Collider>().enabled = false;

            Gun gun = player.GetComponentInChildren<Gun> ();
            gun.ReloadBulletsAmmoPowerup();

            Destroy(gameObject);
        }
    }


    void Explode()
    {
        // show effect
        
    }
}
