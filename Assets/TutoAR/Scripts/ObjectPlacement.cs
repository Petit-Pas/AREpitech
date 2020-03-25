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

    public Text debug = null;

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

    double get_trigonometric_angle(double degrees)
    {
        return (Math.PI * (degrees * 100)) / 180;
    }

    void HandleInput(Vector2 position)
    {
        Vector2 directionOrientation = default;

        if (spawnedObject != null)
        {

            directionOrientation.x = -((direction.position.x - position.x) / (direction.sizeDelta.x / 2));
            directionOrientation.y = -((direction.position.y - position.y) / (direction.sizeDelta.y / 2));

            if (debug != null)
            {
                debug.text = "";
                debug.text += directionOrientation;
            }

            Vector3 forwardOnPlane = Vector3.ProjectOnPlane(ARCamera.transform.forward, Vector3.up).normalized;

            float angle = (float)Math.Atan2(-directionOrientation.x, directionOrientation.y) * Mathf.Rad2Deg;

            Vector3 movement = Quaternion.AngleAxis(-angle, Vector3.up) * forwardOnPlane;
            spawnedObjectRigidbody.AddForce(movement * Speed);
        }
    }

    // Update is called once per frame
    void Update()
    {
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
