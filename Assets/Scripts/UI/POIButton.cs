using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI button component representing a Point of Interest (POI) in the VRMap application.
/// Handles initialization and click events to update the player's location and UI.
/// </summary>
public class POIButton : MonoBehaviour
{
    /// <summary>
    /// Reference to the TextMeshProUGUI component displaying the POI name.
    /// </summary>
    public TextMeshProUGUI buttonText;

    /// <summary>
    /// The POI data associated with this button.
    /// </summary>
    public POIData poiData;

    /// <summary>
    /// Reference to the TourManager for updating player location.
    /// </summary>
    private TourManager tourManager;

    /// <summary>
    /// Reference to the Unity UI Button component.
    /// </summary>
    public Button buttonComponent;

    /// <summary>
    /// Initializes the POIButton with POI data and a reference to the TourManager.
    /// Sets up the button text and click event listener.
    /// </summary>
    /// <param name="data">The POI data to associate with this button.</param>
    /// <param name="manager">Reference to the TourManager.</param>
    public void Init(POIData data, TourManager manager)
    {
        poiData = data;
        tourManager = manager;

        // Set the button text to the POI name, if the text component is assigned
        if (buttonText != null)
        {
            buttonText.text = data.poiName;
        }

        // Get the Button component attached to this GameObject
        buttonComponent = GetComponent<Button>();
        if (buttonComponent != null)
        {
            // Add the click event listener
            buttonComponent.onClick.AddListener(OnButtonClicked);
        }
    }

    /// <summary>
    /// Called when the POI button is clicked.
    /// Updates the player's location, POI info UI, and map coordinates.
    /// </summary>
    public void OnButtonClicked()
    {
        // Ensure both the TourManager and POI data are assigned
        if (tourManager != null && poiData != null)
        {
            // Move the player to the POI's location
            tourManager.UpdatePlayerLocation(poiData.latitude, poiData.longitude, poiData.height);

            // Update the POI info panel in the UI
            AppManager.instance.VRMapUI.UpdatePOIINFO(poiData);

            // Update the map viewer to center on the POI
            AppManager.instance.VRMapUI.mapViewer.UpdateCoordinates(poiData.latitude, poiData.longitude, 15);
        }
    }
}
