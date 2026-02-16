using UnityEngine;

public class AppManager : MonoBehaviour
{
    public Transform player;
    public static AppManager instance;

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
