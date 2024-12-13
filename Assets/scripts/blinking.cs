using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class blinking : MonoBehaviour
{
    public TMP_Text dontTouch;
    public float blinkInterval = 1f;
    // Start is called before the first frame update
    void Start()
    {
        if (dontTouch == null)
                {
                    dontTouch = GetComponent<TMP_Text>();
                }
                StartCoroutine(BlinkText());
    }
    private IEnumerator BlinkText()
        {
            while (true)
            {
                for (float alpha = 1; alpha >= 0; alpha -= 0.1f)
                        {
                            dontTouch.color = new Color(dontTouch.color.r, dontTouch.color.g, dontTouch.color.b, alpha);
                            yield return new WaitForSeconds(0.1f);
                        }
                        for (float alpha = 0; alpha <= 1; alpha += 0.1f)
                        {
                            dontTouch.color = new Color(dontTouch.color.r, dontTouch.color.g, dontTouch.color.b, alpha);
                            yield return new WaitForSeconds(0.1f);
                        }
            }
        }
    // Update is called once per frame
    void Update()
    {
        
    }
}