using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnconnectedGraph : MonoBehaviour
{
    ISet<Node> nodes = new HashSet<Node>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AddNode(Node node)
    {
        // add this node to our set and make its position depend on us
        nodes.Add(node);
        node.transform.parent = transform;
        // arrange into a square
        int rows = Mathf.CeilToInt(Mathf.Sqrt(nodes.Count));
        if (rows > 0)
        {
            Vector3 firstSlot = new Vector3(0f, 0f, (3.0f * (rows - 1)) / 2.0f);
            int i = 0;
            foreach (Node n in nodes)
            {
                int rowNum = i % rows;
                int colNum = i / rows;
                Vector3 offset = new Vector3(colNum * 3.0f, 0f, rowNum * -3.0f);
                Vector3 slot = firstSlot + offset;
                slot.Scale(transform.parent.localScale);
                n.transform.localPosition = slot;
                i++;
            }
        }
    }
}
