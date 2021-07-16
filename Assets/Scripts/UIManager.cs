using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.model;
using System.Linq;
using System;

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
        {
            // try to populate this node's endpoints IF it is relevant to the current request
            PopulateRequestNodeEndpointsContextMenu(node);
            return;
        }
        if (CurrentNode != null)
            CurrentNode.SetDefaultMat();
        CurrentNode = node;
        CurrentNode.SetActiveMat();
        SetNeighborMats(true);
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

    // show the current request step
    public void PopulateRequestStepContextMenu()
    {
        List<(string, string)> contentList = new List<(string, string)>();
        (Node, MsLabel) dest = CurrentRequestPip.GetNextDestination();
        (Node, MsLabel) src = CurrentRequestPip.GetPreviousDestination();
        if (dest.Item1 != null)
        {
            string destinationButtonText = "Next destination: " + dest.Item1.GetLabelText();
            string destinationContentText = "HTTP call: " + dest.Item2.type + " " + dest.Item2.path + "\n" +
                                            "Endpoint function: " + dest.Item2.endpointFunction + "\n" +
                                            "Arguments: " + dest.Item2.argument + "\n" +
                                            "Expected return: " + dest.Item2.msReturn;
            contentList.Add((destinationButtonText, destinationContentText));
        }
        if (src.Item1 != null)
        {
            string sourceButtonText = "Previous endpoint: " + src.Item1.GetLabelText();
            string sourceContentText = "HTTP call: " + src.Item2.type + " " + src.Item2.path + "\n" +
                                      "Endpoint function: " + src.Item2.endpointFunction + "\n" +
                                      "Arguments: " + src.Item2.argument + "\n" +
                                      "Expected return: " + src.Item2.msReturn;
            contentList.Add((sourceButtonText, sourceContentText));
        }
        menu.SetupMenu(contentList);
        menu.SetTitle("Current request");
    }

    // show the endpoints of a node that are used in the current request
    public void PopulateRequestNodeEndpointsContextMenu(Node node)
    {
        IEnumerable<MsLabel> relevantEndpoints = CurrentRequestPip.Steps.Where(s => s.Item1.GetLabelText() == node.GetLabelText()).Select(s => s.Item2);
        if (!relevantEndpoints.Any())
            return;
        List<(string, string)> contentList = new List<(string, string)>();
        foreach (MsLabel endpoint in node.endpoints)
        {
            if (relevantEndpoints.Where(re => re.type == endpoint.type && re.path == endpoint.path).Any())
            {
                string buttonText = endpoint.type + " " + endpoint.path;
                string contentText = "Endpoint function: " + endpoint.endpointFunction + "\n" +
                                     "Arguments: " + endpoint.argument + "\n" +
                                     "Return type: " + endpoint.msReturn;
                contentList.Add((buttonText, contentText));
            }
        }
        menu.SetupMenu(contentList);
        menu.SetTitle("Endpoints used in request - " + node.GetLabelText());
    }

    // Set neighbor materials. If isActive, set as neighbor mats; else, revert to default mats
    private void SetNeighborMats(bool isActive)
    {
        if (graph == null)
            graph = GameObject.FindWithTag("graph").GetComponent<Graph>();
        if (CurrentNode == null)
            return;
        
        Dictionary<(string, string), GameObject> edges = graph.edges;
        Dictionary<string, Node> nodes = graph.nodes;
        List<Node> neighborNodes = edges.Select(e => e.Key).Where(key => key.Item1 == CurrentNode.GetLabelText()).Select(key => nodes[key.Item2]).ToList();
        List<Node> otherNodes = nodes.Values.Except(neighborNodes).ToList();
        // color the neighbors
        foreach (var neighbor in neighborNodes)
        {
            if (isActive)
                neighbor.SetNeighborMat();
            else
                neighbor.SetDefaultMat();
        }
        // color non-neighbors
        foreach (var other in otherNodes)
            other.SetDefaultMat();
        // color current node
        if (isActive)
            CurrentNode.SetActiveMat();
        else
            CurrentNode.SetDefaultMat();
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
        menu.CloseMenu();
        SetNeighborMats(false);
        CurrentRequestPip = request;
        CurrentRequestPip.StatusChangedEvent.AddListener(requestStatusChangeListener);
        RequestController.SetCurrentRequest(CurrentRequestPip);
        return true;
    }

    private bool isRequestActive()
    {
        return CurrentRequestPip != null && !CurrentRequestPip.IsFinished;
    }

    public void CloseContextMenu()
    {
        menu.CloseMenu();
        if (isRequestActive())
            return; // do nothing, don't mess with mats while request is running
        SetNeighborMats(false); // if no request running and menu is closed, reset all colors
    }

    private void requestStatusChangeListener()
    {
        if (menu.gameObject.activeSelf)
        {
            PopulateRequestStepContextMenu();
        }
    }
}