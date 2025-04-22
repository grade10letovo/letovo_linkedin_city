using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ServerAPI : MonoBehaviour
{
    private const string baseUrl = "http://localhost:8080/api";  // Заменить на ваш серверный адрес

    // ---------------------------------- Методы для загрузки/сохранения данных мира ----------------------------------

    // Получение состояния мира в JSON формате
    public async Task<string> GetWorldStateJson(string worldId, string platform)
    {
        string url = $"{baseUrl}/world/{worldId}/state/{platform}";
        return await HttpGetAsync(url);
    }

    // Получение патчей (диффа) с момента последнего известного snapshot
    public async Task<DeltaResponse> GetDeltaSince(string worldId, string platform, string lastKnownSnapshotHash)
    {
        string url = $"{baseUrl}/world/{worldId}/delta/{platform}/{lastKnownSnapshotHash}";
        string response = await HttpGetAsync(url);
        return JsonUtility.FromJson<DeltaResponse>(response);
    }

    // Сохранение состояния мира
    public async Task<bool> SaveWorldState(string worldId, string platform, string jsonData, string worldSnapshotHash)
    {
        string url = $"{baseUrl}/world/{worldId}/state/{platform}";
        var payload = new
        {
            jsonData,
            snapshotHash = worldSnapshotHash
        };
        string response = await HttpPostAsync(url, payload);
        return response.Contains("success");
    }

    // Получение информации о версии мира
    public async Task<WorldVersionInfo> GetWorldVersionInfo(string worldId, string platform)
    {
        string url = $"{baseUrl}/world/{worldId}/version/{platform}";
        string response = await HttpGetAsync(url);
        return JsonUtility.FromJson<WorldVersionInfo>(response);
    }

    // ---------------------------------- Методы для работы с ассетами ----------------------------------

    // Получение ассет‑bundle
    public async Task<AssetDownloadResponse> GetAssetBundle(string assetBundleHash)
    {
        string url = $"{baseUrl}/assets/bundle/{assetBundleHash}";
        string response = await HttpGetAsync(url);
        return JsonUtility.FromJson<AssetDownloadResponse>(response);
    }

    // Загрузка ассет‑bundle на сервер
    public async Task<bool> UploadAssetBundle(string worldId, string platform, string assetBundleHash, byte[] assetBundleData)
    {
        string url = $"{baseUrl}/assets/upload/{worldId}/{platform}/{assetBundleHash}";
        var form = new WWWForm();
        form.AddBinaryData("file", assetBundleData);
        string response = await HttpPostAsync(url, form);
        return response.Contains("success");
    }

    // Проверка подписей данных
    public async Task<bool> ValidateSignature(string worldId, string platform, string signature, string payload)
    {
        string url = $"{baseUrl}/world/{worldId}/validate-signature/{platform}";
        var payloadData = new
        {
            signature = signature,
            payload = payload
        };
        string response = await HttpPostAsync(url, payloadData);
        return response.Contains("valid");
    }

    // ---------------------------------- Методы для безопасности ----------------------------------

    // Валидация ассета перед загрузкой
    public async Task<bool> ValidateAssetBundle(string assetBundleHash)
    {
        string url = $"{baseUrl}/assets/validate/{assetBundleHash}";
        string response = await HttpGetAsync(url);
        return response.Contains("valid");
    }

    // Логирование ошибки
    public async Task LogError(string errorType, string message, string stackTrace)
    {
        string url = $"{baseUrl}/log/error";
        var payload = new
        {
            errorType,
            message,
            stackTrace
        };
        await HttpPostAsync(url, payload);
    }

    // ---------------------------------- Вспомогательные методы для HTTP запросов ----------------------------------

    // Асинхронный GET запрос
    private async Task<string> HttpGetAsync(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            var tcs = new TaskCompletionSource<string>();

            request.SendWebRequest().completed += (op) =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error fetching data: {request.error}");
                    tcs.SetException(new Exception(request.error));
                }
                else
                {
                    tcs.SetResult(request.downloadHandler.text);
                }
            };

            return await tcs.Task;
        }
    }

    // Асинхронный POST запрос с JSON
    private async Task<string> HttpPostAsync(string url, object payload)
    {
        string json = JsonUtility.ToJson(payload);
        using (UnityWebRequest request = UnityWebRequest.Post(url, json))
        {
            var tcs = new TaskCompletionSource<string>();

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            request.SendWebRequest().completed += (op) =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error sending data: {request.error}");
                    tcs.SetException(new Exception(request.error));
                }
                else
                {
                    tcs.SetResult(request.downloadHandler.text);
                }
            };

            return await tcs.Task;
        }
    }

    // Асинхронный POST запрос с формой (для ассетов)
    private async Task<string> HttpPostFormAsync(string url, WWWForm form)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            var tcs = new TaskCompletionSource<string>();

            request.SendWebRequest().completed += (op) =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error sending form: {request.error}");
                    tcs.SetException(new Exception(request.error));
                }
                else
                {
                    tcs.SetResult(request.downloadHandler.text);
                }
            };

            return await tcs.Task;
        }
    }
}
