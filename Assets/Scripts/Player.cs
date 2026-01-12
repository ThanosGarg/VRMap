using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public PointOfInterest currentPOI;

    public Action<POIData> POIEntered;
    public Action POIExited;

    private void OnTriggerEnter(Collider other)
    {
        PointOfInterest poi = other.GetComponent<PointOfInterest>();
        if (poi != null)
        {
            currentPOI = poi;
            Debug.Log("Entered POI: " + poi.poiData.address);
            POIEntered?.Invoke(currentPOI.poiData);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PointOfInterest poi = other.GetComponent<PointOfInterest>();
        if (poi != null && currentPOI == poi)
        {
            Debug.Log("Exited POI: " + poi.poiData.address);
            currentPOI = null;
            POIExited?.Invoke();
        }
    }
}
