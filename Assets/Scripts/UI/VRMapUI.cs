using System.Collections;
using TMPro;
using UnityEngine;

public class VRMapUI : VRPlayerUI
{

    public OpenStreetMapViewer mapViewer; // Assign in Inspector
    public TextMeshProUGUI CurrentPOIINFO;
    public int zoomLevel = 10;
    void OnEnable()
    {
        AppManager.instance.player.onEnterPOI += UpdatePOIINFO;
        AppManager.instance.player.onExitPOI += ClearPOIINFO;
    }
    void OnDisable()
    {

        AppManager.instance.player.onEnterPOI -= UpdatePOIINFO;
        AppManager.instance.player.onExitPOI -= ClearPOIINFO;
    }


    public void UpdatePlayerMap()
    {
        mapViewer.UpdateCoordinates(AppManager.instance.tourManager.lat, AppManager.instance.tourManager.lon, zoomLevel);
    }
    void UpdatePOIINFO(POIData currentPOI)
    {
        Debug.Log($"Entered POI: {currentPOI.poiName}");
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
    void ClearPOIINFO()
    {
        Debug.Log("Exited POI");
        if (CurrentPOIINFO != null)
        {
            CurrentPOIINFO.text = "No POI Selected";
        }
    }
    public IEnumerator UpdateMapRoutine()
    {
        // suspend execution for 5 seconds
        while (true)
        {
            yield return new WaitForSeconds(5);
            UpdatePlayerMap();
        }
    }

}
