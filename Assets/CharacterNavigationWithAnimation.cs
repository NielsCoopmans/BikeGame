using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CharacterNavigationAlwaysWalking : MonoBehaviour
{
    [Header("Navigation Settings")]
    public float movementSpeed = 2f;
    public float rotationSpeed = 120f;
    public float stopDistance = 2.5f;
    public bool reachedDestination = false;
    public Vector3 destination = new Vector3(51.8f, 0.215f, 31.87f);

    [Header("Animator")]
    public Animator animator; // Reference to the character's Animator

    private void Start()
    {
        if (animator == null)
        {
            UnityEngine.Debug.LogError("Animator not assigned. Please assign an Animator component.");
        }
        else
        {
            // Set the character to always be in the walking state
            animator.SetBool("Moving", true);
            animator.SetFloat("InputY", 1f); // Simulate constant forward input
        }
    }

    private void Update()
    {
        NavigateToDestination();
    }

    /// Handles navigation to the destination
    private void NavigateToDestination()
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
        }
    }

    /// Sets a new destination for the character to navigate to
    public void SetDestination(Vector3 newDestination)
    {
        destination = newDestination;
        reachedDestination = false;
    }
}
