using System.Collections;
using UnityEngine;

public class RampPortal : MonoBehaviour
{
    public Transform targetPosition; // Target position to move to
    public float transitionDuration = 1f; // Duration for smooth transition

    private bool isTransitioning = false; // Prevent multiple transitions
    public GameObject bike; // Reference to the bike

    public void ActivatePortal(Collider other)
    {
        UnityEngine.Debug.Log(" in activate portal function");
        if (isTransitioning) return;

        bike = other.gameObject;
        StartCoroutine(SmoothTransition());
        
    }

    private IEnumerator SmoothTransition()
    {
        UnityEngine.Debug.Log("in transition function...");
        isTransitioning = true;
        Vector3 startPosition = bike.transform.position;
        Vector3 endPosition = targetPosition.position;

        UnityEngine.Debug.Log("Bike position: " + bike.transform.position);
        UnityEngine.Debug.Log("startPosition: " + startPosition);
        UnityEngine.Debug.Log("endPosition " + endPosition);

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            bike.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bike.transform.position = endPosition; // Ensure final position is exact
        UnityEngine.Debug.Log("Transition complete. Bike at " + bike.transform.position);
        isTransitioning = false;
    }
}
