using UnityEngine;
using TMPro; // Ensure you have this namespace for TextMeshPro

public class ShowTextAfterDelay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProObject; // Assign the TextMeshPro object in the Inspector
    [SerializeField] private float delay = 15f; // Time in seconds to delay before showing the text

    private void Start()
    {
        // Hide the text initially
        if (textMeshProObject != null)
        {
            textMeshProObject.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("TextMeshPro Object is not assigned!");
        }

        // Start the coroutine to show the text after the delay
        StartCoroutine(ShowTextAfterDelayCoroutine());
    }

    private System.Collections.IEnumerator ShowTextAfterDelayCoroutine()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Show the TextMeshPro object
        if (textMeshProObject != null)
        {
            if(GameStateManager.currentLevel == 1){
                textMeshProObject.text = "Level 1 Complete";
            }
            else if(GameStateManager.currentLevel == 2){
                textMeshProObject.text = "You Win!";
            }
            textMeshProObject.gameObject.SetActive(true);
        }
    }
}
