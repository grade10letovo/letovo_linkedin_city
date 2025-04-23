using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance { get; private set; }

    public string ServerAddress { get; private set; }
    public string DatabaseDSN { get; private set; }
    public string Platform { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadConfig(string configFilePath)
    {
        string json = File.ReadAllText(configFilePath);
        Config config = JsonConvert.DeserializeObject<Config>(json);

        ServerAddress = config.ServerAddress;
        DatabaseDSN = config.DatabaseDSN;
        Platform = Application.platform.ToString(); // Automatically set platform (e.g., Windows, Android)

        Debug.Log("Configuration loaded.");
    }

    [System.Serializable]
    public class Config
    {
        public string ServerAddress;
        public string DatabaseDSN;
    }
}
