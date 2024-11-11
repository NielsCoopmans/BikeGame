using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountDown : MonoBehaviour
{
    public int countdownTime = 3; 
    public TextMeshProUGUI countdownDisplay; 
    public TextMeshProUGUI countdownMessage; 
    private BicycleVehicle bicycleVehicleScript; 

    private void Start()
    {
        
        bicycleVehicleScript = FindObjectOfType<BicycleVehicle>();
        bicycleVehicleScript.enabled = false;

        StartCoroutine(CountDownToStart());
    }

    private IEnumerator CountDownToStart()
    {
    
        while (countdownTime > 0)
        {
            countdownDisplay.text = countdownTime.ToString();
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        countdownDisplay.text = "GO!";
        yield return new WaitForSeconds(1f);

        countdownDisplay.gameObject.SetActive(false);
        countdownMessage.gameObject.SetActive(false);
        bicycleVehicleScript.enabled = true;
    }
}
