using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject myObject;
    public float spawnHeightOffset = -10f;
    public int maxSpawnedObjects = 5;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    public string[] roadTiles = { "Road Lane_01 (71)"/*, "Road Intersection_01", "Road Lane_01 (95)", "Road Lane_01 (100)", "Road Lane_01 (99)"*/ };

    public void SpawnObject(Vector3 position)
    {
         // If the number of spawned objects does not exceeds the max create it, otherwise don't
        if (spawnedObjects.Count < maxSpawnedObjects)
        {
            GameObject newObject = Instantiate(myObject, position, Quaternion.identity);

            // Add the new sphere to the list of spawned objects
            spawnedObjects.Add(newObject);

            // Optionally, we can tell the powerup to register itself to be removed when it's picked up.
            PowerUp powerupScript = newObject.GetComponent<PowerUp>();
            if (powerupScript != null)
            {
                powerupScript.SetSpawner(this); // Pass reference to spawner for removal later
            }
        }
        else
        {
            //don't spawn
        }
    }

    public void RemoveObject(GameObject oldObject)
    {
        if (spawnedObjects.Contains(oldObject))
        {
            spawnedObjects.Remove(oldObject);
        }
    }

    void Update()
    {
        // Check if the max number of objects have been spawned
        if (spawnedObjects.Count < maxSpawnedObjects)
        {
            // Pick a random road tile from the array
            if (roadTiles.Length > 0)  // Use Length for arrays
            {
                // Pick a random index
                int randomTileIndex = UnityEngine.Random.Range(0, roadTiles.Length);
                string selectedTileName = roadTiles[randomTileIndex];

                // Find the selected road tile GameObject by name
                GameObject selectedTile = GameObject.Find(selectedTileName);

                if (selectedTile != null)
                {
                    MeshRenderer selectedRenderer = selectedTile.GetComponent<MeshRenderer>();

                    if (selectedRenderer != null)
                    {
                        // Generate a random position within the bounds of the selected road tile
                        Vector3 randomSpawnPosition = new Vector3(
                            UnityEngine.Random.Range(selectedRenderer.bounds.min.x, selectedRenderer.bounds.max.x),
                            selectedRenderer.bounds.center.y + spawnHeightOffset,
                            UnityEngine.Random.Range(selectedRenderer.bounds.min.z, selectedRenderer.bounds.max.z)
                        );

                        // Spawn the sphere at the random position
                        SpawnObject(randomSpawnPosition);
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("Selected road tile does not have a MeshRenderer component.");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Road tile with name '{selectedTileName}' not found.");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("No road tiles are available in the array.");
            }
        }
    }
}
