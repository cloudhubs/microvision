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
    public IList<(Node, MsLabel)> Steps { get; private set; }
    private (Node, MsLabel) currentTarget;
    private int stepIdx;

    // counter stuff
    private float pauseTime = 3f;
    private float speed = 1.0f;
    private float timeLeft = 999f;

    // flags
    private bool isMoving;
    private bool timerStarted;
    public bool AtTarget { get; private set; }
    public bool IsPlaying { get; private set; }
    public bool IsFinished { get; set; }

    public UIManager UiManager;

    public UnityEvent RequestFinishedEvent { get; private set; }
    public UnityEvent NodeReachedEvent { get; private set; }

    void Awake()
    {
        if (UiManager == null)
            UiManager = GameObject.FindWithTag("ui_manager").GetComponent<UIManager>();
        if (RequestFinishedEvent == null)
            RequestFinishedEvent = new UnityEvent();
        if (NodeReachedEvent == null)
            NodeReachedEvent = new UnityEvent();
    }

    // Handling movement of the pip

    public void Init(IList<(Node, MsLabel)> steps)
    {
        if (!steps.Any()) // no nodes in list, do nothing
        {
            return;
        }
        // set up initial step
        this.Steps = steps;
        currentTarget = (this.Steps[0].Item1, this.Steps[0].Item2);
        transform.parent = currentTarget.Item1.transform;
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        transform.localPosition = Vector3.zero;
        stepIdx = 0;
        // set initial mats for all the nodes in the path
        foreach (Node n in steps.Select(s => s.Item1))
            n.SetNeighborMat();
        currentTarget.Item1.SetActiveMat();
        // set up flags
        isMoving = false;
        AtTarget = true;
        IsFinished = false;
        IsPlaying = true;
        StartPause();
    }

    // reached a node, start the pause timer
    private void StartPause()
    {
        currentTarget.Item1.SetActiveMat();
        timeLeft = pauseTime;
        timerStarted = true;
        AtTarget = true;
    }

    // start moving to next node; returns true if there was another step to go to, false if not
    public bool NextStep()
    {
        stepIdx++;
        if (stepIdx == Steps.Count)
        {
            StopRequest();
            return false;
        }
        currentTarget.Item1.SetNeighborMat();
        AtTarget = false;
        currentTarget = (Steps[stepIdx].Item1, Steps[stepIdx].Item2);
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
                isMoving = false;
                NodeReachedEvent.Invoke();
                StartPause();
            }
        }
    }

    // playback control functions

    public void TogglePlayPause()
    {
        // playing, so pause the request
        if (IsPlaying)
        {
            IsPlaying = false;
            if (AtTarget) // if we're currently waiting at a node, just pause the timer
                timerStarted = false;
            else // not at a target, just stop moving
                isMoving = false;
        }
        // paused, so play the request
        else
        {
            IsPlaying = true;
            if (AtTarget) // if waiting at a node, just reset the timer
                StartPause();
            else // not at target, keep moving
                isMoving = true;
        }
    }

    public void SkipToNext()
    {
        // if we're at a node, then we need to switch to next target and teleport there
        if (AtTarget)
        {
            stepIdx++;
            if (stepIdx == Steps.Count)
            {
                stepIdx--; // revert back to original target
                return;
            }
            currentTarget.Item1.SetNeighborMat();
            currentTarget = Steps[stepIdx];
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
        currentTarget.Item1.SetActiveMat();
        NodeReachedEvent.Invoke();
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
        currentTarget.Item1.SetNeighborMat();
        currentTarget = Steps[stepIdx];
        transform.parent = currentTarget.Item1.transform;
        transform.localPosition = Vector3.zero;
        if (IsPlaying)
            StartPause();
        currentTarget.Item1.SetActiveMat();
        NodeReachedEvent.Invoke();
    }

    // status functions (for displaying info boxes)

    public (Node, MsLabel) GetNextDestination()
    {
        if (AtTarget) // if we're at a node, we need to calculate next target
        {
            int newIdx = stepIdx + 1;
            if (newIdx == Steps.Count)
                return (null, null);
            return Steps[newIdx];
        }
        else // if we're not at a node, we just return the current target we're moving toward
        {
            return currentTarget;
        }
    }

    public (Node, MsLabel) GetPreviousDestination()
    {
        int newIdx = stepIdx - 1;
        if (newIdx < 0)
            return (null, null);
        return Steps[newIdx];
    }

    public void CancelRequest()
    {
        StopRequest();
    }

    private void OnMouseDown()
    {
        Debug.Log("We clicked a pip");
        UiManager.PopulateCurrentRequestMenu();
    }


    private void StopRequest()
    {
        foreach (Node n in Steps.Select(s => s.Item1))
            n.SetDefaultMat();
        timerStarted = false;
        isMoving = false;
        IsFinished = true;
        RequestFinishedEvent.Invoke();
        Destroy(gameObject);
    }

}
