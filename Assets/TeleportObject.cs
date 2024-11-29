using UnityEngine;

public class TeleportObject : MonoBehaviour
{
    // Set the target position and rotation in the inspector or dynamically
    public Transform targetPosition;

    // Teleport the object to the target position and match its rotation
    public void Teleport()
    {
        transform.position = targetPosition.position;
        transform.rotation = targetPosition.rotation;
    }

    // Example of triggering teleportation
    private void Update()
    {
        // Press the "T" key to teleport the object
        if (Input.GetKeyDown(KeyCode.T))
        {
            Teleport();
        }
    }
}
