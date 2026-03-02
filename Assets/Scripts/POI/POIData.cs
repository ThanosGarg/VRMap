using System;
using UnityEngine;
/// <summary>
/// Serializable data class for a Point of Interest (POI).
/// </summary>
[Serializable]
public class POIData
{
    /// <summary>
    /// Name of the POI.
    /// </summary>
    public string poiName;
    /// <summary>
    /// Type/category of the POI.
    /// </summary>
    public POIType poiType;
    /// <summary>
    /// Address of the POI (for geocoding).
    /// </summary>
    public string address;
    /// <summary>
    /// Latitude coordinate.
    /// </summary>
    public float latitude = 0f;
    /// <summary>
    /// Longitude coordinate.
    /// </summary>
    public float longitude = 0f;
    /// <summary>
    /// Height above ground level.
    /// </summary>
    public float height = 0f;
}
