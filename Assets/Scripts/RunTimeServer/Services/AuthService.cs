using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class AuthService : MonoBehaviour
{
    public string baseUrl = "http://localhost:8080";
    public string platform = "unity";
    public string token = "super-secret-token";

    public Action<string> OnLoginSuccess;
    public Action<string> OnLoginError;

    public void Authenticate()
    {
        StartCoroutine(SendAuthRequest());
    }

    private IEnumerator SendAuthRequest()
    {
        string url = $"{baseUrl}/auth/validate-signature/{platform}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.SetRequestHeader("Authorization", $"Bearer {token}");
        request.uploadHandler = new UploadHandlerRaw(new byte[0]);
        request.downloadHandler = new DownloadHandlerBuffer();

        Debug.Log("[AUTH] 🔐 Sending login request...");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[AUTH] ❌ Error: {request.error}");
            OnLoginError?.Invoke(request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            var response = JsonUtility.FromJson<AuthResponse>(json);

            if (response.valid)
            {
                Debug.Log($"[AUTH] ✅ Auth success! PlayerId: {response.playerId}");
                OnLoginSuccess?.Invoke(response.playerId);
            }
            else
            {
                Debug.LogWarning("[AUTH] ⚠️ Invalid token");
                OnLoginError?.Invoke("Invalid token");
            }
        }
    }

    [Serializable]
    public class AuthResponse
    {
        public string playerId;
        public bool valid;
    }
}
