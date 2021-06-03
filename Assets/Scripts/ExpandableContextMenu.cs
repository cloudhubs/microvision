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
    public GameObject Title;
    public GameObject GraphBase;

    private IList<GameObject> menuItems;

    public void SetupMenu(IList<(string, string)> buttonAndContentTexts)
    {
        resetMenu();
        foreach ((string, string) item in buttonAndContentTexts)
        {
            CreateMenuItem(item.Item1, item.Item2);
        }
        gameObject.SetActive(true);
    }

    public void SetupMenu(IList<(string, string, IList<(Node, MsLabel)> requestSteps)> menuItems)
    {
        resetMenu();
        foreach ((string, string, IList<(Node, MsLabel)>) item in menuItems)
        {
            var menuItem = CreateMenuItem(item.Item1, item.Item2);
            menuItem.SetRequest(item.Item3);
        }
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
