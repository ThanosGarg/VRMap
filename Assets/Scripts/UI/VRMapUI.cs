using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class VRMapUI : VRPlayerUI
{

    public OpenStreetMapViewer mapViewer; // Assign in Inspector
    public TextMeshProUGUI CurrentPOIINFO;
    public int zoomLevel = 10;
    public POIButton poiButtonPrefab;
    public Transform poiArea;
    void Start()
    {
        SpawnPOIButtons();
    }

    private void SpawnPOIButtons()
    {
        foreach (POIData data in AppManager.instance.tourManager.poiDatabase.poiList)
        {
            POIButton button = Instantiate(poiButtonPrefab, poiArea);
            button.Init(data);
        }
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
