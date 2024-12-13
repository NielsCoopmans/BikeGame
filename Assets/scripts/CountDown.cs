using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountDown : MonoBehaviour
{
    public int countdownTime = 3; 
    public int missionTime;
    public TextMeshProUGUI countdownDisplay; 
    public TextMeshProUGUI countdownMessage; 
    public TextMeshProUGUI MissionTimeDisplay; 
    public TextMeshProUGUI TimeLeft;
    public TextMeshProUGUI NearInfo;

    public TextMeshProUGUI MissionTimeCounter;
    public EnemyController enemyController;

    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public AudioSource warningSound;  

    private bool warningTriggered = false;

    public AudioSource Sound;
    public AudioClip go;
    public AudioClip count;
    

    private float orginalFontSize;

    private BicycleVehicle bicycleVehicleScript;

    private void Start()
    {
        TimeLeft.enabled = false;
        orginalFontSize = MissionTimeCounter.fontSize;
        bicycleVehicleScript = FindObjectOfType<BicycleVehicle>();
        if (bicycleVehicleScript != null)
        {
            bicycleVehicleScript.isCountdownComplete = false; // Disable movement initially
        }
        bicycleVehicleScript.enabled = true;
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

        countdownDisplay.gameObject.SetActive(false);
        Sound.PlayOneShot(go);
        countdownMessage.text = "GO!";

        yield return new WaitForSeconds(1f);

        countdownMessage.gameObject.SetActive(false);
        bicycleVehicleScript.enabled = true;
        enemyController.enabled = true;
        NearInfo.text = "Leave the garage!";


        MissionTimeCounter.gameObject.SetActive(true);

        if (bicycleVehicleScript != null)
        {
            bicycleVehicleScript.isCountdownComplete = true; 
        }
    }

    public void startMissionTimeCountdown(){
        StartCoroutine(MissionTimeCountdown());
    }

    public IEnumerator MissionTimeCountdown()
    {
        MissionTimeDisplay.gameObject.SetActive(true);
        while (missionTime > 0)
        {
            MissionTimeCounter.text = missionTime.ToString();
            yield return new WaitForSeconds(1f);
            missionTime--;
            if (missionTime <= 10 && !warningTriggered)
                timeAlmostUp();
        }
        if (MissionTimeCounter != null){
            MissionTimeCounter.text = "Time's Up!";
            enemyController.TriggerGameOverCutscene();  
        }
    }

    void timeAlmostUp()
    {
        warningTriggered = true;
        MissionTimeCounter.color = warningColor;
        StartCoroutine(BlinkWarningSound());
    
        StartCoroutine(ScaleWarningText());
    }

    private IEnumerator ScaleWarningText()
    {
        while (true)
        {
            MissionTimeCounter.fontSize = orginalFontSize * 1.5f;
            yield return new WaitForSeconds(0.5f);
            MissionTimeCounter.fontSize = orginalFontSize;
            yield return new WaitForSeconds(0.5f);
        }
    }


    private IEnumerator BlinkWarningSound()
    {
        while (true)
        {
            if (warningSound != null)
                warningSound.Play();

            yield return new WaitForSeconds(1f);
        }
    }

    public int getMissionTime()
    {
        return missionTime;
    }
}
