using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Globalization;
using System.Threading.Tasks; // added

/// <summary>
/// Handles geocoding of addresses to latitude/longitude using the Nominatim (OpenStreetMap) API.
/// Supports both coroutine-based and async/await workflows. Respects Nominatim usage policy.
/// </summary>
public class GeoCodeAddress : MonoBehaviour
{
    // --- Inspector fields ---

    [Header("Nominatim (OpenStreetMap)")]
    [Tooltip("Nominatim requires a valid contact in the User-Agent per usage policy.")]
    [SerializeField] private string contactEmail = "replace-with-your-email@example.com";

    [Tooltip("Minimum interval (seconds) between requests to Nominatim.")]
    [SerializeField] private float minRequestIntervalSeconds = 1.0f;

    [Header("Demo")]
    [Tooltip("Address to geocode on start for demo purposes.")]
    [SerializeField] private string demoAddress = "";

    [Tooltip("If true, runs a geocode demo on Start.")]
    [SerializeField] private bool runDemoOnStart = false;

    /// <summary>
    /// Tracks the time of the last Nominatim request (static for all instances).
    /// </summary>
    private static float s_lastRequestTime;

    /// <summary>
    /// Unity Start method. Optionally runs a geocode demo if enabled in the inspector.
    /// </summary>
    void Start()
    {
        if (runDemoOnStart && !string.IsNullOrWhiteSpace(demoAddress))
        {
            // Run a demo geocode and log the result
            Geocode(demoAddress, (lat, lon) => Debug.Log($"Geocode success: {lat}, {lon}"), err => Debug.LogError($"Geocode error: {err}"));
        }
    }

    /// <summary>
    /// Public API: Geocode an address using a coroutine. Results are returned via callbacks.
    /// </summary>
    /// <param name="address">The address to geocode.</param>
    /// <param name="onSuccess">Callback invoked with (lat, lon) on success.</param>
    /// <param name="onError">Callback invoked with error message on failure.</param>
    public void Geocode(string address, Action<double, double> onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(GeocodeCoroutine(address, onSuccess, onError));
    }

    /// <summary>
    /// Coroutine to perform the geocoding request and parse the result.
    /// </summary>
    /// <param name="address">The address to geocode.</param>
    /// <param name="onSuccess">Callback invoked with (lat, lon) on success.</param>
    /// <param name="onError">Callback invoked with error message on failure.</param>
    private IEnumerator GeocodeCoroutine(string address, Action<double, double> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            onError?.Invoke("Address is empty.");
            yield break;
        }

        // Respect Nominatim usage policy: 1 request/sec
        var wait = s_lastRequestTime + minRequestIntervalSeconds - Time.realtimeSinceStartup;
        if (wait > 0f) yield return new WaitForSeconds(wait);

        // Build the Nominatim search URL
        var url = "https://nominatim.openstreetmap.org/search?format=json&limit=1&q=" + UnityWebRequest.EscapeURL(address);

        using (var req = UnityWebRequest.Get(url))
        {
            // Identify your app; include a contact per policy
            req.SetRequestHeader("User-Agent", $"VRMap/1.0 ({contactEmail})");
            req.SetRequestHeader("Accept-Language", "en");

            yield return req.SendWebRequest();
            s_lastRequestTime = Time.realtimeSinceStartup;

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"HTTP error: {req.error}");
                yield break;
            }

            var json = req.downloadHandler.text;
            if (string.IsNullOrEmpty(json) || json == "[]")
            {
                onError?.Invoke("No results found.");
                yield break;
            }

            // JsonUtility can't parse a top-level array; wrap it in an object
            var wrapped = "{\"items\":" + json + "}";
            NominatimResponse parsed = null;
            try
            {
                parsed = JsonUtility.FromJson<NominatimResponse>(wrapped);
            }
            catch (Exception ex)
            {
                onError?.Invoke($"JSON parse error: {ex.Message}");
                yield break;
            }

            if (parsed?.items == null || parsed.items.Length == 0)
            {
                onError?.Invoke("No results found.");
                yield break;
            }

            // Parse coordinates from the first result
            if (!double.TryParse(parsed.items[0].lat, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) ||
                !double.TryParse(parsed.items[0].lon, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
            {
                onError?.Invoke("Invalid coordinates in response.");
                yield break;
            }

            onSuccess?.Invoke(lat, lon);
        }
    }

    /// <summary>
    /// Static async method to geocode an address and return coordinates as a tuple.
    /// </summary>
    /// <param name="address">The address to geocode.</param>
    /// <param name="contactEmail">Contact email for Nominatim usage policy.</param>
    /// <param name="minIntervalSeconds">Minimum interval between requests.</param>
    /// <returns>Tuple of (latitude, longitude).</returns>
    public static async Task<(double lat, double lon)> GeocodeAsync(
        string address,
        string contactEmail = "replace-with-your-email@example.com",
        float minIntervalSeconds = 1f)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address is empty.", nameof(address));

        // Respect Nominatim usage policy: ≥1 req/sec
        var wait = s_lastRequestTime + minIntervalSeconds - Time.realtimeSinceStartup;
        if (wait > 0f) await Task.Delay(TimeSpan.FromSeconds(wait));

        // Build the Nominatim search URL
        var url = "https://nominatim.openstreetmap.org/search?format=json&limit=1&q=" + UnityWebRequest.EscapeURL(address);

        using (var req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("User-Agent", $"VRMap/1.0 ({contactEmail})");
            req.SetRequestHeader("Accept-Language", "en");

            // Await the web request
            var op = req.SendWebRequest();
            while (!op.isDone) await Task.Yield();
            s_lastRequestTime = Time.realtimeSinceStartup;

            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception($"HTTP error: {req.error}");

            var json = req.downloadHandler.text;
            if (string.IsNullOrEmpty(json) || json == "[]")
                throw new Exception("No results found.");

            // JsonUtility can't parse a top-level array; wrap it in an object
            var wrapped = "{\"items\":" + json + "}";
            NominatimResponse parsed;
            try
            {
                parsed = JsonUtility.FromJson<NominatimResponse>(wrapped);
            }
            catch (Exception ex)
            {
                throw new Exception($"JSON parse error: {ex.Message}");
            }

            if (parsed?.items == null || parsed.items.Length == 0)
                throw new Exception("No results found.");

            // Parse coordinates from the first result
            if (!double.TryParse(parsed.items[0].lat, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) ||
                !double.TryParse(parsed.items[0].lon, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                throw new Exception("Invalid coordinates in response.");

            return (lat, lon);
        }
    }

    /// <summary>
    /// Represents a single Nominatim geocoding result item.
    /// </summary>
    [Serializable]
    private class NominatimItem
    {
        /// <summary>Latitude as a string.</summary>
        public string lat;
        /// <summary>Longitude as a string.</summary>
        public string lon;
        /// <summary>Display name of the result.</summary>
        public string display_name;
    }

    /// <summary>
    /// Wrapper for parsing a Nominatim response as an array of items.
    /// </summary>
    [Serializable]
    private class NominatimResponse
    {
        public NominatimItem[] items;
    }
}
