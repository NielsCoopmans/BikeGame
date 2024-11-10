using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float delay = 3f;
    bool hasExploded = false;
    private float countdown;
    public float duration = 4f;
    public Spawner spawner; // Reference to the spawner

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
            countdown = delay; // Reset countdown if you want it to repeat, or remove this line to only explode once
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
        Debug.Log("Power Picked Up");

        BicycleVehicle bicycle = player.GetComponent<BicycleVehicle>();
        bicycle.movementSpeed += 10;

        // Notify the spawner to remove this power-up from the list
        spawner.RemoveObject(gameObject);

        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = false;
        }
        GetComponent<Collider>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(4);

        bicycle.movementSpeed -= 10;

        Destroy(gameObject);
    }
    
    void Explode()
    {
        // show effect
        
    }
}
