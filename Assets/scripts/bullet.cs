using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;          
    public float maxSize = 5f;           
    public float expansionRate = 2f;     
    public GameObject VFX_EasyExplosion;
    public GameObject VFX_EasyExplosion_car;
    public GameObject VFX_EasyExplosion_pedestrian;
    public GameObject VFX_EasyExplosion_money;
    
    public AudioSource explosionSound;
    public float volume = 1.0f;

    private HighScoreManager highScoreManager;

    private void Start()
    {
        GameObject highScoreManagerObject = GameObject.Find("HighScoreManager");
        if (highScoreManagerObject != null)
        {
            highScoreManager = highScoreManagerObject.GetComponent<HighScoreManager>();
            if (highScoreManager != null)
            {
                UnityEngine.Debug.Log("found higscoremanager in bicycleVehicle");
            }
        }
       
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
        else if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        else if (collision.gameObject.CompareTag("enemy"))
        {
            highScoreManager.hitEnemy(collision.contacts[0].point);
            if (VFX_EasyExplosion_money != null)
            {
                Vector3 explosionPosition = collision.contacts[0].point;
                GameObject moneyExplosion = Instantiate(VFX_EasyExplosion_money, explosionPosition, Quaternion.identity);
                Destroy(moneyExplosion, 2f);
            }

            Destroy(gameObject);
            return;
        }
        else if (collision.gameObject.CompareTag("car") || collision.gameObject.CompareTag("explodable")){
            highScoreManager.hitCar(collision.contacts[0].point);
            if (VFX_EasyExplosion_car != null)
            {
                
                Vector3 explosionPosition = collision.gameObject.transform.position;
                explosionPosition.y += 1.0f;
                GameObject explosion = Instantiate(VFX_EasyExplosion_car, explosionPosition, collision.gameObject.transform.rotation);
                AudioSource explosionAudio = explosion.GetComponent<AudioSource>();
                if (explosionAudio != null)
                {
                    explosionAudio.Play();
                }
                Destroy(explosion, 2f);
            }
            Destroy(collision.gameObject);
            Destroy(gameObject);
            return;
        }

        // Instantiate the explosion effect at the bullet's position
        //Instantiate(VFX_EasyExplosion, transform.position, transform.rotation);
        Explode();
        // Destroy the bullet upon collision
        //Destroy(gameObject);
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
