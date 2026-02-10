using System.Collections;
using UnityEngine;

using UnityEngine.UI;

public class OpenStreetMapViewer : MonoBehaviour
{
    // Map center coordinates
    public float latitude = 40.7128f; // Example: New York City
    public float longitude = -74.0060f;
    public int zoom = 15;
    public Image targetImage; // Assign a UI Image in the Inspector


    public void UpdateCoordinates(float lat, float lon, int newZoom)
    {
        latitude = lat;
        longitude = lon;
        zoom = newZoom;
        StartCoroutine(DownloadAndDisplayTile());
    }
    IEnumerator DownloadAndDisplayTile()
    {
        int tileX, tileY;
        LatLonToTileXY(latitude, longitude, zoom, out tileX, out tileY);
        string url = $"https://tile.openstreetmap.org/{zoom}/{tileX}/{tileY}.png";
        using (var www = new UnityEngine.Networking.UnityWebRequest(url))
        {
            www.downloadHandler = new UnityEngine.Networking.DownloadHandlerTexture();
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                var tex = ((UnityEngine.Networking.DownloadHandlerTexture)www.downloadHandler).texture;
                if (targetImage != null)
                {
                    // Create a Sprite from the downloaded texture
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

    // Converts latitude/longitude to OSM tile X/Y
    void LatLonToTileXY(float lat, float lon, int zoom, out int tileX, out int tileY)
    {
        double latRad = lat * Mathf.Deg2Rad;
        double n = Mathf.Pow(2, zoom);
        tileX = (int)((lon + 180.0) / 360.0 * n);
        tileY = (int)((1.0 - Mathf.Log((float)(Mathf.Tan((float)latRad) + 1.0 / Mathf.Cos((float)latRad))) / Mathf.PI) / 2.0 * n);
    }
}
