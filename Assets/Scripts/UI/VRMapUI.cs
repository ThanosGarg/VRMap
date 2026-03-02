using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles the VR map UI, including map display and POI information updates.
/// Inherits from VRPlayerUI.
/// </summary>
public class VRMapUI : VRPlayerUI
{
    /// <summary>
    /// Reference to the OpenStreetMapViewer component. Assign in Inspector.
    /// </summary>
    public OpenStreetMapViewer mapViewer; // Assign in Inspector

    /// <summary>
    /// Text field for displaying current POI information.
    /// </summary>
    public TextMeshProUGUI CurrentPOIINFO;

    /// <summary>
    /// Map zoom level.
    /// </summary>
    public int zoomLevel = 10;

    /// <summary>
    /// Unity Start method. Initializes the map coordinates and POI info text.
    /// </summary>
    void Start()
    {
        // Set the map to the player's current coordinates and zoom level
        mapViewer.UpdateCoordinates(AppManager.instance.tourManager.lat, AppManager.instance.tourManager.lon, zoomLevel);
        // Initialize POI info text
        CurrentPOIINFO.text = "No POI Selected";
    }

    /// <summary>
    /// Updates the UI with information about the currently selected POI.
    /// </summary>
    /// <param name="currentPOI">The currently selected POI data</param>
    public void UpdatePOIINFO(POIData currentPOI)
    {
        // If a POI is selected, display its info; otherwise, show default message
        if (currentPOI != null && CurrentPOIINFO != null)
        {
            CurrentPOIINFO.text = $"POI Name: {currentPOI.poiName}\n" +
                                  $"Type: {currentPOI.poiType}\n" +
                                  $"Address: {currentPOI.address}\n";
        }
        else if (CurrentPOIINFO != null)
        {
            CurrentPOIINFO.text = "No POI Selected";
        }
    }

    /// <summary>
    /// Clears the POI info text and logs exit from POI.
    /// </summary>
    void ClearPOIINFO()
    {
        // Log exit and reset POI info text
        Debug.Log("Exited POI");
        if (CurrentPOIINFO != null)
        {
            CurrentPOIINFO.text = "No POI Selected";
        }
    }
}
