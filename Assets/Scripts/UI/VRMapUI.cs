using UnityEngine;
using TMPro;

public class VRMapUI : VRPlayerUI
{
    public POIData currentPOI;
    public TextMeshProUGUI poiNameAddressText;

    private void Start()
    {
        AppManager.instance.player.POIEntered += OnPOIEntered;
        AppManager.instance.player.POIExited += OnPOIExited;
    }

    private void OnPOIEntered(POIData poiData)
    {
        currentPOI = poiData;
        poiNameAddressText.text = $"Current POI: {currentPOI.poiName} {currentPOI.address}";
    }

    private void OnPOIExited()
    {
        currentPOI = null;
        poiNameAddressText.text = "Current POI: ---";

    }
}