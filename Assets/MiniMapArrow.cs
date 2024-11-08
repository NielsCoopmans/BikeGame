using UnityEngine;
using UnityEngine.UI;

public class MinimapArrowIndicator : MonoBehaviour
{
    public Camera minimapCamera;        // Reference to the minimap camera
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
            // Calculate direction vector from player to enemy
            Vector3 direction = (enemy.position - player.position).normalized;

            // Set the arrow rotation to point towards the enemy
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            arrowIcon.rotation = Quaternion.Euler(0, 0, -angle + 180f);
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
