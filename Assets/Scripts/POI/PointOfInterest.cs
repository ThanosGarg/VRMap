using CesiumForUnity;
using UnityEngine;
using Unity.Mathematics; // added

[RequireComponent(typeof(CesiumGlobeAnchor))]
public class PointOfInterest : MonoBehaviour
{
    public POIData poiData;
    private CesiumGlobeAnchor globeAnchor;

    public void Init(POIData data)
    {
        poiData = data;
        globeAnchor = GetComponent<CesiumGlobeAnchor>();
        globeAnchor.longitudeLatitudeHeight = new double3(poiData.longitude, poiData.latitude, poiData.height);
    }
}
