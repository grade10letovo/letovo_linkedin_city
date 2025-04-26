using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class ConfigLoader
{
    public static T LoadConfig<T>(string path)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, path);
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Config file not found at {fullPath}");
            return default;
        }

        string json = File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<T>(json);
    }
    public static string LoadUrl(string additional_url, string protocol="https")
    {
        string projectRootPath = Path.GetFullPath(Path.Combine(Application.dataPath, "Config/config.json"));
        var config = LoadConfig<Config>(projectRootPath);
        if (config != null)
        {
            return protocol + "://" + config.serverUrl + "/" + additional_url;
        }
        else
        {
            Debug.LogError("Failed to load Auth config.");
            return null;
        }
    }
    private string LoadToken()
    {
        if (PlayerPrefs.HasKey("AuthToken"))
        {
            return PlayerPrefs.GetString("AuthToken");
        }
        else
        {
            Debug.LogError("No AuthToken found in PlayerPrefs.");
            return null;
        }
    }
}

[System.Serializable]
public class Config
{
    public string serverUrl;
}