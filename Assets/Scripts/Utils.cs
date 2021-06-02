using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Utils
{
    public static void RefreshLayoutGroupsImmediateAndRecursive(GameObject root)
    {
        var componentsInChildren = root.GetComponentsInChildren<LayoutGroup>(true);
        foreach (var layoutGroup in componentsInChildren)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }
        var parent = root.GetComponent<LayoutGroup>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
    }
}
