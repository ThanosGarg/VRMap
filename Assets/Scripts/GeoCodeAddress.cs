using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Globalization;
using System.Threading.Tasks; // added

public class GeoCodeAddress : MonoBehaviour
{
    // Nominatim (OpenStreetMap) settings
    [Header("Nominatim (OpenStreetMap)")]
    [Tooltip("Nominatim requires a valid contact in the User-Agent per usage policy.")]
    [SerializeField] private string contactEmail = "replace-with-your-email@example.com";
    [SerializeField] private float minRequestIntervalSeconds = 1.0f;

    // Optional demo
    [Header("Demo")]
    [SerializeField] private string demoAddress = "";
    [SerializeField] private bool runDemoOnStart = false;

    private static float s_lastRequestTime;

    // Start: optionally run demo
    void Start()
    {
        if (runDemoOnStart && !string.IsNullOrWhiteSpace(demoAddress))
        {
            Geocode(demoAddress, (lat, lon) => Debug.Log($"Geocode success: {lat}, {lon}"), err => Debug.LogError($"Geocode error: {err}"));
        }
    }



    // Public API: call with an address, receive lat/lon via callbacks
    public void Geocode(string address, Action<double, double> onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(GeocodeCoroutine(address, onSuccess, onError));
    }

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

            // JsonUtility can't parse a top-level array; wrap it
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

            if (!double.TryParse(parsed.items[0].lat, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) ||
                !double.TryParse(parsed.items[0].lon, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
            {
                onError?.Invoke("Invalid coordinates in response.");
                yield break;
            }

            onSuccess?.Invoke(lat, lon);
        }
    }

    // Simple one-call method: await and get (lat, lon)
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

        var url = "https://nominatim.openstreetmap.org/search?format=json&limit=1&q=" + UnityWebRequest.EscapeURL(address);

        using (var req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("User-Agent", $"VRMap/1.0 ({contactEmail})");
            req.SetRequestHeader("Accept-Language", "en");

            var op = req.SendWebRequest();
            while (!op.isDone) await Task.Yield();
            s_lastRequestTime = Time.realtimeSinceStartup;

            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception($"HTTP error: {req.error}");

            var json = req.downloadHandler.text;
            if (string.IsNullOrEmpty(json) || json == "[]")
                throw new Exception("No results found.");

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

            if (!double.TryParse(parsed.items[0].lat, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) ||
                !double.TryParse(parsed.items[0].lon, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                throw new Exception("Invalid coordinates in response.");

            return (lat, lon);
        }
    }

    [Serializable]
    private class NominatimItem
    {
        public string lat;
        public string lon;
        public string display_name;
    }

    [Serializable]
    private class NominatimResponse
    {
        public NominatimItem[] items;
    }
}
