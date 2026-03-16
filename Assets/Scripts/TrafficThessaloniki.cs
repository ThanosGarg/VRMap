using UnityEngine;
using System;
using System.Collections;

public class TrafficThessaloniki : MonoBehaviour
{
    [SerializeField] private string API_URL = "https://api.cluster.imetb.gr/trafficthess/api/links?timestamp=2026-03-10T16:03:00Z";
    [SerializeField] private string USERNAME = "trafficthess";
    [SerializeField] private string PASSWORD = "th3sstr@ff1c26";

    // Public variable to hold the traffic response
    public TrafficResponse trafficResponse;

    public Action<TrafficResponse> OnTrafficDataReceived;
    void Start()
    {
        GetTrafficData();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GetTrafficData()
    {

        StartCoroutine(GetTrafficData(API_URL, USERNAME, PASSWORD));
    }

    private IEnumerator GetTrafficData(string url, string username, string password)
    {
        var request = new UnityEngine.Networking.UnityWebRequest(url, "GET");
        request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
        string auth = System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(username + ":" + password));
        request.SetRequestHeader("Authorization", "Basic " + auth);
        yield return request.SendWebRequest();

        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("Traffic data: " + json);
            // Deserialize JSON into TrafficResponse
            trafficResponse = JsonUtility.FromJson<TrafficResponse>(json);
            OnTrafficDataReceived?.Invoke(trafficResponse);
        }
        else
        {
            Debug.LogError("Error fetching traffic data: " + request.error);
        }
    }

}
[Serializable]
public class TrafficLink
{
    public int linkId;
    public string streetName;
    public int linkFreeFlowSpeed;
    public string geog;
    public int? speed;
    public int? samples;
    public int? uniqueEntries;
    public string latestTimestamp;
}

[Serializable]
public class TrafficResponse
{
    public bool success;
    public string message;
    public int count;
    public TrafficLink[] data;
}