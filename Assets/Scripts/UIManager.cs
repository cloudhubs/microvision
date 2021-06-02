using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.model;

public class UIManager : MonoBehaviour
{
    public GameObject contextMenuObj;

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
}