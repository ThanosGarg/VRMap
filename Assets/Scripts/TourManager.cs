using CesiumForUnity;
using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages the tour logic, including player location, POI spawning, and georeferencing.
/// </summary>
public class TourManager : MonoBehaviour
{
    /// <summary>
    /// Current latitude of the player or tour origin.
    /// </summary>
    public float lat = 0f;
    /// <summary>
    /// Current longitude of the player or tour origin.
    /// </summary>
    public float lon = 0f;
    /// <summary>
    /// Current altitude of the player or tour origin.
    /// </summary>
    public float altitude = 150f;
    /// <summary>
    /// Prefab used to instantiate points of interest (POIs).
    /// </summary>
    public PointOfInterest poiPrefab;
    /// <summary>
    /// Reference to the CesiumGeoreference component for geospatial origin management.
    /// </summary>
    public CesiumGeoreference georeference;
    /// <summary>
    /// Database of POI data objects.
    /// </summary>
    public POIDB poiDatabase;
    /// <summary>
    /// List of POI icons used for visual representation.
    /// </summary>
    public List<POIIcon> Icons = new List<POIIcon>();

    /// <summary>
    /// Unity Start method. Spawns all POIs from the database and sets the georeference origin.
    /// </summary>
    void Start()
    {
        // Spawn all POIs defined in the database
        foreach (POIData data in poiDatabase.poiList)
        {
            SpawnPOI(data);
        }

        // Set the Cesium georeference origin to the current lat/lon/altitude
        georeference.SetOriginLongitudeLatitudeHeight(lon, lat, altitude);
    }

    /// <summary>
    /// Updates the player's location and moves the Cesium georeference origin.
    /// </summary>
    /// <param name="newLat">New latitude</param>
    /// <param name="newLon">New longitude</param>
    /// <param name="newAltitude">New altitude</param>
    public void UpdatePlayerLocation(float newLat, float newLon, float newAltitude)
    {
        lat = newLat;
        lon = newLon;
        altitude = newAltitude;

        // Update the Cesium georeference origin
        georeference.SetOriginLongitudeLatitudeHeight(lon, lat, altitude);
    }

    /// <summary>
    /// Instantiates a POI prefab and initializes it with the given data.
    /// </summary>
    /// <param name="data">POI data to initialize the POI</param>
    public void SpawnPOI(POIData data)
    {
        // Instantiate the POI prefab as a child of the georeference
        PointOfInterest poi = Instantiate(poiPrefab, georeference.transform);
        // Initialize the POI with its data and a reference to this manager
        poi.Init(data, this);
    }
}

/// <summary>
/// Types of points of interest (POIs) supported by the application.
/// </summary>
public enum POIType
{
    /// <summary>Sightseeing location</summary>
    SeightSeeing,
    /// <summary>Hospital</summary>
    Hospital,
    /// <summary>Embassy</summary>
    Embassy,
    /// <summary>Bus stop</summary>
    BusStop,
    /// <summary>Museum</summary>
    Museum,
    /// <summary>Metro station</summary>
    MetroStation,
    /// <summary>Automated Teller Machine</summary>
    ATM,




}