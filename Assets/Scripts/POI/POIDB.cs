using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ScriptableObject database for storing a list of Point of Interest (POI) data objects.
/// Used to manage and persist POIs in the Unity Editor.
/// </summary>
[CreateAssetMenu(fileName = "POIDB", menuName = "Create POIDB")]
public class POIDB : ScriptableObject
{
    /// <summary>
    /// List of all POI data entries in the database.
    /// </summary>
    public List<POIData> poiList = new List<POIData>();
}
