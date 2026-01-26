using CesiumForUnity;
using UnityEngine;
using System;
[RequireComponent(typeof(CesiumGlobeAnchor))]
public class PointOfInterest : MonoBehaviour


{
    public Transform labelTransform;
    public Action<double, double> onAddressToCoordinatesResolved; // added
    public POIData poiData;
    private CesiumGlobeAnchor globeAnchor;

    public void Init(POIData data)
    {
        onAddressToCoordinatesResolved += OnAddressToCoordinatesResolved;

        poiData = data;
        globeAnchor = GetComponent<CesiumGlobeAnchor>();
        AppManager.instance.geoCoder.Geocode(poiData.address, onAddressToCoordinatesResolved);
    }

    public void OnAddressToCoordinatesResolved(double lat, double lon) // added
    {
        onAddressToCoordinatesResolved -= OnAddressToCoordinatesResolved;
        poiData.latitude = (float)lat;
        poiData.longitude = (float)lon;
        globeAnchor.SetPositionLongitudeLatitudeHeight(lon, lat, poiData.height);

    
    }
public void UpdateLabelOrientation(Transform playerTransform)
    {
        if (labelTransform != null && playerTransform != null)
        {
            Vector3 directionToPlayer = playerTransform.position - labelTransform.position;
            directionToPlayer.y = 0; // Keep the label upright
            if (directionToPlayer.sqrMagnitude > 0.001f)
            {
                labelTransform.rotation = Quaternion.LookRotation(-directionToPlayer);
            }
        }
    }
    void Update()
    {
         UpdateLabelOrientation(AppManager.instance.player.transform);
    }
}
