using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections;

public class WorldGenerator : MonoBehaviour
{
    [Header("World Settings")]
    public string worldId = "newWorld";  // ID мира
    public string platform = "Windows";  // Платформа (например, Windows, Android)

    [TextArea]
    public string worldDescription = "A new world description";  // Описание мира

    // Метод для сохранения мира
    public void SaveWorld()
    {
        // Создаём данные мира
        var worldData = new
        {
            seed = Random.Range(1, 10000),  // Генерация случайного seed для мира
            description = worldDescription,  // Описание мира
            spawnPoints = new[]
            {
                new { x = 0, y = 0 },
                new { x = 10, y = 5 }
            }
        };

        // Отправляем запрос на сервер
        StartCoroutine(SendCreateWorldRequest(worldId, platform, worldData));
    }

    // Метод для отправки запроса на сервер для создания мира
    private IEnumerator SendCreateWorldRequest(string worldId, string platform, object worldData)
    {
        string url = $"{ConfigManager.Instance.ServerAddress}/world/{worldId}/state/{platform}";

        string json = JsonUtility.ToJson(worldData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {AuthManager.Instance.AuthToken}");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error creating world: {request.error}");
        }
        else
        {
            Debug.Log($"World '{worldId}' successfully created!");
        }
    }
}
