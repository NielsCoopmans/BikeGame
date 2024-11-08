using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;          // Time before the bullet is destroyed
    public float maxSize = 5f;           // Maximum size of the bullet
    public float expansionRate = 2f;     // Rate at which the bullet grows over time

    private void Start()
    {
        // Destroy the bullet after a certain time to avoid clutter in the scene
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Expand the bullet over time until it reaches maxSize
        if (transform.localScale.x < maxSize)
        {
            // Increase the scale uniformly on all axes
            transform.localScale += Vector3.one * expansionRate * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy the bullet upon collision, unless it hits an object tagged "enemy"
        if (!collision.collider.CompareTag("enemy"))
        {
            Destroy(gameObject);
        }
    }
}
