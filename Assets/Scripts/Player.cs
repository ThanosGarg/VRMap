using System;
using UnityEngine;

/// <summary>
/// Handles player interactions with Points of Interest (POIs) in the scene.
/// Triggers events when entering or exiting a POI collider.
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// Event invoked when the player enters a POI. Passes the POI's data.
    /// </summary>
    public Action<POIData> onEnterPOI;

    /// <summary>
    /// Event invoked when the player exits the current POI.
    /// </summary>
    public Action onExitPOI;

    /// <summary>
    /// The POI the player is currently inside, or null if not in any.
    /// </summary>
    public PointOfInterest currentPOI;

    /// <summary>
    /// Unity callback when the player's collider enters another collider.
    /// Checks if the collider belongs to a POI and triggers entry logic.
    /// </summary>
    /// <param name="other">The collider the player entered.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Try to get a PointOfInterest component from the collider
        PointOfInterest poi = other.GetComponent<PointOfInterest>();
        if (poi != null)
        {
            // Set as current POI and invoke entry event
            currentPOI = poi;
            onEnterPOI?.Invoke(poi.poiData);
        }
    }

    /// <summary>
    /// Unity callback when the player's collider exits another collider.
    /// Checks if the collider belongs to the current POI and triggers exit logic.
    /// </summary>
    /// <param name="other">The collider the player exited.</param>
    private void OnTriggerExit(Collider other)
    {
        // Try to get a PointOfInterest component from the collider
        PointOfInterest poi = other.GetComponent<PointOfInterest>();
        // Only trigger exit if this is the current POI
        if (poi != null && currentPOI == poi)
        {
            onExitPOI?.Invoke();
            currentPOI = null;
        }
    }
}
