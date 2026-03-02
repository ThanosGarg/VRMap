using UnityEngine;

/// <summary>
/// Singleton entry point for the VRMap application.
/// Provides global access to key managers and the player transform.
/// </summary>
public class AppManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the player transform in the scene.
    /// </summary>
    public Transform player;

    /// <summary>
    /// Singleton instance of the AppManager.
    /// </summary>
    public static AppManager instance;

    /// <summary>
    /// Reference to the TourManager, which handles POI tours and spawning.
    /// </summary>
    public TourManager tourManager;

    /// <summary>
    /// Reference to the main VRMap UI manager.
    /// </summary>
    public VRMapUI VRMapUI;

    /// <summary>
    /// Unity Awake method. Initializes the singleton instance.
    /// </summary>
    private void Awake()
    {
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
        }
    }
}
