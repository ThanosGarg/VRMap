using System;
using UnityEngine;
[Serializable]

public class POIData
{
    public POIIconType iconType;
    public string poiName;
    public string address;
    public float latitude = 0f;
    public float longitude = 0f;
    public float height = 0f;
}

public enum POIIconType
{
    SightSeen,
    Hotel,
    Museum,
    Park,
    Hospital,
    Embassy,
    BusStation,
    MetroStation,
    ATM
}
