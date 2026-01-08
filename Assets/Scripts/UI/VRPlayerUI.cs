using TMPro;
using UnityEngine;


public class VRPlayerUI : MonoBehaviour
{
    /// <summary>
    /// The hand GameObject that the UI should follow.
    /// </summary>
    public GameObject handToFollow;

    

    [Header("UI Offsets")]
    /// <summary>
    /// Offset to apply to the UI's position relative to the hand.
    /// </summary>
    public Vector3 positionOffset = Vector3.zero;
    /// <summary>
    /// Offset to apply to the UI's rotation relative to the hand.
    /// </summary>
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Smoothing")]
    /// <summary>
    /// Smoothing factor for position interpolation (0 = no smoothing, 1 = instant).
    /// </summary>
    [Range(0f, 1f)]
    public float positionSmooth = 0.1f;
    /// <summary>
    /// Smoothing factor for rotation interpolation (0 = no smoothing, 1 = instant).
    /// </summary>
    [Range(0f, 1f)]
    public float rotationSmooth = 0.1f;

    

    /// <summary>
    /// Called once per frame. Updates the UI's position and rotation to follow the hand.
    /// </summary>
    void Update()
    {
        if (handToFollow == null) return;

        // Calculate target position and rotation with offsets
        Vector3 targetPosition = handToFollow.transform.position + handToFollow.transform.rotation * positionOffset;
        Quaternion targetRotation = handToFollow.transform.rotation * Quaternion.Euler(rotationOffset);

        // Smoothly interpolate position and rotation
        transform.position = Vector3.Lerp(transform.position, targetPosition, positionSmooth);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmooth);
    }

    /// <summary>
    /// Toggles the UI on or off.
    /// </summary>
    public void ToggleUI()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    /// <summary>
    /// Returns whether the UI is currently visible.
    /// </summary>
    public bool IsVisible
    {
        get { return gameObject.activeSelf; }
    }

   
    
}
