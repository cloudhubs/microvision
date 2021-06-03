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
    private ARPlaneManager planeManager;
    private Vector2 touchPosition;

    private bool isSearching;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        raycaster = GetComponent<ARRaycastManager>(); // component that was required above
        planeManager = GetComponent<ARPlaneManager>();
        isSearching = true;
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
        if (!isSearching)
            return;
        if (!TryGetTouchPosition(out touchPosition))
            return;
        // fire the ray! 
        if (raycaster.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            spawnedObject = Instantiate(toSpawn, hitPose.position, hitPose.rotation);
            // stop looking for planes
            isSearching = false;
        }
    }
}
