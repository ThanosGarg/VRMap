using CesiumForUnity;
using System;
using System.Collections.Generic;
using UnityEngine;


public class TourManager : MonoBehaviour
{

    public string address = "1600 Amphitheatre Parkway, Mountain View, CA";
    public float lat = 0f;
    public float lon = 0f;
    public PointOfInterest poiPrefab;
    public CesiumGeoreference georeference;
    public POIDB poiDatabase;
    public List<POIIcon> Icons = new List<POIIcon>();

    public Action<double, double> onGeocodeSuccess;

    public Action<string> onGeocodeError;

    void OnEnable()
    {
        onGeocodeSuccess += OnGeocodeSuccess;
        onGeocodeError += OnGeocodeError;
    }

    void OnDisable()
    {
        onGeocodeSuccess -= OnGeocodeSuccess;
        onGeocodeError -= OnGeocodeError;
    }

    void Start()
    {
        foreach (POIData data in poiDatabase.poiList)
        {
            SpawnPOI(data);
        }

         AppManager.instance.geoCoder.Geocode(address, onGeocodeSuccess, onGeocodeError);
    }

    private void OnGeocodeSuccess(double latitude, double longitude)
    {
        lat = (float)latitude;
        lon = (float)longitude;
        georeference.SetOriginLongitudeLatitudeHeight(lon, lat, 150f);
        onGeocodeSuccess -= OnGeocodeSuccess;
        onGeocodeError -= OnGeocodeError;
    }

    private void OnGeocodeError(string error)
    {
        Debug.Log($"Geocode error: {error}");

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