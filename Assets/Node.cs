using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    GameObject edgepf;
    public bool anchored { get; set; }

    List<GameObject>  edges  = new List<GameObject> ();
    List<SpringJoint> joints = new List<SpringJoint>();  
  
    void Start()
    {
        //transform.GetChild(0).GetComponent<TextMesh>().text = name;
    }

    void Update()
    {
        int i = 0;
        foreach (GameObject edge in edges)
        {
            edge.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            SpringJoint sj = joints[i];
            GameObject target = sj.connectedBody.gameObject;
            edge.transform.LookAt(target.transform);
            Vector3 ls = edge.transform.localScale;
            //////ls.z = Vector3.Distance(transform.position, target.transform.position);
            ls.z = Vector3.Distance(transform.localPosition, target.transform.localPosition);
            edge.transform.localScale = ls;
            edge.transform.position = new Vector3((transform.position.x + target.transform.position.x) / 2,
                            (transform.position.y + target.transform.position.y) / 2,
                            (transform.position.z + target.transform.position.z) / 2);
            i++;
        }
    }

    public void SetEdgePrefab(GameObject epf)
    {
        this.edgepf = epf;
    }

    public void AddEdge(Node n)
    {
        SpringJoint sj = gameObject.AddComponent<SpringJoint>();
        sj.autoConfigureConnectedAnchor = false;
        sj.anchor = new Vector3(0, 0.5f, 0);
        sj.connectedAnchor = new Vector3(0, 0, 0);
        sj.enableCollision = true;
        sj.connectedBody = n.GetComponent<Rigidbody>();
        sj.connectedBody.transform.parent = transform.parent;
        //GameObject edge = Instantiate(this.edgepf, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        //edge.transform.parent = transform.parent;
        GameObject edge = Instantiate(this.edgepf, transform.parent);
        edge.transform.localPosition = transform.localPosition;
        edges.Add(edge);
        joints.Add(sj);
    }

    public string GetLabelText()
    {
        return transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text;
    }

    public void SetLabelText(string label)
    {
        //transform.GetChild(0).GetComponent<TextMesh>().text = label;
        transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = label;
    }
    
}
