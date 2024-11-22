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

    private Vector3 lastPosition;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position; // Initialize lastPosition
    }

    // Update is called once per frame
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

                // Move forward
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
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

    public void SetDestination(Vector3 newDestination)
    {
        destination = newDestination;
        reachedDestination = false;
    }
}
