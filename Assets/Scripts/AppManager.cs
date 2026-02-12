using UnityEngine;

public class AppManager : MonoBehaviour
{
    public Player player;
    public static AppManager instance;
    public GeoCodeAddress geoCoder;

    public TourManager tourManager;
    public VRMapUI VRMapUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
