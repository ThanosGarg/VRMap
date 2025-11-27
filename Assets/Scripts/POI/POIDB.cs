using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "POIDB", menuName = "Create POIDB")]

public class POIDB : ScriptableObject
{
    public List<POIData> poiList = new List<POIData>();
}
