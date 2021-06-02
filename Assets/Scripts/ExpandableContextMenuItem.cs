using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpandableContextMenuItem : MonoBehaviour
{
    public GameObject ToggleButton;
    public GameObject ContentText;

    public void ToggleContent()
    {
        ContentText.SetActive(!ContentText.activeSelf);
        Utils.RefreshLayoutGroupsImmediateAndRecursive(gameObject);
    }

    public void SetButtonText(string buttonText)
    {
        TextMeshProUGUI text = ToggleButton.GetComponentInChildren<TextMeshProUGUI>();
        text.SetText(buttonText);
    }

    public void SetContentText(string contentText)
    {
        TextMeshProUGUI text = ContentText.GetComponent<TextMeshProUGUI>();
        text.SetText(contentText);
    }
    
}
