using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.IO;

public class AuthService : MonoBehaviour
{
    private string _authUrl;
    private string _token;
    public Action<string> OnLoginSuccess;
    public Action<string> OnLoginError;
    private void LoadToken()
    {
        if (PlayerPrefs.HasKey("AuthToken"))
        {
            _token = PlayerPrefs.GetString("AuthToken");
        }
        else
        {
            Debug.LogError("No AuthToken found in PlayerPrefs.");
        }
    }

    public void Authenticate()
    {
        _authUrl= ConfigLoader.LoadUrl("auth/validate-token", "http");
        LoadToken();
        StartCoroutine(SendAuthRequest());
    }

    private IEnumerator SendAuthRequest()
    {
        UnityWebRequest request = UnityWebRequest.PostWwwForm(_authUrl, "");
        request.SetRequestHeader("Authorization", $"Bearer {_token}");
        request.downloadHandler = new DownloadHandlerBuffer();

        Debug.Log($"[AUTH] 🔐 Sending token: {_token}");

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
