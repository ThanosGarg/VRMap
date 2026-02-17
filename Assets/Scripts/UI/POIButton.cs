using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class POIButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

    public POIData poiData;
    private TourManager tourManager;
    public Button buttonComponent;
    public void Init(POIData data, TourManager manager)
    {
        poiData = data;
        tourManager = manager;
        if (buttonText != null)
        {
            buttonText.text = data.poiName;
        }
        buttonComponent = GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(OnButtonClicked);
        }
    }
    public void OnButtonClicked()
    {
        if (tourManager != null && poiData != null)
        {
            tourManager.UpdatePlayerLocation(poiData.latitude, poiData.longitude);
            AppManager.instance.VRMapUI.UpdatePOIINFO(poiData);
            AppManager.instance.VRMapUI.mapViewer.UpdateCoordinates(poiData.latitude, poiData.longitude, 15);
        }
    }

}
