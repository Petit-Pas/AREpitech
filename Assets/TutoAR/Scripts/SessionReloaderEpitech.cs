using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class SessionReloaderEpitech : MonoBehaviour
{
    public ARSession session;
    public GameObject sessionPrefab;
    public Button resetButton;

    IEnumerator DoReload()
    {
        Destroy(session.gameObject);
        yield return null;

        if (sessionPrefab != null)
        {
            session = Instantiate(sessionPrefab).GetComponent<ARSession>();
        }
    }

    public void ReloadSession()
    {
        if (session != null)
        {
            StartCoroutine(DoReload());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
