using CesiumForUnity;
using System;
using System.Collections.Generic;
using UnityEngine;


public class TourManager : MonoBehaviour
{

    public float lat = 0f;
    public float lon = 0f;
    public PointOfInterest poiPrefab;
    public CesiumGeoreference georeference;
    public POIDB poiDatabase;
    public List<POIIcon> Icons = new List<POIIcon>();






    void Start()
    {
        foreach (POIData data in poiDatabase.poiList)
        {
            SpawnPOI(data);
        }

        georeference.SetOriginLongitudeLatitudeHeight(lon, lat, 150f);
    }
    public void UpdatePlayerLocation(float newLat, float newLon)
    {
        lat = newLat;
        lon = newLon;
        georeference.SetOriginLongitudeLatitudeHeight(lon, lat, 150f);
    }




    public void SpawnPOI(POIData data)
    {
        PointOfInterest poi = Instantiate(poiPrefab, georeference.transform);
        poi.Init(data, this);
    }
}

public enum POIType
{
    SeightSeeing,
    Hospital,
    Embassy,
    BusStop,
    Museum,
    MetroStation,
    ATM,


}