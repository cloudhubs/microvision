using Assets.Scripts.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ExpandableContextMenu : MonoBehaviour
{
    public GameObject ContentList;  // should be something capable of displaying list of child items intelligently (strong word for what I'm doing) (i.e. it should have a layout group and content fitter
    public GameObject MenuItemPf;   // prefab for the menu item
    public GameObject CloseButton;
    public GameObject SwitchModeButton;
    public GameObject Title;

    public bool IsEndpointMode { get; private set; }
    private static string EndpointModeButtonText = "Switch to requests"; // when in endpoint mode, button should offer to switch to request mode
    private static string RequestModeButtonText = "Switch to endpoints"; // vice versa

    private IList<GameObject> menuItems;

    // setup menu for just endpoints
    public void SetupMenu(IList<(string buttonText, string bodyText)> buttonAndContentTexts)
    {
        resetMenu();
        foreach ((string, string) item in buttonAndContentTexts)
        {
            var menuItem = CreateMenuItem(item.Item1, item.Item2);
            menuItems.Add(menuItem.gameObject);
        }
        gameObject.SetActive(true);
    }

    // setup menu for requests
    public void SetupMenu(IList<(string buttonText, string bodyText, IList<(Node, MsLabel)> requestSteps)> items)
    {
        resetMenu();
        foreach ((string, string, IList<(Node, MsLabel)>) item in items)
        {
            var menuItem = CreateMenuItem(item.Item1, item.Item2);
            menuItem.SetRequest(item.Item3);
            menuItems.Add(menuItem.gameObject);
        }
        gameObject.SetActive(true);
    }

    public ExpandableContextMenuItem CreateMenuItem(string buttonText, string contentText)
    {
        // make menu item child of the content list
        GameObject menuItemObj = Instantiate(MenuItemPf, ContentList.transform);
        ExpandableContextMenuItem menuItem = menuItemObj.GetComponent<ExpandableContextMenuItem>();
        menuItem.SetButtonText(buttonText);
        menuItem.SetContentText(contentText);
        return menuItem;
    }

    public void SetIsEndpointMode(bool endpointMode)
    {
        IsEndpointMode = endpointMode;
        if (endpointMode)
        {
            SwitchModeButton.GetComponentInChildren<TextMeshProUGUI>().SetText(EndpointModeButtonText);
        }
        else
        {
            SwitchModeButton.GetComponentInChildren<TextMeshProUGUI>().SetText(RequestModeButtonText);
        }
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    public void SetTitle(string newTitle)
    {
        TextMeshProUGUI text = Title.GetComponent<TextMeshProUGUI>();
        text.SetText(newTitle);
    }

    private void resetMenu()
    {
        if (menuItems != null)
        {
            menuItems.Clear();

            while (ContentList.transform.childCount > 0)
            {
                DestroyImmediate(ContentList.transform.GetChild(0).gameObject);
            }
        }
        menuItems = new List<GameObject>();
    }
}
