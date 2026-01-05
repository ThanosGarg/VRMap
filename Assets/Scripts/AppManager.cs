using UnityEngine;

public class AppManager : MonoBehaviour
{
    public Player player;
    public static AppManager instance;
    public GeoCodeAddress geoCoder;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
