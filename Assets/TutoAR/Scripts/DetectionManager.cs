using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DetectionManager : MonoBehaviour
{
    private ARPointCloudManager CloudManager;
    public Camera arCamera;
    public RectTransform Frame;
    public GameObject PlacedObject;
    public GameObject PlacedHolder;
    public GameObject PlacedAverage;

    private TrackableCollection<ARPointCloud> arpcList;
    private Vector3 avgPos = Vector3.zero;
    private int pointNumber = 0;
    int state = 0;

    // Start is called before the first frame update
    void Start()
    {
        CloudManager = this.GetComponent<ARPointCloudManager>();
        arpcList = CloudManager.trackables;
    }

    public int PointCloudProcessPeriod = 0;

    // Update is called once per frame
    void Update()
    {
        if (state == 0)
        {
            if (Input.GetMouseButtonUp(0))
            {
                state = 1;
            }
        }
        else if (state == 1)
        {
            if (processCount == PointCloudProcessPeriod)
            {
                ProcessPointClouds();
                processCount = 0;
            }
            else
            {
                processCount += 1;
            }
            if (Input.GetMouseButtonUp(0))
                state = 2;
        }

        else if (state == 2)
        {
            avgPos /= pointNumber;
            Instantiate(PlacedAverage, avgPos, Quaternion.identity, PlacedHolder.transform);
            state = 3;
        }
    }

    bool IsPointInUIElement (Vector3 pos)
    {
        Vector2 screenPoint = arCamera.WorldToScreenPoint(pos);
        return RectTransformUtility.RectangleContainsScreenPoint(Frame, screenPoint, null);
    }

    private int lastIndex = 0;
    private int processCount = 0;

    void ProcessPointClouds()
    {
        foreach (ARPointCloud arpc in arpcList)
        {
            for (int i = lastIndex; i < arpc.positions.Value.Length; i++)
            {
                Vector3 pos = arpc.positions.Value[i];
                if (IsPointInUIElement(pos))
                {
                    pointNumber++;
                    avgPos += pos;
                    Instantiate(PlacedObject, pos, Quaternion.identity, PlacedHolder.transform) ;
                }
                else
                {

                }
            }
            lastIndex = arpc.positions.Value.Length;
        }
    }
}
