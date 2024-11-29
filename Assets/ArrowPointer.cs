using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public Transform player;   // Reference to the player object
    public string enemyTag = "enemy"; // Tag to identify enemies in the scene
    public Vector3 rotationOffset = new Vector3(90, 0, 0); // Adjust as needed

    void Update()
    {
        Transform nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            // Calculate the direction to the nearest enemy
            Vector3 directionToEnemy = nearestEnemy.position - player.position;

            // Ignore vertical differences (optional)
            directionToEnemy.y = 0;

            // Rotate the arrow to face the enemy
            Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy, Vector3.up);

            // Apply the rotation with the offset
            transform.rotation = targetRotation * Quaternion.Euler(rotationOffset);
        }
        else
        {
            // Hide or disable the arrow if no enemy is present
            gameObject.SetActive(false);
        }
    }

    Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(player.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }
}
