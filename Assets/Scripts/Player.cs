using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Action<POIData> onEnterPOI;
    public Action onExitPOI;

    
    public PointOfInterest currentPOI;

    private void OnTriggerEnter(Collider other)
    {
        PointOfInterest poi = other.GetComponent<PointOfInterest>();
        if (poi != null)
        {
            currentPOI = poi;
           onEnterPOI?.Invoke(poi.poiData);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PointOfInterest poi = other.GetComponent<PointOfInterest>();
        if (poi != null && currentPOI == poi)
        {
            onExitPOI?.Invoke();
            currentPOI = null;
        }
    
    }

}
