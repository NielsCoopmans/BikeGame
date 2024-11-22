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

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position; // Initialize last position
        rb = GetComponent<Rigidbody>();    // Get the Rigidbody component if available

        if (currentWaypoint != null)
        {
            targetPosition = currentWaypoint.GetPosition(); // Set the initial destination to the first waypoint
        }
        else
        {
            Debug.LogError("No starting waypoint assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the current waypoint is assigned and if the enemy is moving
        if (currentWaypoint == null) return;

        Vector3 destinationDirection = targetPosition - transform.position;
        destinationDirection.y = 0; // Keep movement in the horizontal plane
        float destinationDistance = destinationDirection.magnitude;

        // If not yet reached the target
        if (destinationDistance >= stopDistance)
        {
            reachedDestination = false;

            // Rotate towards the destination
            Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move forward
            Vector3 moveDirection = destinationDirection.normalized * movementSpeed * Time.deltaTime;

            // Use Rigidbody for smooth movement if available
            if (rb != null)
            {
                rb.MovePosition(transform.position + moveDirection);
            }
            else
            {
                transform.Translate(moveDirection, Space.World); // Fallback to transform if no Rigidbody is available
            }
        }
        else
        {
            reachedDestination = true;  // Reached the current waypoint
            ChooseNextWaypoint();       // Move to the next waypoint
        }

        // Calculate velocity
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        velocity.y = 0; // Ignore vertical velocity

        // Update lastPosition
        lastPosition = transform.position;
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
