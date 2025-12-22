using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class TrafficManager : MonoBehaviour
{
    [Header("Geoapify Routing - free API key required")]
    [SerializeField] private string geoapifyApiKey = "";

    [Header("Route settings")]
    [SerializeField] private double startLat = 50.96209827745463;
    [SerializeField] private double startLon = 4.414458883409225;
    [SerializeField] private double endLat = 50.429137079078345;
    [SerializeField] private double endLon = 5.00088081232559;
    [SerializeField] private string travelMode = "drive"; // drive|bicycle|walk

    public List<Disruption> LatestDisruptions = new List<Disruption>();
    public event Action<List<Disruption>> OnTrafficDataUpdated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start periodic polling of the traffic API
        StopAllCoroutines();
        StartCoroutine(FetchOnce());
    }

    // Update is called once per frame
    void Update() { }

    // Trigger a manual refresh outside the poll cycle
    public void RefreshNow()
    {
        StartCoroutine(FetchOnce());
    }

    // Build Geoapify Routing URL (lat,lon|lat,lon)
    private string BuildGeoapifyRoutingUrl()
    {
        if (string.IsNullOrEmpty(geoapifyApiKey))
        {
            Debug.LogWarning("Geoapify API key is missing.");
            return null;
        }

        // If needed, override with sample coords
        var sLat = startLat.ToString("G17", CultureInfo.InvariantCulture);
        var sLon = startLon.ToString("G17", CultureInfo.InvariantCulture);
        var eLat = endLat.ToString("G17", CultureInfo.InvariantCulture);
        var eLon = endLon.ToString("G17", CultureInfo.InvariantCulture);
        var mode = string.IsNullOrEmpty(travelMode) ? "drive" : travelMode;

        var waypoints = $"{sLat},{sLon}|{eLat},{eLon}"; // URL encoding of '|' handled by UnityWebRequest

        var url = $"https://api.geoapify.com/v1/routing?waypoints={waypoints}&mode={mode}&apiKey={geoapifyApiKey}";
        return url;
    }

    private IEnumerator FetchOnce()
    {
        var url = BuildGeoapifyRoutingUrl();
        if (string.IsNullOrEmpty(url)) yield break;

        Debug.Log($"Geoapify Routing: {url}");
        using (var req = UnityWebRequest.Get(url))
        {
            req.timeout = 10;
            req.SetRequestHeader("Accept", "application/json");
            yield return req.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
            var ok = req.result == UnityWebRequest.Result.Success;
#else
            var ok = !(req.isNetworkError || req.isHttpError);
#endif
            if (!ok)
            {
                Debug.LogWarning($"Routing fetch failed: {req.error} (HTTP {req.responseCode})");
                yield break;
            }

            var json = req.downloadHandler.text;
            // Debug log a small snippet for validation
            Debug.Log($"Geoapify Routing response: {json.Substring(0, Math.Min(256, json.Length))}");

            try
            {
                var root = JsonUtility.FromJson<GeoapifyRoutingRoot>(json);
                if (root?.features != null && root.features.Length > 0)
                {
                    var props = root.features[0]?.properties;
                    var distanceM = props?.distance ?? 0f;
                    var timeSec = props?.time ?? 0f;
                    var mode = props?.mode ?? travelMode;

                    var comments = $"Mode: {mode}, Distance: {distanceM:F0} m, Time: {timeSec:F0} s";
                    var fromTo = $"{startLat:F6},{startLon:F6} -> {endLat:F6},{endLon:F6}";

                    LatestDisruptions = new List<Disruption>
                    {
                        new Disruption
                        {
                            id = $"route-{DateTime.UtcNow:yyyyMMddHHmmss}",
                            category = "Route",
                            subCategory = mode,
                            severity = null,
                            comments = $"{fromTo} | {comments}",
                            startDateTime = null,
                            endDateTime = null,
                            roadName = null,
                            corridorIds = null,
                            isProvisional = false,
                            closureText = null,
                            disruptionGeometry = null // geometry omitted
                        }
                    };

                    OnTrafficDataUpdated?.Invoke(LatestDisruptions);
                    Debug.Log("Geoapify route parsed and emitted as Disruption.");
                }
                else
                {
                    Debug.LogWarning("Geoapify Routing: empty features.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Routing parse failed: {ex.Message}");
            }
        }
    }

    // Geoapify Routing DTOs (minimal)
    [Serializable] private class GeoapifyRoutingRoot { public GeoapifyRoutingFeature[] features; }
    [Serializable] private class GeoapifyRoutingFeature { public string type; public GeoapifyRoutingProps properties; }
    [Serializable]
    private class GeoapifyRoutingProps
    {
        public float distance; // meters
        public float time;     // seconds
        public string mode;    // drive|bicycle|walk
    }

    // Normalized model used to emit route summary
    [Serializable]
    public class Disruption
    {
        public string id;
        public string category;
        public string subCategory;
        public string severity;
        public string comments;
        public string currentUpdate;
        public string startDateTime;
        public string endDateTime;
        public string roadName;
        public string corridorIds;
        public bool isProvisional;
        public string closureText;
        public string disruptionGeometry;
    }
}
