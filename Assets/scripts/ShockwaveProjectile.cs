using UnityEngine;

public class ShockwaveProjectile : MonoBehaviour
{
    public float speed = 10f;             // Speed of the projectile
    public float maxSize = 5f;            // Max size of the shockwave expansion
    public float expansionRate = 2f;      // Speed at which the shockwave expands
    public float lifespan = 2f;           // Duration before the projectile is destroyed

    private Vector3 initialScale;
    private bool hasHitSomething = false;

    void Start()
    {
        // Save the initial scale of the shockwave
        initialScale = transform.localScale;

        // Destroy the projectile after the specified lifespan
        Destroy(gameObject, lifespan);
    }

    void Update()
    {
        if (hasHitSomething) return;

        // Move forward
        transform.position += transform.forward * speed * Time.deltaTime;

        // Expand the shockwave effect
        if (transform.localScale.x < maxSize)
        {
            transform.localScale += Vector3.one * expansionRate * Time.deltaTime;
        }
        else
        {
            // Destroy the projectile if it has reached max size
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the shockwave collides with something
        if (other.CompareTag("Obstacle") || other.CompareTag("Enemy"))
        {
            hasHitSomething = true;
            Destroy(gameObject); // Destroy the projectile on collision
        }
    }
}
