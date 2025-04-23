using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    public string AuthToken { get; private set; }

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

    public IEnumerator AuthenticateUser()
    {
        string url = $"{ConfigManager.Instance.ServerAddress}/auth/validate-signature/{ConfigManager.Instance.Platform}";

        // ��������� � ������ ��� ��������������
        var payload = new
        {
            signature = "user_signature", // ����� ����� �������� �������
            payload = "user_data"        // �������� ������ ������������
        };

        string json = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"������ ��������������: {request.error}");
        }
        else
        {
            // ���������� ������
            AuthToken = request.downloadHandler.text;
            Debug.Log($"�������������� �������. �����: {AuthToken}");
        }
    }
}
