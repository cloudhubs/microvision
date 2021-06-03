using Assets.Scripts.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

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
    public bool finished { get; set; }

    public void Init(IList<(Node, MsLabel)> steps)
    {
        if (!steps.Any()) // no nodes in list, do nothing
        {
            return;
        }
        this.steps = steps;
        stepIdx = 0;
        // set up flags
        isMoving = false;
        atTarget = true;
        finished = false;
        currentTarget = (steps[0].Item1, steps[0].Item2);
        transform.parent = currentTarget.Item1.transform;
        transform.localPosition = Vector3.zero;
        StartPause();
    }

    //// pause timer expired, go to next step
    //private void OnPauseTimer(object source, ElapsedEventArgs e)
    //{
    //    Debug.Log("Timer expired");
    //    timer.Stop(); // stop the pause timer while we travel to next step
    //    bool isNext = NextStep(); // travel to next step
    //    Debug.Log("someNode name " + someNode.GetLabelText());
    //    Debug.Log("someNOde position " + someNode.transform.position.ToString());
    //    Debug.Log("Next step done?");
    //    if (isNext)
    //    {
    //        // temp!
    //        timer.Start();
    //    }
    //}

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
    

    private void StopRequest()
    {
        timerStarted = false;
        isMoving = false;
    }

}
