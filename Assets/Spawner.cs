using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject myObject; // The power-up to spawn
    public float spawnHeightOffset = -10f; // Offset for spawn height
    public int maxSpawnedObjects = 5; // Max number of spawned power-ups at any time

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private GameObject[] roadTiles; // Array of tiles with the spawnPowerup tag

    void Start()
    {
        // Find all GameObjects with the tag "spawnPowerup" once, at the start
        roadTiles = GameObject.FindGameObjectsWithTag("spawnPowerup");

        if (roadTiles.Length == 0)
        {
            UnityEngine.Debug.LogWarning("No tiles found with the tag 'spawnPowerup'. Power-ups won't spawn.");
        }
    }

    public void SpawnObject(Vector3 position)
    {
        // Ensure max spawn count is not exceeded
        if (spawnedObjects.Count < maxSpawnedObjects)
        {
            GameObject newObject = Instantiate(myObject, position, Quaternion.identity);

            // Track spawned objects
            spawnedObjects.Add(newObject);

            // Optional: Notify the PowerUp script about this spawner
            PowerUp powerupScript = newObject.GetComponent<PowerUp>();
            if (powerupScript != null)
            {
                powerupScript.SetSpawner(this);
            }
        }
    }

    public void RemoveObject(GameObject oldObject)
    {
        // Remove object from the list if it exists
        if (spawnedObjects.Contains(oldObject))
        {
            spawnedObjects.Remove(oldObject);
        }
    }

    void Update()
    {
        // Check if we can spawn more objects
        if (spawnedObjects.Count < maxSpawnedObjects && roadTiles.Length > 0)
        {
            // Choose a random tile from the array
            int randomTileIndex = UnityEngine.Random.Range(0, roadTiles.Length);
            GameObject selectedTile = roadTiles[randomTileIndex];

            // Ensure the selected tile has a MeshRenderer to determine bounds
            MeshRenderer selectedRenderer = selectedTile.GetComponent<MeshRenderer>();

            if (selectedRenderer != null)
            {
                // Generate a random position within the bounds of the selected tile
                Vector3 randomSpawnPosition = new Vector3(
                    UnityEngine.Random.Range(selectedRenderer.bounds.min.x, selectedRenderer.bounds.max.x),
                    selectedRenderer.bounds.center.y + spawnHeightOffset,
                    UnityEngine.Random.Range(selectedRenderer.bounds.min.z, selectedRenderer.bounds.max.z)
                );

                // Spawn the power-up at the calculated position
                SpawnObject(randomSpawnPosition);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Selected road tile does not have a MeshRenderer component.");
            }
        }
    }
}
