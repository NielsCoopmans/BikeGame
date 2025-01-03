using UnityEngine;
using TMPro;
using System.Collections;
using System.Diagnostics;

public class Gun : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;
    public int maxBullets = 10;
    private int currentBullets;

    public TextMeshProUGUI bulletCountText;
    public AudioSource shotSound;

    private Vector3 originalPosition;
    private Vector3 centerPosition = new(-1000, 550, 0);
    private Vector3 originalScale = Vector3.one;
    private Vector3 enlargedScale = Vector3.one * 3f;
    private float animationDuration = 1f;

    private Coroutine textAnimationCoroutine;

    void Start()
    {
        currentBullets = maxBullets;
        UpdateBulletCountUI();

        if (bulletCountText != null)
        {
            originalPosition = bulletCountText.transform.position;
            originalScale = bulletCountText.transform.localScale;
            centerPosition += originalPosition;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) FireBullet();
    }

    public void FireBullet()
    {
        BicycleVehicle bicycle = FindObjectOfType<BicycleVehicle>();

        if (bicycle == null)
        {
            UnityEngine.Debug.LogError("BicycleVehicle not found in the scene!");
        }
        float movementSpeed = bicycle.movementSpeed;
        UnityEngine.Debug.Log("Bicycle movement speed: " + movementSpeed);
        //change speed of bullet to add the speed of the bike to the basespeed
        if (currentBullets <= 0) return;

        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * (bulletSpeed + movementSpeed);

        shotSound.Play();
        currentBullets--;
        UpdateBulletCountUI();
    }

    void UpdateBulletCountUI()
    {
        if (currentBullets == 0)
        {

            if (textAnimationCoroutine == null)
            {
                bulletCountText.text = "YOU ARE OUT OF BULLETS";
                textAnimationCoroutine = StartCoroutine(AnimateTextToCenter());
            }
        }
        else
        {
            bulletCountText.text = $"{currentBullets} / {maxBullets}";
        }
    }

    public void ReloadBullets()
    {
        if (currentBullets == 0)
        {
            currentBullets = maxBullets;
            UpdateBulletCountUI();
        }
    }

    public void ReloadBulletsAmmoPowerup()
    {
        currentBullets = Mathf.Min(currentBullets + 5, maxBullets);
        UpdateBulletCountUI();
    }

    private IEnumerator AnimateTextToCenter()
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (animationDuration / 2f);

            bulletCountText.rectTransform.position = Vector3.Lerp(originalPosition, centerPosition, t);
            bulletCountText.transform.localScale = Vector3.Lerp(originalScale, enlargedScale, t);

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        elapsedTime = 0f;

        while (elapsedTime < animationDuration / 2f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (animationDuration / 2f);

            bulletCountText.rectTransform.position = Vector3.Lerp(centerPosition, originalPosition, t);
            bulletCountText.transform.localScale = Vector3.Lerp(enlargedScale, originalScale, t);

            yield return null;
        }

        yield return new WaitForSeconds(animationDuration);

        bulletCountText.rectTransform.position = originalPosition;
        bulletCountText.transform.localScale = originalScale;
        textAnimationCoroutine = null;
        bulletCountText.text = "RELOAD!";
    }
}
