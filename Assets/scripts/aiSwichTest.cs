using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNavigationController : MonoBehaviour
{
    public float movementSpeed = 2f;    // Movement speed
    public float rotationSpeed = 120f;   // Rotation speed
    public float stopDistance = 2.5f;   // Distance threshold to stop moving
    public bool reachedDestination = false; // Whether the destination is reached
    public Waypoint currentWaypoint;    // Start waypoint
    private Vector3 targetPosition;

    private Vector3 lastPosition;
    private Vector3 velocity;

    private bool isSlowed = false;
    private float originalSpeed;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position; // Initialize last position
        rb = GetComponent<Rigidbody>();    // Get the Rigidbody component if available

        originalSpeed = movementSpeed; // Initialize the original speed
        Debug.Log($"Original speed set to: {originalSpeed}");

    }

    public bool isMoving = false; // Controls whether the enemy moves

    void Update()
    {
        if (!isMoving) return; // Skip the rest of Update if not moving

        if (currentWaypoint == null) return; // No waypoint assigned, stop processing

        Vector3 destinationDirection = targetPosition - transform.position;
        destinationDirection.y = 0; // Keep movement in the horizontal plane
        float destinationDistance = destinationDirection.magnitude;

        if (destinationDistance >= stopDistance)
        {
            reachedDestination = false;
            Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            Vector3 moveDirection = destinationDirection.normalized * movementSpeed * Time.deltaTime;

            if (rb != null)
                rb.MovePosition(transform.position + moveDirection);
            else
                transform.Translate(moveDirection, Space.World);
        }
        else
        {
            reachedDestination = true;
            ChooseNextWaypoint();
        }
    }

    public void StartMoving()
    {
        Debug.Log("Enemy started moving.");
        isMoving = true; // Enable movement
        if (currentWaypoint != null)
            targetPosition = currentWaypoint.GetPosition();
    }


    public void ApplySlow(float duration, float slowFactor)
    {
        if (isSlowed)
        {
            Debug.Log("Enemy is already slowed. Skipping.");
            return; // Prevent multiple slows
        }

        isSlowed = true;
        originalSpeed = movementSpeed; // Save the current speed as the original speed
        movementSpeed *= slowFactor;  // Apply the slow factor

        Debug.Log($"Slow applied: Speed changed from {originalSpeed} to {movementSpeed}. Duration: {duration}");

        // Schedule a reset after the duration
        StartCoroutine(ResetSpeedAfterDelay(duration));
    }

    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset the movement speed to its original value
        Debug.Log($"Resetting speed from {movementSpeed} to {originalSpeed} after {delay} seconds.");
        movementSpeed = originalSpeed;
        isSlowed = false;
    }

    // Method to update the destination to the next waypoint
    void ChooseNextWaypoint()
    {
        if (currentWaypoint != null)
        {
            // Check if there are branches
            if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
            {
                // Decide whether to take a branch based on branchRatio
                if (UnityEngine.Random.value < currentWaypoint.branchRatio)
                {
                    // Choose a random branch from the list
                    currentWaypoint = currentWaypoint.branches[UnityEngine.Random.Range(0, currentWaypoint.branches.Count)];
                    targetPosition = currentWaypoint.GetPosition();
                    Debug.Log("Branch chosen, moving to: " + currentWaypoint.name);
                    return;
                }
            }

            // If no branch is chosen, move to the next waypoint
            if (currentWaypoint.nextWaypoint != null)
            {
                currentWaypoint = currentWaypoint.nextWaypoint;
                targetPosition = currentWaypoint.GetPosition();
                Debug.Log("Moving to next waypoint: " + currentWaypoint.name);
            }
            else
            {
                Debug.Log("No more waypoints or end of path reached!");
                reachedDestination = true; // Stop moving if no next waypoint
            }
        }
    }
}
