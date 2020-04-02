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
    public RectTransform controls;
    [Header("Ball")]
    public GameObject ball;
    [Header("Walls")]
    public Material defaultMaterial;
    public Material highlightMaterial;
    public GameObject tagsParent;

    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> rayHits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition, out TouchPhase phase)) return;
        if (!IsPointInDirectionElement(touchPosition))
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && phase == TouchPhase.Began)
            {
                //ARTrackedImage selection = hit.transform.GetComponent<ARTrackedImage>();
                if (hit.transform.CompareTag("WALL"))
                {
                    Transform clone = Instantiate(hit.transform, tagsParent.transform);
                    clone.tag = "FIXED_WALL";
                }
                else if (hit.transform.CompareTag("START"))
                {
                    GameObject[] starts = GameObject.FindGameObjectsWithTag("FIXED_START");
                    foreach (GameObject start in starts)
                        GameObject.Destroy(start);
                    Transform clone = Instantiate(hit.transform, tagsParent.transform);
                    clone.tag = "FIXED_START";
                }
                else if (hit.transform.CompareTag("END"))
                {
                    GameObject[] ends = GameObject.FindGameObjectsWithTag("FIXED_END");
                    foreach (GameObject end in ends)
                        GameObject.Destroy(end);
                    Transform clone = Instantiate(hit.transform, tagsParent.transform);
                    clone.tag = "FIXED_END";
                }
                else if (hit.transform.CompareTag("FIXED_WALL"))
                {
                    Renderer selectionRenderer = hit.transform.GetComponent<Renderer>();
                    if (selectionRenderer != null)
                    {
                        if (selectionRenderer.sharedMaterial == highlightMaterial)
                        {
                            selectionRenderer.material = defaultMaterial;
                        }
                        else
                        {
                            selectionRenderer.material = highlightMaterial;
                        }
                    }
                }
                else if (raycastManager.Raycast(touchPosition, rayHits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = default;
                    GameObject[] starts = GameObject.FindGameObjectsWithTag("FIXED_START");
                    if (starts.Length == 0)
                    {
                        starts = GameObject.FindGameObjectsWithTag("START");
                    }
                    if (starts.Length < 1)
                    {
                        hitPose = rayHits[0].pose;
                        hitPose.position.y += 1;
                        hitPose.position.x -= 0.1f;
                    } else
                    {
                        hitPose.position.x = starts[0].transform.position.x;
                        hitPose.position.y = starts[0].transform.position.y + 1;
                        hitPose.position.z = starts[0].transform.position.z;
                    }
                    ball.SetActive(true);
                    ball.transform.position = hitPose.position;
                    ball.GetComponent<Rigidbody>().velocity = default;
                }
            }
        }
    }

    bool TryGetTouchPosition(out Vector2 touchPosition, out TouchPhase phase)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            phase = Input.GetTouch(0).phase;
            return true;
        }
        touchPosition = default;
        phase = TouchPhase.Canceled;
        return false;
    }

    bool IsPointInDirectionElement(Vector2 position)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(controls, position, null);
    }

    public void Reset()
    {
        GameObject[] starts = GameObject.FindGameObjectsWithTag("FIXED_START");
        foreach (GameObject start in starts)
            GameObject.Destroy(start);
        GameObject[] ends = GameObject.FindGameObjectsWithTag("FIXED_END");
        foreach (GameObject end in ends)
            GameObject.Destroy(end);
        GameObject[] walls = GameObject.FindGameObjectsWithTag("FIXED_WALL");
        foreach (GameObject wall in walls)
            GameObject.Destroy(wall);
    }

    public void ResetBall()
    {
        Pose hitPose = default;
        GameObject[] starts = GameObject.FindGameObjectsWithTag("FIXED_START");
        if (starts.Length == 0)
        {
            starts = GameObject.FindGameObjectsWithTag("START");
        }
        if (starts.Length > 0)
        {
            hitPose.position.x = starts[0].transform.position.x;
            hitPose.position.y = starts[0].transform.position.y + 1;
            hitPose.position.z = starts[0].transform.position.z;
        }
        ball.SetActive(true);
        ball.transform.position = hitPose.position;
        ball.GetComponent<Rigidbody>().velocity = default;
    }
}
