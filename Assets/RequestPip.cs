using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

public class RequestPip : MonoBehaviour
{
    private IList<Node> steps;
    private int stepIdx;
    private int pauseTime = 3000;
    private Timer timer;
    private bool isMoving;
    private float speed = 0.5f;

    ~RequestPip()
    {
        timer.Stop();
        timer.Dispose();
    }

    public void Init(IList<Node> steps)
    {
        if (!steps.Any()) // no nodes in list, do nothing
        {
            return;
        }
        foreach(Node n in steps)
        {
            Debug.Log("Node name: " + n.GetLabelText());
            Debug.Log("Node position: " + n.transform.position.ToString());
        }
        this.steps = steps;
        stepIdx = 0;
        isMoving = false;
        Node first = steps[0];
        Debug.Log(first.GetLabelText());
        transform.parent = first.transform;
        transform.localPosition = Vector3.zero;
        //timer = new Timer(pauseTime);
        //timer.Elapsed += OnPauseTimer;
        //timer.AutoReset = false;
        //timer.Enabled = true;
        // TEMP!!!
        NextStep();
        Debug.Log("Node global position: " + transform.parent.position.ToString());
        Debug.Log("Pip global position: " + transform.position.ToString());
        Debug.Log("Pip local position: " + transform.localPosition.ToString());
    }

    // pause timer expired, go to next step
    private void OnPauseTimer(object source, ElapsedEventArgs e)
    {
        Debug.Log("Timer expired");
        timer.Stop(); // stop the pause timer while we travel to next step
        bool isNext = NextStep(); // travel to next step
        if (isNext)
        {
            //timer.Start();
        }
    }

    // reached a node, start the pause timer
    private void StartPause()
    {
        Debug.Log("Timer started");
        timer.Start();
    }

    // start moving to next node; returns true if there was another step to go to, false if not
    public bool NextStep()
    {
        Debug.Log("moving to next node");
        stepIdx++;
        Debug.Log("stepIdx: " + stepIdx);
        Debug.Log("steps.Count: " + steps.Count);
        Node next = steps.ElementAt(stepIdx);
        Debug.Log("Next: " + next.GetLabelText());
        Debug.Log("Next position: " + next.transform.position.ToString());
        transform.parent = next.transform;
        //if (stepIdx == steps.Count)
        //{
        //    StopRequest();
        //    return false;
        //}


        Debug.Log("Next node global position: " + transform.parent.position.ToString());
        Debug.Log("Pip global position: " + transform.position.ToString());
        Debug.Log("Pip local position: " + transform.localPosition.ToString());
        isMoving = true;
        return true;
    }

    void Update()
    {
        if (isMoving)
        {
            float step = speed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, step); // move to zero because that's where parent is???
            if (Vector3.Distance(transform.localPosition, Vector3.zero) < 0.001f) // we arrived
            {
                Debug.Log("reached node, stopping");
                isMoving = false;
                //StartPause();
                // TEMP!!!!
                NextStep();
            }

        }
    }

    public void PreviousStep()
    {

    }
    

    private void StopRequest()
    {
        timer.Stop();
        timer.Dispose();
    }

}
