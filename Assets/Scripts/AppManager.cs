using UnityEngine;

public class AppManager : MonoBehaviour
{
    public Player player;
    public static AppManager instance;
    public GeoCodeAddress geoCoder;
    public VRMapUI vrMapUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
