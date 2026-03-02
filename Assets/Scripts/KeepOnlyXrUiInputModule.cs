using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;

/// <summary>
/// Ensures that only the XRUIInputModule is attached to the EventSystem.
/// Removes all other BaseInputModule components at runtime.
/// </summary>
public class KeepOnlyXrUiInputModule : MonoBehaviour
{
    /// <summary>
    /// On Awake, keeps only the XRUIInputModule on the EventSystem and removes all other input modules.
    /// </summary>
    void Awake()
    {
        // Find the first EventSystem in the scene
        var es = FindObjectOfType<EventSystem>();
        if (!es) return;

        // Keep only XRUIInputModule (add if missing)
        var xr = es.GetComponent<XRUIInputModule>();
        if (!xr) xr = es.gameObject.AddComponent<XRUIInputModule>();

        // Remove all other BaseInputModule components except XRUIInputModule
        var modules = es.GetComponents<BaseInputModule>();
        foreach (var m in modules)
        {
            // Skip the XRUIInputModule itself
            if (m is XRUIInputModule) continue;
            // Destroy any other input module
            Destroy(m);
        }
    }
}
