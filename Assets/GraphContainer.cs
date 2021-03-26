using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphContainer: MonoBehaviour
{
    public GameObject graphPrefab;
    Graph graph;

    void Start()
    {
        GameObject go = Instantiate(graphPrefab, transform);
        go.transform.localPosition = Vector3.zero;
        graph = go.GetComponent<Graph>();
    }

    void Update()
    {
    }
}
