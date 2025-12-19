using UnityEngine;

public class Player : MonoBehaviour
{
    public PointOfInterest currentPOI;

    private void OnTriggerEnter(Collider other)
    {
        PointOfInterest poi = other.GetComponent<PointOfInterest>();
        if (poi != null)
        {
            currentPOI = poi;
            Debug.Log("Entered POI: " + poi.poiData.address);
        }
    }
}
