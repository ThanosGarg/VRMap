using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class DataManager : MonoBehaviour
{



    public PinList pinsData;

    public string jsonFilePath = "C:/Users/thano/OneDrive/Desktop/Data/final_results_07_min.json";

    void Start()
    {
        LoadPinsFromJson();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadPinsFromJson()
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"JSON file not found at {jsonFilePath}");
            return;
        }
        string json = File.ReadAllText(jsonFilePath);
        pinsData = JsonUtility.FromJson<PinList>(json);
        if (pinsData == null)
        {
            Debug.LogError("Failed to deserialize JSON into PinList");
        }
        else
        {
            Debug.Log($"Loaded {pinsData.valid_pins?.Count ?? 0} valid pins");
        }
    }

}
[Serializable]
public class PinList
{
    public List<List<double>> valid_pins;
    public List<List<double>> invalid_pins;
    public List<List<double>> missing_pins;
}