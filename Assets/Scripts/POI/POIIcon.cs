using System;
using UnityEngine;
/// <summary>
/// Represents an icon associated with a specific POI type for use in the VRMap UI.
/// </summary>
[Serializable]
public class POIIcon
{
    /// <summary>
    /// The type of Point of Interest this icon represents.
    /// </summary>
    public POIType type;

    /// <summary>
    /// The sprite image to use for this POI type.
    /// </summary>
    public Sprite icon;
}
