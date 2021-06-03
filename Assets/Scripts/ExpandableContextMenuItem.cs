using Assets.Scripts.model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpandableContextMenuItem : MonoBehaviour
{
    public GameObject ToggleButton;
    public GameObject ContentText;
    public GameObject PlayRequestContainer;
    public GameObject PlayRequestButton;
    public GameObject RequestPf;

    public UIManager UiManager;

    public IList<(Node, MsLabel)> request;

    public bool HasRequest { get; private set; }

    void Start()
    {
        if (UiManager == null)
            UiManager = GameObject.FindWithTag("ui_manager").GetComponent<UIManager>();
    }

    public void ToggleContent()
    {
        ContentText.SetActive(!ContentText.activeSelf);
        PlayRequestContainer.SetActive(HasRequest && !PlayRequestContainer.activeSelf);
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

    public void SetRequest(IList<(Node, MsLabel)> requestSteps)
    {
        HasRequest = true;
        request = requestSteps;
    }

    public void PlayRequest()
    {
        GameObject requestObj = Instantiate(RequestPf, transform);
        RequestPip currentRequest = requestObj.GetComponent<RequestPip>();
        bool isValid = UiManager.SetCurrentRequest(currentRequest);
        if (isValid)
        {
            currentRequest.Init(request);
        }
    }
}
