using UnityEngine;

public class ShockwaveShooter : MonoBehaviour
{
    public GameObject shockwavePrefab;   // Assign your shockwave prefab here
    public float shootingCooldown = 1f;  // Time between each shot

    private float lastShotTime;

    void Update()
    {
        // Check if the shoot button is pressed and the cooldown has passed
        if (Input.GetKeyDown(KeyCode.Backspace) && Time.time > lastShotTime + shootingCooldown)
        {
            ShootShockwave();
            lastShotTime = Time.time;
        }
    }

    void ShootShockwave()
    {
        // Instantiate the shockwave projectile at the shooter's position and rotation
        Instantiate(shockwavePrefab, transform.position, transform.rotation);
    }
}
