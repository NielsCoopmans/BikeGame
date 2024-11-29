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
    //public GameObject VFX_EasyExplosion;

    private Vector3 lastPosition;
    private Vector3 velocity;
    private bool isSped = false;
    private float originalMovementSpeed;




    // Start is called before the first frame update
    void Start()
    {
        originalMovementSpeed = movementSpeed;
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

    //collided with bullet
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
        //Destroy(bullet.gameObject); // Destroy the bullet after collision
        yield break;
        //Explode();
        
    }

    /*void Explode()
    {
        void Explode()
        {
            if (VFX_EasyExplosion != null)
            {
                GameObject explosion = Instantiate(VFX_EasyExplosion, transform.position, transform.rotation);
                Destroy(explosion, 2f);
            }
            Destroy(gameObject);
        }
    }*/
}
