using Assets.Scripts.model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Graph : MonoBehaviour
{
    public GameObject nodepf;
    public GameObject edgepf;
    public GameObject requestpf;
    public float width;
    public float length;
    public float height;
    
    UnconnectedGraph unconnectedGraph;
    ServicesMenu menu;

    // keys nodes by their label (microservice name, basically)
    public Dictionary<string, Node> nodes { get; private set; }
    // keys edges by their (from, to) labels
    public Dictionary<(string, string), GameObject> edges { get; private set; }
    // list of all paths (contains name, http method, and URL of each path, doesn't actually contain path nodes)
    public List<List<MsPathStep>> paths { get; private set; }
    ISet<Node> connectedNodes = new HashSet<Node>();
    bool initDone = false;
    

    void Start()
    {
        gameObject.tag = "graph";

        ProphetData data = CallProphet();
        nodes = new Dictionary<string,Node>();
        edges = new Dictionary<(string, string), GameObject>();
        GameObject ugObject = new GameObject();
        unconnectedGraph = ugObject.AddComponent<UnconnectedGraph>() as UnconnectedGraph;
        unconnectedGraph.transform.parent = transform;
        
        // process nodes
        for (int i = 0; i < data.communication.nodes.Count; i++)
        {
            string label = data.communication.nodes[i].label;
            //Vector3 init = new Vector3(Random.Range(-width / 2, width / 2), Random.Range(-length / 2, length / 2), Random.Range(-height / 2, height / 2));
            //init.Scale(transform.lossyScale);
            //Vector3 init = new Vector3(Random.Range(transform.position.x - width / 2, transform.position.x + width / 2),
            //                           Random.Range(transform.position.y - length / 2, transform.position.y + length / 2),
            //                           Random.Range(transform.position.z - height / 2, transform.position.z + height / 2));
            Vector3 localInit = new Vector3(Random.Range(-width / 2, width / 2),
                                       Random.Range(-length / 2, length / 2),
                                       Random.Range(-height / 2, height / 2));
            //GameObject go = Instantiate(nodepf, new Vector3(Random.Range(-width / 2, width / 2), Random.Range(-length / 2, length / 2), Random.Range(-height / 2, height / 2)), Quaternion.identity);
            //GameObject go = Instantiate(nodepf, init, Quaternion.identity);
            GameObject go = Instantiate(nodepf, transform);
            go.transform.localPosition = localInit;
            Node n = go.GetComponent<Node>();
            n.endpoints = data.communication.nodes[i].endpoints;
            if (i == 0)
            {
                n.anchored = true;
            }
            //n.transform.parent = transform;
            //n.transform.localPosition = localInit;
            n.SetEdgePrefab(edgepf);
            n.SetLabelText(label);
            nodes.Add(label, n);
        }

        // process edges
        for (int i = 0; i < data.communication.edges.Count; i++)
        {
            MsEdge edge = data.communication.edges[i];
            Node src = nodes[edge.from.label];
            Node dst = nodes[edge.to.label];
            GameObject edgeObj = src.AddEdge(dst);
            connectedNodes.Add(src);
            connectedNodes.Add(dst);
            if (!edges.ContainsKey((edge.from.label, edge.to.label)))
            {
                edges.Add((edge.from.label, edge.to.label), edgeObj);
            }
        }

        // process paths
        paths = data.communication.paths.Select(p => p.pathSteps).ToList();
        
        // handle unconnected nodes
        foreach(Node n in nodes.Values)
        {
            if (!connectedNodes.Contains(n))
            {
                BoxCollider bc = n.GetComponent<BoxCollider>() as BoxCollider;
                bc.enabled = false;
                unconnectedGraph.AddNode(n);
            }
        }

        // send the nodes over to the menu to display in a list
        menu = GameObject.Find("ServicesMenu").GetComponent<ServicesMenu>();
        menu.InitializeServiceList(nodes);
        initDone = true;
    }

    void Update()
    {
        if (initDone)
        {
            if (nodes != null && connectedNodes.Count > 0)
            {
                //// scaler is what we use to divide by number of nodes
                //float scaler = 1.0f / connectedNodes.Count;
                //// add up all the position vectors and divide by the number of nodes
                //Vector3 sum = connectedNodes.Select(n => n.gameObject.transform.localPosition).Aggregate(Vector3.zero, (t, u) => t + u, t => t);
                //Vector3 offset = sum * scaler * -1.0f;
                //offset.y = -yMin;
                //transform.localPosition = Vector3.zero + offset;
                // apparently all that was useless.
                float yMin = connectedNodes.Min(n => n.gameObject.transform.localPosition.y) * transform.localScale.y;
                transform.localPosition = new Vector3(0f, -yMin + 0.3f, 0f);

                // update the unconnected nodes' position
                float xMax = connectedNodes.Max(n => n.gameObject.transform.localPosition.x); // get the highest x value of any connected node
                unconnectedGraph.gameObject.transform.localPosition = new Vector3(xMax + 5f, 0f, 0f);
                if (Input.GetKeyDown("v"))
                {
                    //if (currentRequest == null)
                    //{
                    //    List<GameObject> requestNodes = new List<GameObject>();
                    //    requestNodes.Add(nodes["cms"].gameObject);
                    //    requestNodes.Add(nodes["ems"].gameObject);
                    //    requestNodes.Add(nodes["qms"].gameObject);
                    //    requestNodes.Add(nodes["vms"].gameObject);
                    //    currentRequestObject = Instantiate(requestpf, transform);
                    //    currentRequest = currentRequestObject.GetComponent<RequestPip>();
                    //    currentRequest.Init(requestNodes);
                    //}
                    //Vector3 otherSum = new Vector3(0f, 0f, 0f);
                    //foreach (Node n in connectedNodes)
                    //{
                    //    otherSum = otherSum + n.transform.position;
                    //    Debug.Log(otherSum);
                    //}
                }
            }
        }
    }

    private Vector3 calcGraphCentroid()
    {
        float scaler = 1.0f / nodes.Count;
        Vector3 centroid = nodes.Values.Select(n => n.transform.position).Aggregate(new Vector3(0f, 0f, 0f), (t, u) => t + u, t => t * scaler);
        return centroid;
    }



    void LoadGMLFromFile(TextAsset f){
        string[] lines = f.text.Split('\n');
        int currentobject = -1; // 0 = graph, 1 = node, 2 = edge
        int stage = -1; // 0 waiting to open, 1 = waiting for attribute, 2 = waiting for id, 3 = waiting for label, 4 = waiting for source, 5 = waiting for target
        Node n = null;
        Dictionary<string,Node> nodes = new Dictionary<string,Node>();
        foreach (string line in lines){
	        string l = line.Trim();
	        string [] words = l.Split(' ');
	        foreach(string word in words) {
	            if (word == "graph" && stage == -1) {
	                currentobject = 0;
	            }
	            if (word == "node" && stage == -1) {
	                currentobject = 1;
	                stage = 0;	    
	            }
	            if (word == "edge" && stage == -1) {
	                currentobject = 2;
	                stage = 0;	    
	            }
	    if (word == "[" && stage == 0 && currentobject == 2){
	        stage = 1;
	    }
	  if (word == "[" && stage == 0 && currentobject == 1){
	    stage = 1;
	    GameObject go = Instantiate(nodepf, new Vector3(Random.Range(-width/2, width/2), Random.Range(-length/2, length/2), Random.Range(-height/2, height/2)), Quaternion.identity);
	    n = go.GetComponent<Node>();
	    n.transform.parent = transform;
	    n.SetEdgePrefab(edgepf);
	    continue;
	  }
	  if (word == "]"){
	    stage = -1;
	  }
	  if (word == "id" && stage == 1 && currentobject == 1){
	    stage = 2;
	    continue;
	  }
	  if (word == "label" && stage == 1 && currentobject == 1){
	    stage = 3;
	    continue;
	  }
	  if (stage == 2){
	    nodes.Add(word, n);
	    stage = 1;
	    break;
	  }
	  if (stage == 3){
	    n.name = word;
	    stage = 1;
	    break;
	  }
	  if (word == "source" && stage == 1 && currentobject == 2){
	    stage = 4;
	    continue;
	  }
	  if (word == "target" && stage == 1 && currentobject == 2){
	    stage = 5;
	    continue;
	  }
	  if (stage == 4){
	    n = nodes[word];
	    stage = 1;
	    break;
	  }
	  if (stage == 5){
	    n.AddEdge(nodes[word]);
	    stage = 1;
	    break;
	  }
	}
      }
    }

    private ProphetData CallProphet()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(System.String.Format("http://demo1986600.mockable.io/"));
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        ProphetData info = JsonUtility.FromJson<ProphetData>(jsonResponse);
        return info;
    }

    private float getTextScale(float parentScale)
    {
        return 0.005f / parentScale;
    }


}
