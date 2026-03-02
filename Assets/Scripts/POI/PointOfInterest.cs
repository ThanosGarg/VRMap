using CesiumForUnity;
using UnityEngine;
using System;
using UnityEngine.UI;
/// <summary>
/// Handles the visualization and geospatial anchoring of a Point of Interest (POI) in the VRMap application.
/// Manages icon, label orientation, and geocoding updates for the POI.
/// </summary>
[RequireComponent(typeof(CesiumGlobeAnchor))]
public class PointOfInterest : MonoBehaviour
{
    /// <summary>
    /// Transform of the label UI element for this POI.
    /// </summary>
    public Transform labelTransform;

    /// <summary>
    /// Event called when address geocoding resolves to coordinates (lat, lon).
    /// </summary>
    public Action<double, double> onAddressToCoordinatesResolved;

    /// <summary>
    /// The POI data associated with this instance.
    /// </summary>
    public POIData poiData;

    /// <summary>
    /// Reference to the CesiumGlobeAnchor for geospatial placement.
    /// </summary>
    private CesiumGlobeAnchor globeAnchor;

    /// <summary>
    /// Reference to the TourManager for icon lookup and management.
    /// </summary>
    private TourManager tourManager;

    /// <summary>
    /// Initializes the POI with data and manager, sets up globe anchor and icon.
    /// </summary>
    /// <param name="data">POI data to use for this instance.</param>
    /// <param name="manager">Reference to the TourManager.</param>
    public void Init(POIData data, TourManager manager)
    {
        // Assign data and manager references
        poiData = data;
        tourManager = manager;

        // Get the CesiumGlobeAnchor component and set its position
        globeAnchor = GetComponent<CesiumGlobeAnchor>();
        globeAnchor.SetPositionLongitudeLatitudeHeight(poiData.longitude, poiData.latitude, poiData.height);

        // Optionally, geocode the address to update coordinates (commented out)
        // AppManager.instance.geoCoder.Geocode(poiData.address, onAddressToCoordinatesResolved);

        // Set the icon based on POI type
        foreach (POIIcon icon in tourManager.Icons)
        {
            if (icon.type == poiData.poiType)
            {
                UpdateIcon(icon.icon);
                break;
            }
        }
    }

    /// <summary>
    /// Updates the icon displayed for this POI.
    /// </summary>
    /// <param name="icon">Sprite to use as the icon.</param>
    public void UpdateIcon(Sprite icon)
    {
        // Get the Image component from the label transform and set its sprite
        Image iconImage = labelTransform.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = icon;
        }
    }

    /// <summary>
    /// Callback for when geocoding resolves an address to coordinates.
    /// Updates POI data and globe anchor position.
    /// </summary>
    /// <param name="lat">Latitude resolved from geocoding.</param>
    /// <param name="lon">Longitude resolved from geocoding.</param>
    public void OnAddressToCoordinatesResolved(double lat, double lon)
    {
        // Remove this handler to prevent multiple calls
        onAddressToCoordinatesResolved -= OnAddressToCoordinatesResolved;
        // Update POI data with new coordinates
        poiData.latitude = (float)lat;
        poiData.longitude = (float)lon;
        // Update the globe anchor position
        globeAnchor.SetPositionLongitudeLatitudeHeight(lon, lat, poiData.height);
    }

    /// <summary>
    /// Rotates the label to always face the player horizontally (billboarding).
    /// </summary>
    /// <param name="playerTransform">Transform of the player/camera.</param>
    public void UpdateLabelOrientation(Transform playerTransform)
    {
        if (labelTransform != null && playerTransform != null)
        {
            // Calculate direction to player, ignoring vertical difference
            Vector3 directionToPlayer = playerTransform.position - labelTransform.position;
            directionToPlayer.y = 0; // Keep the label upright
            if (directionToPlayer.sqrMagnitude > 0.001f)
            {
                // Rotate label to face the player
                labelTransform.rotation = Quaternion.LookRotation(-directionToPlayer);
            }
        }
    }

    /// <summary>
    /// Unity Update loop: keeps the label facing the player each frame.
    /// </summary>
    void Update()
    {
        // Update label orientation to face the player
        UpdateLabelOrientation(AppManager.instance.player);
    }
}

