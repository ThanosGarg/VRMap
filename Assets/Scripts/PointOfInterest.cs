using CesiumForUnity;
using UnityEngine;

[RequireComponent(typeof(CesiumGlobeAnchor))]
public class PointOfInterest : MonoBehaviour
{
    public float latitude = 0f;
    public float longitude = 0f;

    private CesiumGlobeAnchor globeAnchor;

    void Start()
    {
        globeAnchor = GetComponent<CesiumGlobeAnchor>();
        //  globeAnchor.SetLocation(latitude, longitude, 0);
    }
}
