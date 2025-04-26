using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ServerApi : MonoBehaviour
{
    private string authToken { 
        get { 
            return LoadAuthToken();
        }
    }
    // Строки для хранения токена и времени окончания сессии
    private float sessionExpirationTime;

    // Адрес сервера для запросов
    private string serverUrl = "http://localhost:8080/api";

    // Примерный класс для ответа сервера
    [Serializable]
    public class ApiResponse
    {
        public bool isSuccessful;
        public string token; // Токен для авторизации
        public float expirationTime; // Время жизни токена в секундах
        public string data; // Прочие данные
    }

    // Метод для авторизации пользователя
    public async Task<bool> Authorize(string username, string password)
    {
        var requestData = new Dictionary<string, string>
        {
            { "username", username },
            { "password", password }
        };

        // Выполняем запрос на сервер
        var response = await SendRequest("POST", "/auth/login", requestData);

        if (response.isSuccessful)
        {
            SaveAuthToken(response.token);  // Сохраняем токен
            sessionExpirationTime = Time.time + response.expirationTime;  // Время окончания сессии
            return true;
        }

        return false;
    }
    // Сохранение токена в зашифрованном файле
    public void SaveAuthToken(string token)
    {
        SecureStorage.SaveAuthToken(token);  // Используем SecureStorage для хранения токена
    }
    // Загрузка токена из зашифрованного файла
    public string LoadAuthToken()
    {
        return SecureStorage.LoadAuthToken();  // Загружаем токен из файла
    }

    public async Task<bool> IsAuthorizedAsync()
    {
        // Проверка на наличие токена и проверка срока его действия
        if (string.IsNullOrEmpty(authToken) || Time.time >= sessionExpirationTime)
        {
            return false;
        }

        // Если токен актуален, можно дополнительно проверить на сервере, действителен ли токен
        bool isValid = await CheckTokenValidityOnServer();
        return isValid;
    }

    // Асинхронная проверка токена на сервере
    private async Task<bool> CheckTokenValidityOnServer()
    {
        // Например, мы можем сделать запрос на сервер, чтобы проверить валидность токена
        var response = await SendRequest("POST", "/auth/validate", new Dictionary<string, string> { { "token", authToken } });

        return response.isSuccessful;
    }


    // Метод для обновления сессии
    public async Task<bool> RefreshSession()
    {
        if (string.IsNullOrEmpty(authToken))
        {
            return false;
        }

        var requestData = new Dictionary<string, string>
        {
            { "token", authToken }
        };

        var response = await SendRequest("POST", "/auth/refresh", requestData);

        if (response.isSuccessful)
        {
            SaveAuthToken(response.token); // Обновляем токен
            sessionExpirationTime = Time.time + response.expirationTime; // Обновляем время окончания сессии
            return true;
        }

        return false;
    }

    // Метод для запроса данных мира с сервера
    public async Task<string> GetWorldDataJson()
    {
        if (!await IsAuthorizedAsync())
        {
            throw new UnauthorizedAccessException("Необходимо авторизоваться.");
        }

        var response = await SendRequest("GET", "/world", new Dictionary<string, string>
        {
            { "token", authToken }
        });

        if (response.isSuccessful)
        {
            return response.data; // Возвращаем полученные данные
        }

        throw new Exception("Ошибка при получении данных мира.");
    }


    // Универсальный метод для отправки запросов на сервер
    private async Task<ApiResponse> SendRequest(string method, string endpoint, Dictionary<string, string> data)
    {
        // Формируем полный URL для запроса
        string url = serverUrl + endpoint;

        // Для POST запроса создаём форму
        WWWForm form = new WWWForm();
        foreach (var kvp in data)
        {
            form.AddField(kvp.Key, kvp.Value);
        }

        // Создаем UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(url, method)
        {
            uploadHandler = new UploadHandlerRaw(form.data),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        // Ожидаем завершения запроса асинхронно
        await SendWebRequestAsync(request);

        // Десериализуем полученные данные
        return JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);
    }
    // Обертка для асинхронного выполнения SendWebRequest
    private async Task SendWebRequestAsync(UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<bool>();

        request.SendWebRequest().completed += (op) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                tcs.SetResult(true);
            }
            else
            {
                tcs.SetException(new Exception(request.error));
            }
        };

        await tcs.Task; // Ожидаем завершения запроса
    }
    // Метод для выхода из системы
    public void Logout()
    {
        SaveAuthToken(null);
        sessionExpirationTime = 0;
    }

    // ---------------------------------- Методы для загрузки/сохранения данных мира ----------------------------------

    public async Task<string> GetWorldStateJson(string worldId, string platform)
    {
        string url = $"{serverUrl}/world/{worldId}/state/{platform}";
        return await HttpGetAsync(url);
    }

    public async Task<DeltaResponse> GetDeltaSince(string worldId, string platform, string lastKnownSnapshotHash)
    {
        string url = $"{serverUrl}/world/{worldId}/delta/{platform}/{lastKnownSnapshotHash}";
        string response = await HttpGetAsync(url);
        return JsonUtility.FromJson<DeltaResponse>(response);
    }

    public async Task<bool> SaveWorldState(string worldId, string platform, string jsonData, string worldSnapshotHash)
    {
        string url = $"{serverUrl}/world/{worldId}/state/{platform}";
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
        string url = $"{serverUrl}/world/{worldId}/version/{platform}";
        string response = await HttpGetAsync(url);
        return JsonUtility.FromJson<WorldVersionInfo>(response);
    }

    // ---------------------------------- Методы для работы с ассетами ----------------------------------

    public async Task<AssetDownloadResponse> GetAssetBundle(string assetBundleHash)
    {
        string url = $"{serverUrl}/assets/bundle/{assetBundleHash}";
        string response = await HttpGetAsync(url);
        return JsonUtility.FromJson<AssetDownloadResponse>(response);
    }

    public async Task<bool> UploadAssetBundle(string worldId, string platform, string assetBundleHash, byte[] assetBundleData)
    {
        string url = $"{serverUrl}/assets/upload/{worldId}/{platform}/{assetBundleHash}";
        var form = new WWWForm();
        form.AddBinaryData("file", assetBundleData);
        string response = await HttpPostFormAsync(url, form);
        return response.Contains("success");
    }

    // Валидация ассета перед загрузкой
    public async Task<bool> ValidateAssetBundle(string assetBundleHash)
    {
        string url = $"{serverUrl}/assets/validate/{assetBundleHash}";
        string response = await HttpGetAsync(url);
        return response.Contains("valid");
    }

    // Логирование ошибки
    public async Task LogError(string errorType, string message, string stackTrace)
    {
        string url = $"{serverUrl}/log/error";
        var payload = new
        {
            errorType,
            message,
            stackTrace
        };
        await HttpPostAsync(url, payload);
    }

    // ---------------------------------- Вспомогательные методы для HTTP запросов ----------------------------------

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

    private async Task<string> HttpPostAsync(string url, object payload)
    {
        string json = JsonUtility.ToJson(payload);
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, json))
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
