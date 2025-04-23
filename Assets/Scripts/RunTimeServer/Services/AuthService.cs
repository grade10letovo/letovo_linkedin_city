using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class AuthService : MonoBehaviour
{
    [Header("Server Settings")]
    public string baseUrl = "http://localhost:8080";
    public string token = "dev-token-123";

    public Action<string> OnLoginSuccess;
    public Action<string> OnLoginError;

    public void Authenticate()
    {
        StartCoroutine(SendAuthRequest());
    }

    private IEnumerator SendAuthRequest()
    {
        string url = $"{baseUrl}/auth/validate-token";

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
        request.SetRequestHeader("Authorization", $"Bearer {token}");
        request.downloadHandler = new DownloadHandlerBuffer();

        Debug.Log($"[AUTH] 🔐 Sending token: {token}");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[AUTH] ❌ Network Error: {request.error}");
            OnLoginError?.Invoke(request.error);
            yield break;
        }

        Debug.Log($"[AUTH] 📥 Response: {request.downloadHandler.text}");

        try
        {
            AuthResponse response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);

            if (response.valid)
            {
                Debug.Log($"[AUTH] ✅ Authorized as {response.playerId}");
                OnLoginSuccess?.Invoke(response.playerId);
            }
            else
            {
                Debug.LogWarning("[AUTH] ⚠️ Invalid token (response.valid = false)");
                OnLoginError?.Invoke("Invalid token");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[AUTH] ❌ Failed to parse response: {ex.Message}");
            OnLoginError?.Invoke("Response parsing error");
        }
    }

    [Serializable]
    public class AuthResponse
    {
        public string playerId;
        public bool valid;
    }
}
