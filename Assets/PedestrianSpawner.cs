using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianSpawner : MonoBehaviour
{

    public GameObject[] pedestrianPrefab;
    public int pedestriansToSpawn;
    public float minSpeed = 1f;
    public float maxSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        int count = 0;
        while(count < pedestriansToSpawn)
        {
            int index = UnityEngine.Random.Range(0, pedestrianPrefab.Length);
            GameObject obj = Instantiate(pedestrianPrefab[index]);
            Transform child = transform.GetChild(UnityEngine.Random.Range(0, transform.childCount - 1));
            obj.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();
            obj.transform.position = child.position;
            //obj.movementSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
            // Access the CharacterNavigationController script and set a random speed
            CharacterNavigationController controller = obj.GetComponent<CharacterNavigationController>();
            if (controller != null)
            {
                controller.movementSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed); // Set random speed
            }

            yield return new WaitForEndOfFrame();

            count++;
        }
    }
}
