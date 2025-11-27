using CesiumForUnity;
using JetBrains.Annotations;
using UnityEngine;

public class TourManager : MonoBehaviour
{
    public float lat = 0f;
    public float lon = 0f;
    public PointOfInterest poiPrefab;
    public CesiumGeoreference georeference;
    public POIDB poiDatabase;

    void Start()
    {
        foreach (POIData data in poiDatabase.poiList)
        {
            SpawnPOI(data);
        }


    }
    public void SpawnPOI(POIData data)
    {
        PointOfInterest poi = Instantiate(poiPrefab, georeference.transform);
        poi.Init(data);
    }
}
