using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.model;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public GameObject contextMenuObj;
    public ExpandableContextMenu menu;
    public GameObject RequestControllerObj;
    public RequestController RequestController;

    public RequestPip CurrentRequestPip { get; private set; }
    public Node CurrentNode { get; private set; }

    private Graph graph;

    void Start()
    {
        gameObject.tag = "ui_manager";
        menu = contextMenuObj.GetComponent<ExpandableContextMenu>();
        RequestController = RequestControllerObj.GetComponent<RequestController>();
    }

    public void PopulateEndpointContextMenu(Node node)
    {
        if (isRequestActive())
            return;
        CurrentNode = node;
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
        menu.SetTitle(node.GetLabelText() + " - endpoints");
        menu.SetIsEndpointMode(true);
    }

    public void PopulatePathContextMenu(Node node)
    {
        if (isRequestActive())
            return;
        CurrentNode = node;
        if (graph == null)
            graph = GameObject.FindWithTag("graph").GetComponent<Graph>();
        List<(string, string, IList<(Node, MsLabel)>)> contentList = new List<(string, string, IList<(Node, MsLabel)>)>();

        List<List<MsPathStep>> allPaths = graph.paths;
        Dictionary<string, Node> nodes = graph.nodes;
        List<List<MsPathStep>> relevantPaths = allPaths.Where(p => p.Any() && p.First().node == node.GetLabelText()).ToList();

        foreach (List<MsPathStep> path in relevantPaths)
        {
            List<(Node, MsLabel)> fullSteps = new List<(Node, MsLabel)>();
            foreach (MsPathStep step in path)
            {
                // get the node indicated by the step node label
                Node nextNode = nodes[step.node];
                // get the endpoint (MsLabel) by the step's indicated HTTP type and path
                fullSteps.Add((nextNode, nextNode.endpoints.Where(e => e.path == step.path && e.type == step.type).Single()));
            }
            string buttonText = fullSteps[0].Item2.type + " " + fullSteps[0].Item2.path;
            string contentText = "Initial endpoint function: " + fullSteps[0].Item2.endpointFunction + "\n" +
                                 "Initial Arguments: " + fullSteps[0].Item2.argument + "\n" +
                                 "Total internal hops: " + fullSteps.Count;
            contentList.Add((buttonText, contentText, fullSteps));
        }

        menu.SetupMenu(contentList);
        menu.SetTitle(node.GetLabelText() + " - requests");
        menu.SetIsEndpointMode(false);
    }

    public void PopulateCurrentRequestMenu()
    {
        // TODO: stuff
    }

    public void ToggleMenuMode()
    {
        if (menu.IsEndpointMode)
            PopulatePathContextMenu(CurrentNode);
        else
            PopulateEndpointContextMenu(CurrentNode);
    }

    // returns true if new request can be started; false if old request still running
    public bool SetCurrentRequest(RequestPip request)
    {
        if (isRequestActive())
            return false; // request already exists and is not finished
        if (CurrentRequestPip != null && CurrentRequestPip.IsFinished)
            Destroy(CurrentRequestPip);

        CurrentRequestPip = request;
        RequestController.SetCurrentRequest(CurrentRequestPip);
        menu.CloseMenu();
        return true;
    }

    private bool isRequestActive()
    {
        return CurrentRequestPip != null && !CurrentRequestPip.IsFinished;
    }
}