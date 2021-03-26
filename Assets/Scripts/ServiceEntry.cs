using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServiceEntry : MonoBehaviour
{
    string ServiceName;

    public void SetName(string name)
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = name;
    }


}
