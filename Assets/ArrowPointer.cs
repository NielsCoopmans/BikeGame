using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public Transform player;   // Reference to the player object
    public string enemyTag = "Enemy"; // Tag to identify enemies in the scene
    public float radius = 5f;  // Radius of the semi-circle
    public Vector3 rotationOffset = new Vector3(90, 0, 0); // Adjust as needed

    void Update()
    {
        Transform nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            // Calculate the direction to the nearest enemy
            Vector3 directionToEnemy = nearestEnemy.position - player.position;

            // Ignore vertical differences
            directionToEnemy.y = 0;

            // Normalize the direction vector
            Vector3 normalizedDirection = directionToEnemy.normalized;

            // Calculate the semi-circular position around the player
            Vector3 arrowPosition = player.position + normalizedDirection * radius;

            // Clamp the arrow's position to remain on a semi-circle in the XZ plane
            arrowPosition.y = transform.position.y; // Keep the arrow at its original height

            // Update the arrow's position
            transform.position = arrowPosition;

            // Rotate the arrow to face the enemy
            Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy, Vector3.up);

            // Apply the rotation with the offset
            transform.rotation = targetRotation * Quaternion.Euler(rotationOffset);

            // Ensure the arrow is visible
            gameObject.SetActive(true);
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
