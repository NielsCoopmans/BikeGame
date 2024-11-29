using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highScoreText;
    int score;
    // Start is called before the first frame update
    void Start()
    {
        //random, change it to actual value
        score = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckHighscore()
    {
        if ( score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
    }

    
}
