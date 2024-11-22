using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiSwichTest : MonoBehaviour
{
    public Waypoint currentWaypoint;
    public float speed = 2f;
    public float reachThreshold = 0.1f;

    private Vector3 targetPosition;

    void Start()
    {
        if (currentWaypoint != null)
        {
            targetPosition = currentWaypoint.GetPosition();
        }
    }

    void Update()
    {
        if (currentWaypoint == null) return;

        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the enemy reached the target position
        if (Vector3.Distance(transform.position, targetPosition) <= reachThreshold)
        {
            ChooseNextWaypoint();
        }
    }

    void ChooseNextWaypoint()
    {
        // Determine if there are branches to follow
        if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0 && Random.value < currentWaypoint.branchRatio)
        {
            // Choose a random branch
            currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
        }
        else
        {
            // Follow the next waypoint
            currentWaypoint = currentWaypoint.nextWaypoint;
        }

        // Update the target position for the next waypoint
        if (currentWaypoint != null)
        {
            targetPosition = currentWaypoint.GetPosition();
        }
    }
}
