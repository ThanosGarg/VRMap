using CesiumForUnity;
using UnityEngine;
using Unity.Mathematics; // added

[RequireComponent(typeof(CesiumGlobeAnchor))]
public class PointOfInterest : MonoBehaviour
{
    public float latitude = 0f;
    public float longitude = 0f;
    public float height = 0f;

    private CesiumGlobeAnchor globeAnchor;

    void Start()
    {
        globeAnchor = GetComponent<CesiumGlobeAnchor>();
        globeAnchor.longitudeLatitudeHeight = new double3(longitude, latitude, height);
    }
}
