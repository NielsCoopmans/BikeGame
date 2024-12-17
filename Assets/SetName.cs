using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetName : MonoBehaviour
{
    public TextMeshProUGUI inputName;

    public void setName()
    {
        PlayerPrefs.SetString("playerName", inputName.text);
    }
}
