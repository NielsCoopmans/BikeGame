using UnityEngine;
using UnityEngine.UI;

public class MinimapArrowIndicator : MonoBehaviour
{
    public Camera minimapCamera;        
    public Transform player;            // Reference to the player position
    public Transform enemy;             // Reference to the enemy position
    public RectTransform arrowIcon;     // UI arrow icon that will point to the enemy
    public RectTransform minimapUI;     // RectTransform of the minimap UI
    public float showArrowDistance = 10f; // Distance threshold to show the arrow

    void Update()
{
    // Calculate the distance between the player and the enemy
    float distanceToEnemy = Vector3.Distance(player.position, enemy.position);

    // Show or hide the arrow based on distance
    arrowIcon.gameObject.SetActive(distanceToEnemy > showArrowDistance);

    if (arrowIcon.gameObject.activeSelf)
    {
        // Calculate the direction vector from player to enemy
        Vector3 directionToEnemy = (enemy.position - player.position).normalized;

        // Transform this direction based on the player's rotation
        Vector3 relativeDirection = player.InverseTransformDirection(directionToEnemy);

        // Calculate the angle to rotate the arrow in 2D space (XY plane of UI)
        float angle = Mathf.Atan2(relativeDirection.z, relativeDirection.x) * Mathf.Rad2Deg;

        // Rotate the arrow to point towards the enemy, with an offset to match the starting direction
        arrowIcon.rotation = Quaternion.Euler(0, 0, angle + 135f); 
    }
}

    private Vector2 GetMinimapEdgePosition(Vector3 direction)
    {
        // Get the minimap center in local coordinates
        Vector2 minimapCenter = minimapUI.rect.center;

        // Determine the maximum width and height to position the arrow at the minimap's edge
        float halfWidth = minimapUI.rect.width / 2;
        float halfHeight = minimapUI.rect.height / 2;

        // Scale the direction vector to fit within the minimap's bounds
        float scaleFactor = Mathf.Min(halfWidth / Mathf.Abs(direction.x), halfHeight / Mathf.Abs(direction.z));

        // Calculate and return the arrow's position on the minimap edge
        return minimapCenter + new Vector2(direction.x * scaleFactor, direction.z * scaleFactor);
    }
}
