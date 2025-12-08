using CesiumForUnity;
using UnityEngine;
using System;
[RequireComponent(typeof(CesiumGlobeAnchor))]
public class PointOfInterest : MonoBehaviour

{
    public Action<double, double> onAddressToCoordinatesResolved; // added
    public POIData poiData;
    private CesiumGlobeAnchor globeAnchor;

    void Start()
    {
        onAddressToCoordinatesResolved += OnAddressToCoordinatesResolved;
    }
    public void OnAddressToCoordinatesResolved(double lat,double lon) // added
    {

       
      onAddressToCoordinatesResolved -= OnAddressToCoordinatesResolved;
        poiData.latitude =(float) lat;
        poiData.longitude =(float) lon;
        //.SetOriginLongitudeLatitudeHeight(lon, lat, 150f);

    }

    public void Init(POIData data)

    {

        poiData = data;
        globeAnchor = GetComponent<CesiumGlobeAnchor>();
        AppManager.instance.geoCoder.Geocode(poiData.address, onAddressToCoordinatesResolved);
    }
}
