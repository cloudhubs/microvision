using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServicesMenu : MonoBehaviour
{
    public GameObject servicesArea;
    public GameObject button;
    public GameObject serviceEntryPf;
    IList<Node> services = new List<Node>();


    // Start is called before the first frame update
    void Start()
    {
        //servicesArea.transform.parent = transform;
        //RectTransform menuTransform = servicesArea.GetComponent<RectTransform>();
        //RectTransform buttonTransform = button.GetComponent<RectTransform>();
        //menuTransform.position
    }

    public void InitializeServiceList(Dictionary<string, Node> nodes)
    {
        int i = 0;
        foreach(KeyValuePair<string, Node> nodePair in nodes)
        {
            Node n = nodePair.Value;
            GameObject entryGo = Instantiate(serviceEntryPf);
            ServiceEntry entry = entryGo.GetComponent<ServiceEntry>();
            entry.SetName(nodePair.Key);
            entry.transform.parent = servicesArea.transform;
            float yOffset = i * 50.0f;
            entry.transform.localPosition = new Vector3(0f, -yOffset, 0f);
            i++;
        }
    }

    public void ToggleServicesMenu()
    {
        servicesArea.SetActive(!servicesArea.activeSelf);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
