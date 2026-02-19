using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class KeepOnlyXrUiInputModule : MonoBehaviour
{
    void Awake()
    {
        var es = FindObjectOfType<EventSystem>();
        if (!es) return;

        // κρατάμε μόνο XRUIInputModule
        var xr = es.GetComponent<XRUIInputModule>();
        if (!xr) xr = es.gameObject.AddComponent<XRUIInputModule>();

        // σβήνουμε όλα τα άλλα BaseInputModule
        var modules = es.GetComponents<BaseInputModule>();
        foreach (var m in modules)
        {
            if (m is XRUIInputModule) continue;
            Destroy(m);
        }
    }
}
