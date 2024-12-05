using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public Transform player;           // Reference to the player object
    public string enemyTag = "Enemy";  // Tag to identify enemies in the scene
    public float radius = 5f;          // Radius for arrow positioning in XZ plane
    public Vector3 rotationOffset = new Vector3(90, 0, 0); // Rotation offset for arrow

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

            // Calculate the dot product to determine if the enemy is in front of the player
            Vector3 playerForward = player.forward;
            playerForward.y = 0; // Ignore vertical alignment
            playerForward.Normalize();

            float dotProduct = Vector3.Dot(playerForward, normalizedDirection);

            // If the enemy is in front, position the arrow accordingly
            if (dotProduct > 0)
            {
                // Position the arrow within the semicircle in front of the player
                Vector3 arrowPosition = player.position + normalizedDirection * radius;
                arrowPosition.y = transform.position.y; // Maintain original height

                transform.position = arrowPosition;

                // Rotate the arrow to face the enemy
                Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy, Vector3.up);
                transform.rotation = targetRotation * Quaternion.Euler(rotationOffset);
            }
            else
            {
                // Position the arrow at the edge of the semicircle
                Vector3 clampedDirection = Vector3.RotateTowards(playerForward, normalizedDirection, Mathf.PI / 3, 0.0f);
                clampedDirection.Normalize();
                Vector3 arrowPosition = player.position + clampedDirection * radius;
                arrowPosition.y = transform.position.y; // Maintain original height

                transform.position = arrowPosition;

                // Rotate the arrow to align with the clamped direction
                Quaternion targetRotation = Quaternion.LookRotation(clampedDirection, Vector3.up);
                transform.rotation = targetRotation * Quaternion.Euler(rotationOffset);
            }

            // Ensure the arrow is active
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
