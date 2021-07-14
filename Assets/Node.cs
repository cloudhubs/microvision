using Assets.Scripts.model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{

    GameObject edgepf;
    public bool anchored { get; set; }
    public IList<MsLabel> endpoints { get; set; }

    List<GameObject>  edges  = new List<GameObject> ();
    List<SpringJoint> joints = new List<SpringJoint>();

    GameObject uiManagerObj;
    UIManager uiManager;

    public Material DefaultMat;
    public Material ActiveMat;
    public Material neighborMat;

    void Start()
    {
        //transform.GetChild(0).GetComponent<TextMesh>().text = name;
        uiManagerObj = GameObject.Find("UIManager");
        uiManager = uiManagerObj.GetComponent<UIManager>();
    }

    void Update()
    {
        int i = 0;
        foreach (GameObject edge in edges)
        {
            edge.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            SpringJoint sj = joints[i];
            GameObject target = sj.connectedBody.gameObject;
            if (sj.spring > 0)
            {
                sj.spring = sj.spring - 0.001f;
                if (sj.spring < 0)
                {
                    sj.spring = 0;
                }
            }
            edge.transform.LookAt(target.transform);
            Vector3 ls = edge.transform.localScale;
            ls.z = Vector3.Distance(transform.localPosition, target.transform.localPosition);
            edge.transform.localScale = ls;
            edge.transform.position = new Vector3((transform.position.x + target.transform.position.x) / 2,
                            (transform.position.y + target.transform.position.y) / 2,
                            (transform.position.z + target.transform.position.z) / 2);
            i++;
        }
    }

    private void OnMouseDown()
    {
        // Check if the mouse was clicked over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }
        }
        uiManager.PopulateEndpointContextMenu(this);
    }

    public void SetEdgePrefab(GameObject epf)
    {
        this.edgepf = epf;
    }

    public GameObject AddEdge(Node n)
    {
        SpringJoint joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = new Vector3(0, 0.5f, 0);
        joint.connectedAnchor = new Vector3(0, 0, 0);
        joint.enableCollision = true;
        joint.connectedBody = n.GetComponent<Rigidbody>();
        joint.connectedBody.transform.parent = transform.parent;
        joint.damper = 5f;
        joint.spring = 10f;
        GameObject edge = Instantiate(this.edgepf, transform.parent);
        edge.transform.localPosition = transform.localPosition;
        edges.Add(edge);
        joints.Add(joint);
        return edge;
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

    public void SetDefaultMat()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = DefaultMat;
    }
    public void SetActiveMat()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = ActiveMat;
    }
    public void SetNeighborMat()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = neighborMat;
    }

}
