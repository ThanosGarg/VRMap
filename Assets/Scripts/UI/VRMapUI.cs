using TMPro;
using UnityEngine;

public class VRMapUI : VRPlayerUI
{

public TextMeshProUGUI CurrentPOIINFO;
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
    void UpdatePOIINFO(POIData currentPOI)
    {
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
        if (CurrentPOIINFO != null)
        {
            CurrentPOIINFO.text = "No POI Selected";
        }
    }
     
}
 