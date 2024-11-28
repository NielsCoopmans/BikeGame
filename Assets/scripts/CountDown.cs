using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountDown : MonoBehaviour
{
    public int countdownTime = 3; 
    public int missionTime = 200;
    public TextMeshProUGUI countdownDisplay; 
    public TextMeshProUGUI countdownMessage; 
    public TextMeshProUGUI MissionTimeDisplay; 
    public TextMeshProUGUI MissionTimeCounter; 
    public TextMeshProUGUI GameOver; 
    public EnemyController enemyController;  

    public AudioSource Sound;
    public AudioClip go;
    public AudioClip count;

    public AudioSource backgroundMusic;


    private BicycleVehicle bicycleVehicleScript; 

    private void Start()
    {
        
        bicycleVehicleScript = FindObjectOfType<BicycleVehicle>();
        bicycleVehicleScript.enabled = false;
        enemyController.enabled = false;
        
        MissionTimeDisplay.gameObject.SetActive(false);
        MissionTimeCounter.gameObject.SetActive(false);
       
        StartCoroutine(CountDownToStart());
    
    }

    public IEnumerator CountDownToStart()
    {
    
        while (countdownTime > 0)
        {
            countdownDisplay.text = countdownTime.ToString();
            Sound.PlayOneShot(count);
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }
        
        Sound.PlayOneShot(go);
        countdownDisplay.text = "GO!";
        
        yield return new WaitForSeconds(1f);

        countdownDisplay.gameObject.SetActive(false);
        countdownMessage.gameObject.SetActive(false);
        bicycleVehicleScript.enabled = true;
        enemyController.enabled = true;
    

        MissionTimeDisplay.gameObject.SetActive(true);
        MissionTimeCounter.gameObject.SetActive(true);

        backgroundMusic.Play();

        StartCoroutine(MissionTimeCountdown());
    }

    public IEnumerator MissionTimeCountdown()
    {
        while (missionTime > 0)
        {
            MissionTimeCounter.text = missionTime.ToString();
            yield return new WaitForSeconds(1f);
            missionTime--; 
        }

        if (MissionTimeCounter != null){
            MissionTimeCounter.text = "Time's Up!";
            GameOver.text = "GAME OVER";
            enemyController.TriggerCutscene();  
            }

    }
}
