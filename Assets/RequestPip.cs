using Assets.Scripts.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;

public class RequestPip : MonoBehaviour
{
    // step stuff
    private IList<(Node, MsLabel)> steps;
    private (Node, MsLabel) currentTarget;
    private int stepIdx;

    // counter stuff
    private float pauseTime = 3f;
    private float speed = 1.0f;
    private float timeLeft = 999f;

    // flags
    private bool isMoving;
    private bool timerStarted;
    private bool atTarget;
    public bool IsPlaying { get; private set; }
    public bool IsFinished { get; set; }

    public UIManager UiManager;

    public UnityEvent RequestFinishedEvent { get; private set; }

    void Awake()
    {
        if (UiManager == null)
            UiManager = GameObject.FindWithTag("ui_manager").GetComponent<UIManager>();
        if (RequestFinishedEvent == null)
            RequestFinishedEvent = new UnityEvent();
    }

    public void Init(IList<(Node, MsLabel)> steps)
    {
        if (!steps.Any()) // no nodes in list, do nothing
        {
            return;
        }
        this.steps = steps;
        currentTarget = (steps[0].Item1, steps[0].Item2);
        transform.parent = currentTarget.Item1.transform;
        transform.localScale = transform.parent.lossyScale;
        stepIdx = 0;
        // set up flags
        isMoving = false;
        atTarget = true;
        IsFinished = false;
        IsPlaying = true;
        transform.localPosition = Vector3.zero;
        StartPause();
    }

    // reached a node, start the pause timer
    private void StartPause()
    {
        timeLeft = pauseTime;
        timerStarted = true;
        atTarget = true;
    }

    // start moving to next node; returns true if there was another step to go to, false if not
    public bool NextStep()
    {
        stepIdx++;
        if (stepIdx == steps.Count)
        {
            StopRequest();
            return false;
        }
        atTarget = false;
        currentTarget = (steps[stepIdx].Item1, steps[stepIdx].Item2);
        transform.parent = currentTarget.Item1.transform;
        isMoving = true;
        return true;
    }

    void Update()
    {
        if (timerStarted)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
            }
            else
            {
                timeLeft = 0;
                timerStarted = false;
                NextStep();
            }
        }
        if (isMoving)
        {
            float step = speed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, step); // move to zero because that's where parent is???
            if (Vector3.Distance(transform.localPosition, Vector3.zero) < 0.001f) // we arrived
            {
                Debug.Log("reached node, stopping");
                isMoving = false;
                StartPause();
            }

        }
    }

    public void PreviousStep()
    {

    }

    // playback control functions

    public void TogglePlayPause()
    {
        // playing, so pause the request
        if (IsPlaying)
        {
            IsPlaying = false;
            if (atTarget) // if we're currently waiting at a node, just pause the timer
                timerStarted = false;
            else // not at a target, just stop moving
                isMoving = false;
        }
        // paused, so play the request
        else
        {
            IsPlaying = true;
            if (atTarget) // if waiting at a node, just reset the timer
                StartPause();
            else // not at target, keep moving
                isMoving = true;
        }
    }

    public void SkipToNext()
    {
        // if we're at a node, then we need to switch to next target and teleport there
        if (atTarget)
        {
            stepIdx++;
            if (stepIdx == steps.Count)
            {
                stepIdx--; // revert back to original target
                return;
            }
            currentTarget = steps[stepIdx];
            transform.parent = currentTarget.Item1.transform;
            transform.localPosition = Vector3.zero;
        }
        // if we're moving toward a node, don't switch targets; just teleport to the current target
        else
        {
            transform.localPosition = Vector3.zero;
            isMoving = false; // stop moving in case it doesn't get detected
        }
        // no matter what, always start the timer again (if we are playing) since we are now at a new node
        if (IsPlaying)
            StartPause();
    }

    public void SkipToPrev()
    {
        // don't need to check if we're at a target or moving; the previous node is always just the stepIdx below us
        stepIdx--;
        if (stepIdx < 0)
        {
            stepIdx++; // revert to original target
            return;
        }
        currentTarget = steps[stepIdx];
        transform.parent = currentTarget.Item1.transform;
        transform.localPosition = Vector3.zero;
        if (IsPlaying)
            StartPause();
    }

    public void CancelRequest()
    {
        StopRequest();
    }

    private void OnMouseDown()
    {
        UiManager.PopulateCurrentRequestMenu();
    }


    private void StopRequest()
    {
        timerStarted = false;
        isMoving = false;
        IsFinished = true;
        RequestFinishedEvent.Invoke();
        Destroy(gameObject);
    }

}
