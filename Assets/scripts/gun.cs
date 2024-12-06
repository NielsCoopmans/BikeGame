using UnityEngine;
using TMPro;
using System.Collections;

public class Gun : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public int maxBullets = 10;
    private int currentBullets;

    public TextMeshProUGUI bulletCountText;
    public AudioSource shotSound;

    [SerializeField] private Vector3 originalPosition;
    private Vector3 centerPosition = new(-800, 550, 0);
    [SerializeField] private Vector3 originalScale = Vector3.one;
    [SerializeField] private Vector3 enlargedScale = Vector3.one * 5f;
    [SerializeField] private float animationDuration = 1f;

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
        if (currentBullets <= 0) return;

        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;

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

        yield return new WaitForSeconds(0.5f);

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
