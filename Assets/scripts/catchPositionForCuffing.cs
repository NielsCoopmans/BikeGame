using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class catchPositionForCuffing : MonoBehaviour
{

    Vector3 catchPosition;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("FinalBike")) {
            catchPosition = transform.position; // Save the bad guy's position
            //TriggerCutscene();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
