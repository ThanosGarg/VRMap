using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class TrafficThessApiClient : MonoBehaviour
{

    private const string BaseUrl = "https://api.cluster.imetb.gr/trafficthess/api/links?timestamp=2026-03-10T16:03:00Z";
    private const string Username = "trafficthess";
    private const string Password = "th3sstr@ff1c26";
    void Start()
    {
        GetLatest();
    }
    public void GetLatest()
    {
        StartCoroutine(GetLinksCoroutine(null));
    }

    public void GetHistorical(DateTime utcTimestamp)
    {
        StartCoroutine(GetLinksCoroutine(utcTimestamp));
    }

    private IEnumerator GetLinksCoroutine(DateTime? utcTimestamp)
    {
        string url = BaseUrl;

        if (utcTimestamp.HasValue)
        {
            // API expects format like 2026-03-10T16:03:00Z
            string isoTimestamp = utcTimestamp.Value.ToUniversalTime()
                .ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

            url += $"?timestamp={UnityWebRequest.EscapeURL(isoTimestamp)}";
        }

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"));
            request.SetRequestHeader("Authorization", $"Basic {auth}");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            bool hasError = request.result == UnityWebRequest.Result.ConnectionError ||
                            request.result == UnityWebRequest.Result.ProtocolError;
#else
            bool hasError = request.isNetworkError || request.isHttpError;
#endif

            if (hasError)
            {
                Debug.LogError($"TrafficThess API error: {request.responseCode} - {request.error}");
                Debug.LogError(request.downloadHandler.text);
                yield break;
            }

            string json = request.downloadHandler.text;
            Debug.Log($"Response length: {json.Length}");

            try
            {
                // Αν το endpoint επιστρέφει top-level array:
                LinkDto[] links = JsonHelper.FromJson<LinkDto>(json);

                Debug.Log($"Loaded {links?.Length ?? 0} links");

                if (links != null && links.Length > 0)
                {
                    var first = links[0];
                    Debug.Log($"First link id={first.id}, speed={first.speed}, samples={first.samples}, uniqueEntries={first.uniqueEntries}, geog={first.geog}");
                }

                // εδώ μπορείς να τα κρατήσεις σε field / cache / manager
            }
            catch (Exception ex)
            {
                Debug.LogError("JSON parse failed: " + ex.Message);
                Debug.LogError("Raw JSON: " + json);
            }
        }
    }
}

[Serializable]
public class LinkDto
{
    public int id;
    public float? speed;
    public int? samples;
    public int? uniqueEntries;
    public string geog;

    // Αν το payload έχει κι άλλα πεδία, πρόσθεσέ τα εδώ
}

// Helper class for parsing top-level JSON arrays with JsonUtility
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}