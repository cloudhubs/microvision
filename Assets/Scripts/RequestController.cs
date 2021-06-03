using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RequestController : MonoBehaviour
{
    public RequestPip currentRequest;
    public GameObject PlayPauseButton;

    void Start()
    {
        
    }

    public void SetCurrentRequest(RequestPip request)
    {
        gameObject.SetActive(true);
        currentRequest = request;
        currentRequest.RequestFinishedEvent.AddListener(CloseController);
    }

    private void CloseController()
    {
        gameObject.SetActive(false);
    }

    public void PlayPauseRequest()
    {
        currentRequest.TogglePlayPause();
        if (currentRequest.IsPlaying)
            PlayPauseButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Pause");
        else
            PlayPauseButton.GetComponentInChildren<TextMeshProUGUI>().SetText("Play");
    }

    public void NextStep()
    {
        currentRequest.SkipToNext();
    }

    public void PrevStep()
    {
        currentRequest.SkipToPrev();
    }

    public void CancelRequest()
    {
        currentRequest.CancelRequest();
    }
}
