using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.model;

public class UIManager : MonoBehaviour
{
    public GameObject contextMenuObj;

    public RequestPip CurrentRequestPip { get; private set; }

    private Graph graph;

    void Start()
    {
        gameObject.tag = "ui_manager";
        if (graph == null)
            graph = GameObject.FindWithTag("graph").GetComponent<Graph>();
    }

    public void PopulateEndpointContextMenu(Node node)
    {
        ExpandableContextMenu menu = contextMenuObj.GetComponent<ExpandableContextMenu>();
        List<(string, string)> contentList = new List<(string, string)>();
        foreach (MsLabel endpoint in node.endpoints)
        {
            string buttonText = endpoint.type + " " + endpoint.path;
            string contentText = "Endpoint function: " + endpoint.endpointFunction + "\n" + 
                                 "Arguments: " + endpoint.argument + "\n" +
                                 "Return type: " + endpoint.msReturn;
            contentList.Add((buttonText, contentText));
        }
        menu.SetupMenu(contentList);
        menu.SetTitle(node.GetLabelText());
    }

    public void PopulatePathContextMenu(Node node)
    {
        ExpandableContextMenu menu = contextMenuObj.GetComponent<ExpandableContextMenu>();
        List<(string, string, IList<(Node, MsLabel)>)> contentList = new List<(string, string, IList<(Node, MsLabel)>)>();
        
    }

    // returns true if new request can be started; false if old request still running
    public bool SetCurrentRequest(RequestPip request)
    {
        if (CurrentRequestPip != null && !CurrentRequestPip.isFinished)
        {
            return false; // request already exists and is not finished
        }
        if (CurrentRequestPip.isFinished)
        {
            Destroy(CurrentRequestPip);
        }
        CurrentRequestPip = request;
        return true;
    }
}