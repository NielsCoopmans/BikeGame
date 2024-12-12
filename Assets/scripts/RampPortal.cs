using System.Collections;
using UnityEngine;
using TMPro;

public class RampPortal : MonoBehaviour
{
    public Transform targetPosition; // Target position to move to
    public float transitionDuration = 1f; // Duration for smooth transition
    public float cooldownTime = 1.5f; // Cooldown time to prevent re-triggering
    public GameObject bike; // Reference to the bike

    private bool isTransitioning = false; // Prevent multiple transitions
    private static bool isInCooldown = false; // Tracks whether the portal is in cooldown

    public TextMeshProUGUI NearInfo;

    public void ActivatePortal()
    {
        if (isTransitioning || isInCooldown)
        {
            UnityEngine.Debug.Log($"Portal {gameObject.name} is in transition or cooldown. Ignoring activation.");
            return;
        }

        isTransitioning = true;
        isInCooldown = true;
        UnityEngine.Debug.Log($"Portal {gameObject.name} cooldown started.");

        UnityEngine.Debug.Log($"Portal {gameObject.name} is activating transition.");
        StartCoroutine(SmoothTransition());
    }

    private IEnumerator SmoothTransition()
    {
        UnityEngine.Debug.Log($"Portal {gameObject.name} starting transition.");
        Vector3 startPosition = bike.transform.position;
        Vector3 endPosition = targetPosition.position;

        UnityEngine.Debug.Log("Bike root object: " + bike.transform.root.name);
        UnityEngine.Debug.Log("Bike position: " + bike.transform.position);
        UnityEngine.Debug.Log("startPosition: " + startPosition);
        UnityEngine.Debug.Log("endPosition " + endPosition);

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            //bike.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / transitionDuration);
            bike.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bike.transform.position = endPosition; // Ensure final position is exact
        UnityEngine.Debug.Log("Transition complete. Bike at " + bike.transform.position);
        NearInfo.text = "Succes!";
        isTransitioning = false;

        StartCoroutine(PortalCooldown());
    }

    private IEnumerator PortalCooldown()
    {
        // Wait for the cooldown period to expire
        yield return new WaitForSeconds(cooldownTime);

        isInCooldown = false;
        UnityEngine.Debug.Log($"Portal {gameObject.name} cooldown expired.");
    }
}
