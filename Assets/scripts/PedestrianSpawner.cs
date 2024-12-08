using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            obj.tag = "pedestrian";
            //UnityEngine.Debug.Log("Spawned pedestrian with tag: " + obj.tag);

            yield return new WaitForEndOfFrame();

            //yield return new WaitForSeconds(0.1f); // Small delay to check if tag changes

            //UnityEngine.Debug.Log("Pedestrian tag after delay: " + obj.tag);

            count++;
        }
    }
}
