using UnityEngine;
using TMPro;  // Add this namespace for TextMeshPro

public class Gun : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public int maxBullets = 10;             // Maximum number of bullets that can be fired
    private int currentBullets;             // Number of bullets currently available

    public TextMeshProUGUI bulletCountText; // Reference to the TextMeshProUGUI component to display the bullet count
    public AudioSource shotSound;           // Reference to the AudioSource for the shot sound

    void Start()
    {
        // Initialize the number of bullets
        currentBullets = maxBullets;
        UpdateBulletCountUI();
    }

    void Update()
    {
        // Check if space key is pressed and there are bullets left
        if (Input.GetKeyDown(KeyCode.Space) )
        {
            FireBullet();
        }

        // Optionally, add a reload mechanic or bullet regeneration here
    }

    public void FireBullet()
    {
        if (currentBullets > 0) {
            
            // Instantiate the bullet and set its velocity
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;

            // Play the shot sound
            shotSound.Play();

            // Decrease the bullet count
            currentBullets--;

            // Update the UI with the new bullet count
            UpdateBulletCountUI();
        }
    }

    void UpdateBulletCountUI()
    {
        // Update the TextMeshPro text to show the current number of bullets
        bulletCountText.text = currentBullets.ToString() +" / " + maxBullets.ToString() ;
    }
}
