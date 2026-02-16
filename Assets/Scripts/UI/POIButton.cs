using TMPro;
using UnityEngine;

public class POIButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

    public POIData poiData;
    public void Init(POIData data)
    {
        poiData = data;
        if (buttonText != null)
        {
            buttonText.text = data.poiName;
        }
    }

}
