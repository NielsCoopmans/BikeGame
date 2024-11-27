using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;          // Time before the bullet is destroyed
    public float maxSize = 5f;           // Maximum size of the bullet
    public float expansionRate = 2f;     // Rate at which the bullet grows over time
    public GameObject VFX_EasyExplosion;  

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
       
        if (collision.gameObject.CompareTag("Ball"))
        {
            return;
        }

        // Instantiate the explosion effect at the bullet's position
        //Instantiate(VFX_EasyExplosion, transform.position, transform.rotation);
        Explode();
        // Destroy the bullet upon collision
        Destroy(gameObject);
    }

    void Explode()
    {
        if (VFX_EasyExplosion != null)
        {
            GameObject explosion = Instantiate(VFX_EasyExplosion, transform.position, transform.rotation);
            Destroy(explosion, 2f); 
        }
    }
}
