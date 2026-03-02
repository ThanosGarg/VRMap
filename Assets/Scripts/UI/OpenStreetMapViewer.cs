using System.Collections;
using UnityEngine;

using UnityEngine.UI;

/// <summary>
/// Displays a static OpenStreetMap tile in a Unity UI Image based on latitude, longitude, and zoom level.
/// </summary>
public class OpenStreetMapViewer : MonoBehaviour
{
    /// <summary>
    /// Latitude of the map center (WGS84).
    /// </summary>
    public float latitude = 40.7128f; // Example: New York City

    /// <summary>
    /// Longitude of the map center (WGS84).
    /// </summary>
    public float longitude = -74.0060f;

    /// <summary>
    /// Zoom level for the map tile (0 = world, higher = more detail).
    /// </summary>
    public int zoom;

    /// <summary>
    /// UI Image component where the map tile will be displayed. Assign in Inspector.
    /// </summary>
    public Image targetImage; // Assign a UI Image in the Inspector

    /// <summary>
    /// Updates the map center coordinates and zoom, then refreshes the displayed tile.
    /// </summary>
    /// <param name="lat">Latitude in degrees.</param>
    /// <param name="lon">Longitude in degrees.</param>
    /// <param name="newZoom">Zoom level (integer, 0+).</param>
    public void UpdateCoordinates(float lat, float lon, int newZoom)
    {
        latitude = lat;
        longitude = lon;
        zoom = newZoom;
        // Start coroutine to download and display the new map tile
        StartCoroutine(DownloadAndDisplayTile());
    }

    /// <summary>
    /// Coroutine that downloads the appropriate OSM tile and displays it in the UI Image.
    /// </summary>
    private IEnumerator DownloadAndDisplayTile()
    {
        int tileX, tileY;
        // Convert lat/lon/zoom to OSM tile X/Y coordinates
        LatLonToTileXY(latitude, longitude, zoom, out tileX, out tileY);
        string url = $"https://tile.openstreetmap.org/{zoom}/{tileX}/{tileY}.png";

        // Create a UnityWebRequest to download the tile as a texture
        using (var www = new UnityEngine.Networking.UnityWebRequest(url))
        {
            www.downloadHandler = new UnityEngine.Networking.DownloadHandlerTexture();
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                // Get the downloaded texture
                var tex = ((UnityEngine.Networking.DownloadHandlerTexture)www.downloadHandler).texture;
                if (targetImage != null)
                {
                    // Create a Sprite from the downloaded texture and assign it to the UI Image
                    Rect rect = new Rect(0, 0, tex.width, tex.height);
                    Vector2 pivot = new Vector2(0.5f, 0.5f);
                    Sprite sprite = Sprite.Create(tex, rect, pivot);
                    targetImage.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning("No targetImage assigned. Please assign a UI Image in the Inspector.");
                }
            }
            else
            {
                Debug.LogError($"Failed to download tile: {www.error}");
            }
        }
    }

    /// <summary>
    /// Converts latitude/longitude to OpenStreetMap tile X/Y coordinates for a given zoom level.
    /// </summary>
    /// <param name="lat">Latitude in degrees.</param>
    /// <param name="lon">Longitude in degrees.</param>
    /// <param name="zoom">Zoom level.</param>
    /// <param name="tileX">Output tile X coordinate.</param>
    /// <param name="tileY">Output tile Y coordinate.</param>
    private void LatLonToTileXY(float lat, float lon, int zoom, out int tileX, out int tileY)
    {
        // Convert latitude to radians
        double latRad = lat * Mathf.Deg2Rad;
        // Number of tiles at this zoom level
        double n = Mathf.Pow(2, zoom);
        // Calculate tile X
        tileX = (int)((lon + 180.0) / 360.0 * n);
        // Calculate tile Y using Mercator projection
        tileY = (int)((1.0 - Mathf.Log((float)(Mathf.Tan((float)latRad) + 1.0 / Mathf.Cos((float)latRad))) / Mathf.PI) / 2.0 * n);
    }
}
