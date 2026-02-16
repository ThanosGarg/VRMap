using CesiumForUnity;
using UnityEngine;
using System;
using UnityEngine.UI;
[RequireComponent(typeof(CesiumGlobeAnchor))]
public class PointOfInterest : MonoBehaviour


{

    public Transform labelTransform;
    public Action<double, double> onAddressToCoordinatesResolved; // added
    public POIData poiData;
    private CesiumGlobeAnchor globeAnchor;
    private TourManager tourManager;
    public void Init(POIData data, TourManager manager)
    {
        // onAddressToCoordinatesResolved += OnAddressToCoordinatesResolved;

        poiData = data;
        tourManager = manager;
        globeAnchor = GetComponent<CesiumGlobeAnchor>();
        globeAnchor.SetPositionLongitudeLatitudeHeight(poiData.longitude, poiData.latitude, poiData.height);

        // AppManager.instance.geoCoder.Geocode(poiData.address, onAddressToCoordinatesResolved);
        foreach (POIIcon icon in tourManager.Icons)
        {
            if (icon.type == poiData.poiType)
            {
                UpdateIcon(icon.icon);
                break;
            }
        }
    }
    public void UpdateIcon(Sprite icon)
    {
        Image iconImage = labelTransform.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = icon;
        }
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
        UpdateLabelOrientation(AppManager.instance.player);
    }
}

