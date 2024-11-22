using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNavigationController : MonoBehaviour
{
    public float movementSpeed = 2f;
    public float rotationSpeed = 120f;
    public float stopDistance = 2.5f;
    public bool reachedDestination = false;
    public Vector3 destination = new Vector3(51.8f, 0.215f, 31.87f);
    public float avoidanceRadius = 1.5f; // Radius for avoidance checks
    public LayerMask pedestrianLayer; // Layer for pedestrians

    private Vector3 lastPosition;
    private Vector3 velocity;
    private bool isSped = false;
    private float originalMovementSpeed;

    void Start()
    {
        lastPosition = transform.position; // Initialize lastPosition
        originalMovementSpeed = movementSpeed;
    }

    void Update()
    {
        if (transform.position != destination)
        {
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.y = 0; // Keep movement in the horizontal plane

            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopDistance)
            {
                reachedDestination = false;

                // Rotate towards the destination
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Move forward with avoidance
                Vector3 avoidanceDirection = AvoidObstacles();
                Vector3 moveDirection = (destinationDirection.normalized + avoidanceDirection).normalized;
                transform.Translate(moveDirection * movementSpeed * Time.deltaTime, Space.World);
            }
            else
            {
                reachedDestination = true;
            }

            // Calculate velocity
            velocity = (transform.position - lastPosition) / Time.deltaTime;
            velocity.y = 0; // Ignore vertical velocity

            // Update lastPosition
            lastPosition = transform.position;
        }
    }

    Vector3 AvoidObstacles()
    {
        Vector3 avoidanceVector = Vector3.zero;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, avoidanceRadius, pedestrianLayer);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.transform != transform)
            {
                Vector3 directionToCollider = transform.position - hitCollider.transform.position;
                avoidanceVector += directionToCollider.normalized / directionToCollider.magnitude;
            }
        }

        return avoidanceVector;
    }

    public void SetDestination(Vector3 newDestination)
    {
        destination = newDestination;
        reachedDestination = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet"))
        {
            StartCoroutine(Collision(other));
        }
    }

    IEnumerator Collision(Collider bullet)
    {
        float speedFactor = 20f;
        if (!isSped)
        {
            isSped = true;
            movementSpeed *= speedFactor;
        }

        float duration = 2f; // Total time for the speed limit effect

        yield return new WaitForSeconds(duration);

        // Restore the speed to its base value
        movementSpeed = originalMovementSpeed;
        isSped = false;
        Destroy(bullet.gameObject); // Destroy the bullet after collision
    }
}