using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

// class needed because the node itself has a bounding box way to big to be reasonably clickable, and separating them proved to be too messy. So this should be attached to a box collider the size of the node itself, as a direct child
public class NodeClickable : MonoBehaviour
{
    
}