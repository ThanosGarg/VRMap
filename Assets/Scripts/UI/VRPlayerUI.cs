using TMPro;
using UnityEngine;


/// <summary>
/// Handles the VR player's UI panel, making it follow a specified hand GameObject with configurable offsets and smoothing.
/// Provides functionality to toggle the UI's visibility.
/// </summary>
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
    /// Called once per frame. Updates the UI's position and rotation to follow the hand with smoothing and offsets.
    /// </summary>
    void Update()
    {
        // If no hand is assigned, do nothing
        if (handToFollow == null) return;

        // Calculate the target position by applying the position offset in the hand's local space
        Vector3 targetPosition = handToFollow.transform.position + handToFollow.transform.rotation * positionOffset;

        // Calculate the target rotation by applying the rotation offset to the hand's rotation
        Quaternion targetRotation = handToFollow.transform.rotation * Quaternion.Euler(rotationOffset);

        // Smoothly interpolate the UI's position towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, positionSmooth);

        // Smoothly interpolate the UI's rotation towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmooth);
    }

    /// <summary>
    /// Toggles the UI's active state (on/off).
    /// </summary>
    public void ToggleUI()
    {
        // Set the GameObject's active state to the opposite of its current state
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
