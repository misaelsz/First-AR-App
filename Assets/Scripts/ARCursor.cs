using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARCursor : MonoBehaviour
{
    public GameObject cursorChildObject;
    public GameObject objectToPlace;
    public ARRaycastManager raycastManager;
    public ARPlaneManager m_ARPlaneManager;
    public bool useCursor = true;

    private GameObject InstantiatedObj;
    private Transform aRSession;
    private Quaternion objRotation;

    void Start()
    {
        cursorChildObject.SetActive(useCursor);
        aRSession = GameObject.Find("AR Session Origin").transform.GetChild(0);
    }
    void Update()
    {
        if (useCursor)
        {
            UpdateCursor();
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (useCursor)
            {
                if (InstantiatedObj != null)
                    GameObject.Destroy(InstantiatedObj);

                InstantiatedObj = GameObject.Instantiate(objectToPlace, transform.position, transform.rotation);

                InstantiatedObj.transform.LookAt(aRSession, Vector3.up);

                InstantiatedObj.transform.rotation = new Quaternion(0f, InstantiatedObj.transform.rotation.y, 0f, InstantiatedObj.transform.rotation.w);
                
                SetAllPlanesActive(false);
            }
            else
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                raycastManager.Raycast(Input.GetTouch(0).position, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
                if (hits.Count > 0)
                {
                    if (InstantiatedObj != null)
                        GameObject.Destroy(InstantiatedObj);
                    InstantiatedObj = GameObject.Instantiate(objectToPlace, hits[0].pose.position, hits[0].pose.rotation);
                    SetAllPlanesActive(false);
                }
            }
        }
    }

    void UpdateCursor()
    {
        Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);


        if (hits.Count > 0)
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
        }
    }

    void SetAllPlanesActive(bool value)
    {
        foreach (var plane in m_ARPlaneManager.trackables)
            plane.gameObject.SetActive(value);
    }
}