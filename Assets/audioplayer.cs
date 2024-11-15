using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioplayer : MonoBehaviour
{
    public AudioSource audiosource;
    // Start is called before the first frame update
    void Start()
    {
        audiosource.Play();
    }


}
