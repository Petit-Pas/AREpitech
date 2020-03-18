using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ObjectPlacement : MonoBehaviour
{

    public ARCameraManager ARCamera;

    public RectTransform direction;

    public float Speed = 1;

    public Text debug;

    public GameObject placedObject; // prefab
    private GameObject spawnedObject;
    private Rigidbody spawnedObjectRigidbody;

    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> rayHits = new List<ARRaycastHit>();

    private ARPlaneManager planeManager = default;
    private TrackableCollection<ARPlane> planeCollection = default;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();

        planeCollection = planeManager.trackables;

    }

    bool TryGetTouchPosition(out Vector2 touchPosition)    
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            touchPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            return true;
        }
#else
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
#endif
        touchPosition = default;
        return false;
    }

    bool IsPointInDirectionElement(Vector2 position)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(direction, position, null);
    }

    void HandleInput(Vector2 position)
    {
        Vector2 directionOrientation = default;

        directionOrientation.x = (direction.position.x - position.x) / (direction.sizeDelta.x / 2);
        directionOrientation.y = (direction.position.y - position.y) / (direction.sizeDelta.y / 2);

        //debug.text = "";
        //debug.text += directionOrientation;

        Vector3 movement = new Vector3(directionOrientation.x, 0.0f, directionOrientation.y);
        if (spawnedObject != null)
            spawnedObjectRigidbody.AddForce(movement * Speed);
    }

    // Update is called once per frame
    void Update()
    {
        //debug.text = "default";
        debug.text = "x: ";
        debug.text += Camera.main.transform.rotation.x.ToString();
        debug.text += "y: ";
        debug.text += Camera.main.transform.rotation.y.ToString();
        debug.text += "y: ";
        debug.text += Camera.main.transform.rotation.z.ToString();
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (IsPointInDirectionElement(touchPosition))
            HandleInput(touchPosition);

        else if (raycastManager.Raycast(touchPosition, rayHits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = rayHits[0].pose;
            hitPose.position.y += 1;
            hitPose.position.x -= 0.1f;
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(placedObject, hitPose.position, hitPose.rotation);
                spawnedObjectRigidbody = spawnedObject.GetComponent<Rigidbody>();
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
                spawnedObjectRigidbody.velocity = default;
            }
        }
    }
}
