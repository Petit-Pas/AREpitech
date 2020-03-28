using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackMultiple : MonoBehaviour
{
    [Header("The length of this list must match the number of images in Reference Image Library")]
    public List<GameObject> ObjectsToPlace;

    public Text debug = null;

    private int refImageCount;
    private Dictionary<string, GameObject> allPrefabs;
    private Dictionary<System.IntPtr, GameObject> allObjects;
    private ARTrackedImageManager arTrackedImageManager;
    private IReferenceImageLibrary refLibrary;

    void Awake()
    {
        arTrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    private void Start()
    {
        refLibrary = arTrackedImageManager.referenceLibrary;
        refImageCount = refLibrary.count;
        LoadPrefabsDictionary();
    }

    void LoadPrefabsDictionary()
    {
        allPrefabs = new Dictionary<string, GameObject>();
        for (int i = 0; i < refImageCount; i++)
        {
            allPrefabs.Add(refLibrary[i].name, ObjectsToPlace[i]);
        }
    }

    GameObject InstanciateTrackedObject(string _imageName, Transform transform)
    {
		return Instantiate(allPrefabs[_imageName], transform);
	}

    public void OnImageChanged(ARTrackedImagesChangedEventArgs _args)
    {
        foreach (var addedImage in _args.added)
        {
            allObjects.Add(addedImage.nativePtr, InstanciateTrackedObject(addedImage.referenceImage.name, addedImage.transform));
        }

        foreach (var updated in _args.updated)
        {
            allObjects[updated.nativePtr].transform.position = updated.transform.position;
            allObjects[updated.nativePtr].transform.rotation = updated.transform.rotation;
        }

        foreach (var removed in _args.removed)
        {
            Destroy(allObjects[removed.nativePtr]);
        }

        if (debug != null)
        {
            debug.text = "";
            debug.text += allObjects.Count;
        }
    }
}