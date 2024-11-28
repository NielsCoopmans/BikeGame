using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPlayer : MonoBehaviour
{
    //public GameObject explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Collision(other));
        }
    }

    IEnumerator Collision(Collider player)
    {
        UnityEngine.Debug.Log("pedestrian collision player");
        BicycleVehicle bicycle = player.GetComponent<BicycleVehicle>();

        //float baseSpeed = bicycle.movementSpeed;
        float baseSpeed = 10f; //problem with the stacking of these penalties
        float currentSpeed = bicycle.movementSpeed;
        float penalty = 5f;

        //Instantiate(explosionPrefab, transform.position, transform.rotation);

        //dissable meshes and collider, so that only once
        foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            renderer.enabled = false;
        }
        GetComponent<Collider>().enabled = false;
        
        float duration = 2f; // Total time for the speed limit effect
        
        //bicycle.movementSpeed = currentSpeed - penalty < 0 ? 0 : currentSpeed - penalty;
        bicycle.movementSpeed = penalty;
        
        yield return new WaitForSeconds(duration);
        
        // Restore the speed to its base value
        bicycle.movementSpeed = baseSpeed;
        
        Destroy(gameObject);
        yield break;
    }
}
