using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

public class RequestPip : MonoBehaviour
{
    private IList<GameObject> steps;
    private Node someNode;
    private int stepIdx;
    private float pauseTime = 3f;
    private bool isMoving;
    private bool timerStarted = false;
    private float speed = 1.0f;
    private float timeLeft = 999f;

    public void Init(IList<GameObject> steps)
    {
        if (!steps.Any()) // no nodes in list, do nothing
        {
            return;
        }
        this.steps = steps;
        stepIdx = 0;
        isMoving = false;
        Node first = steps[0].GetComponent<Node>();
        Debug.Log(first.GetLabelText());
        transform.parent = first.transform;
        transform.localPosition = Vector3.zero;
        //timer = new Timer(pauseTime);
        //timer.Elapsed += OnPauseTimer;
        //timer.AutoReset = false;
        //timer.Enabled = true;
        //// TEMP!!!
        //NextStep();
        StartPause();
        Debug.Log("Node global position: " + transform.parent.position.ToString());
        Debug.Log("Pip global position: " + transform.position.ToString());
        Debug.Log("Pip local position: " + transform.localPosition.ToString());
        //someNode = steps[1].GetComponent<Node>();
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
        someNode = steps[stepIdx].GetComponent<Node>();
        Node next = steps[stepIdx].GetComponent<Node>();
        transform.parent = next.transform;
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
