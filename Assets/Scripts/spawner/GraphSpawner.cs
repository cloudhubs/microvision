using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))] // automagically create the manager when we put this script on an object
public class GraphSpawner : MonoBehaviour
{

    public GameObject toSpawn;
    private GameObject spawnedObject;
    private ARRaycastManager raycaster;
    private Vector2 touchPosition;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        raycaster = GetComponent<ARRaycastManager>(); // component that was required above
    }

    bool TryGetTouchPosition(out Vector2 touchPos)
    {
        if (Input.touchCount > 0)
        {
            touchPos = Input.GetTouch(0).position;
            return true;
        }
        touchPos = default;
        return false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (!TryGetTouchPosition(out touchPosition))
        {
            return;
        }
        // fire the ray! 
        if (raycaster.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            spawnedObject = Instantiate(toSpawn, hitPose.position, hitPose.rotation);
        }
    }
}
