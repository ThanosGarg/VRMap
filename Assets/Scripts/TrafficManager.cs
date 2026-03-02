using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// Manages traffic routing and disruption data using the Geoapify Routing API.
/// Fetches route information and emits it as normalized disruption events.
/// </summary>
public class TrafficManager : MonoBehaviour
{
    [Header("Geoapify Routing - free API key required")]
    [Tooltip("Your Geoapify API key. Get one for free at https://myprojects.geoapify.com/")]
    [SerializeField] private string geoapifyApiKey = "";

    [Header("Route settings")]
    [Tooltip("Start latitude for the route.")]
    [SerializeField] private double startLat = 50.96209827745463;
    [Tooltip("Start longitude for the route.")]
    [SerializeField] private double startLon = 4.414458883409225;
    [Tooltip("End latitude for the route.")]
    [SerializeField] private double endLat = 50.429137079078345;
    [Tooltip("End longitude for the route.")]
    [SerializeField] private double endLon = 5.00088081232559;
    [Tooltip("Travel mode: drive, bicycle, or walk.")]
    [SerializeField] private string travelMode = "drive"; // drive|bicycle|walk

    /// <summary>
    /// The most recent list of disruptions (route summaries).
    /// </summary>
    public List<Disruption> LatestDisruptions = new List<Disruption>();

    /// <summary>
    /// Event triggered when new traffic data is available.
    /// </summary>
    public event Action<List<Disruption>> OnTrafficDataUpdated;

    /// <summary>
    /// Unity Start method. Begins polling the traffic API for route data.
    /// </summary>
    void Start()
    {
        // Stop any previous coroutines and start a new fetch
        StopAllCoroutines();
        StartCoroutine(FetchOnce());
    }

    /// <summary>
    /// Unity Update method. Not used in this class.
    /// </summary>
    void Update() { }

    /// <summary>
    /// Manually triggers a refresh of the traffic data.
    /// </summary>
    public void RefreshNow()
    {
        StartCoroutine(FetchOnce());
    }

    /// <summary>
    /// Constructs the Geoapify Routing API URL using the current settings.
    /// </summary>
    /// <returns>Routing API URL, or null if API key is missing.</returns>
    private string BuildGeoapifyRoutingUrl()
    {
        if (string.IsNullOrEmpty(geoapifyApiKey))
        {
            Debug.LogWarning("Geoapify API key is missing.");
            return null;
        }

        // Format coordinates with invariant culture for URL
        var sLat = startLat.ToString("G17", CultureInfo.InvariantCulture);
        var sLon = startLon.ToString("G17", CultureInfo.InvariantCulture);
        var eLat = endLat.ToString("G17", CultureInfo.InvariantCulture);
        var eLon = endLon.ToString("G17", CultureInfo.InvariantCulture);
        var mode = string.IsNullOrEmpty(travelMode) ? "drive" : travelMode;

        // Waypoints format: lat,lon|lat,lon
        var waypoints = $"{sLat},{sLon}|{eLat},{eLon}"; // URL encoding of '|' handled by UnityWebRequest

        var url = $"https://api.geoapify.com/v1/routing?waypoints={waypoints}&mode={mode}&apiKey={geoapifyApiKey}";
        return url;
    }

    /// <summary>
    /// Coroutine to fetch route data from Geoapify and emit as a disruption event.
    /// </summary>
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
            // Log a snippet of the response for debugging
            Debug.Log($"Geoapify Routing response: {json.Substring(0, Math.Min(256, json.Length))}");

            try
            {
                // Parse the Geoapify routing response
                var root = JsonUtility.FromJson<GeoapifyRoutingRoot>(json);
                if (root?.features != null && root.features.Length > 0)
                {
                    var props = root.features[0]?.properties;
                    var distanceM = props?.distance ?? 0f;
                    var timeSec = props?.time ?? 0f;
                    var mode = props?.mode ?? travelMode;

                    var comments = $"Mode: {mode}, Distance: {distanceM:F0} m, Time: {timeSec:F0} s";
                    var fromTo = $"{startLat:F6},{startLon:F6} -> {endLat:F6},{endLon:F6}";

                    // Create a new disruption summary for the route
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

                    // Notify listeners of the new data
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

    // --- DTOs for Geoapify Routing response ---

    /// <summary>
    /// Root object for Geoapify routing response.
    /// </summary>
    [Serializable]
    private class GeoapifyRoutingRoot
    {
        public GeoapifyRoutingFeature[] features;
    }

    /// <summary>
    /// Feature object in Geoapify routing response.
    /// </summary>
    [Serializable]
    private class GeoapifyRoutingFeature
    {
        public string type;
        public GeoapifyRoutingProps properties;
    }

    /// <summary>
    /// Properties of a Geoapify routing feature.
    /// </summary>
    [Serializable]
    private class GeoapifyRoutingProps
    {
        public float distance; // meters
        public float time;     // seconds
        public string mode;    // drive|bicycle|walk
    }

    /// <summary>
    /// Normalized model for emitting route summary as a disruption event.
    /// </summary>
    [Serializable]
    public class Disruption
    {
        /// <summary>Unique identifier for the disruption.</summary>
        public string id;
        /// <summary>General category (e.g., "Route").</summary>
        public string category;
        /// <summary>Subcategory (e.g., travel mode).</summary>
        public string subCategory;
        /// <summary>Severity of the disruption (not used for routes).</summary>
        public string severity;
        /// <summary>Summary comments about the disruption or route.</summary>
        public string comments;
        /// <summary>Current update text (not used for routes).</summary>
        public string currentUpdate;
        /// <summary>Start date/time (not used for routes).</summary>
        public string startDateTime;
        /// <summary>End date/time (not used for routes).</summary>
        public string endDateTime;
        /// <summary>Road name (not used for routes).</summary>
        public string roadName;
        /// <summary>Corridor IDs (not used for routes).</summary>
        public string corridorIds;
        /// <summary>Whether the disruption is provisional.</summary>
        public bool isProvisional;
        /// <summary>Closure text (not used for routes).</summary>
        public string closureText;
        /// <summary>Geometry of the disruption (not used for routes).</summary>
        public string disruptionGeometry;
    }
}
