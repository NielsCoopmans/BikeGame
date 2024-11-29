using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject myObject; //the power-up to spawn
    public float spawnHeightOffset = -10f; // offset for spawn height
    public int maxSpawnedObjects = 5; //max number of spawned power-ups at any time

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private GameObject[] roadTiles; // array of tiles with the spawnPowerup tag

    void Start()
    {
        //find all GameObjects with the tag "spawnPowerup" once, at the start
        roadTiles = GameObject.FindGameObjectsWithTag("spawnPowerup");

        if (roadTiles.Length == 0)
        {
            UnityEngine.Debug.LogWarning("No tiles found with the tag 'spawnPowerup'. Power-ups won't spawn.");
        }
    }

    public void SpawnObject(Vector3 position)
    {
        //ensure max spawn count is not exceeded
        if (spawnedObjects.Count < maxSpawnedObjects)
        {
            GameObject newObject;
            if (myObject.CompareTag("SpeedBoost"))
            {
                Quaternion rotation = Quaternion.Euler(-70, 0, 0);
                newObject = Instantiate(myObject, position, rotation);
            }
            else
            {
                newObject = Instantiate(myObject, position, Quaternion.identity);
            }


            //track spawned objects
            spawnedObjects.Add(newObject);

            //notify the PowerUp script about this spawner
            PowerUp powerupScript = newObject.GetComponent<PowerUp>();
            if (powerupScript != null)
            {
                powerupScript.SetSpawner(this);
            }
        }
    }

    public void RemoveObject(GameObject oldObject)
    {
        //remove object from the list if it exists
        if (spawnedObjects.Contains(oldObject))
        {
            spawnedObjects.Remove(oldObject);
        }
    }

    void Update()
    {
        //check if we can spawn more objects
        if (spawnedObjects.Count < maxSpawnedObjects && roadTiles.Length > 0)
        {
            //choose a random tile from the array
            int randomTileIndex = UnityEngine.Random.Range(0, roadTiles.Length);
            GameObject selectedTile = roadTiles[randomTileIndex];

            //ensure the selected tile has a MeshRenderer to determine bounds
            MeshRenderer selectedRenderer = selectedTile.GetComponent<MeshRenderer>();

            if (selectedRenderer != null)
            {
                //generate a random position within the bounds of the selected tile
                Vector3 randomSpawnPosition = new Vector3(
                    UnityEngine.Random.Range(selectedRenderer.bounds.min.x, selectedRenderer.bounds.max.x),
                    selectedRenderer.bounds.center.y + spawnHeightOffset,
                    UnityEngine.Random.Range(selectedRenderer.bounds.min.z, selectedRenderer.bounds.max.z)
                );

                //spawn the power-up at the calculated position
                SpawnObject(randomSpawnPosition);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Selected road tile does not have a MeshRenderer component.");
            }
        }
    }
}
