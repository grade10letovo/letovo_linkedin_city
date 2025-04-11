using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using Npgsql;

public class ChatBehavior : NetworkBehaviour
{
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputField = null;
    [SerializeField] private GameObject canvas = null;

    [SyncVar(hook = nameof(OnLoginChanged))]
    public string login = "";

    private bool isAuthenticated = false;

    private static event Action<string> OnMessage;
    private string apiAuth = "http://127.0.0.1:8000/api/auth/check/";

    private string dbHost = "localhost";
    private string dbName = "test_database";
    private string dbUser = "postgres";
    private string dbPass = "0000";
    private string dbPort = "5432";

    void Start()
    {
        StartCoroutine(GetDataFromApi());

        if (isServer)
        {
            LoadLastMessagesFromDatabase();
        }
    }

    public override void OnStartAuthority()
    {
        canvas.SetActive(true);
        OnMessage += HandleNewMessage;
    }

    [System.Serializable]
    public class UserResponse
    {
        public string user;
        public string name;
        public string surname;
    }

    IEnumerator GetDataFromApi()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiAuth))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                login = "unauthorized";
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                UserResponse userResponse = JsonUtility.FromJson<UserResponse>(jsonResponse);
                login = userResponse.name + ' ' + userResponse.surname;
            }

            isAuthenticated = true;
            if (isServer)
            {
                RpcUpdateLogin(login);
            }
        }
    }

    [ClientRpc]
    private void RpcUpdateLogin(string newLogin)
    {
        login = newLogin;
    }

    private void OnLoginChanged(string oldLogin, string newLogin)
    {
        login = newLogin;
        isAuthenticated = true;
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (!isOwned)
        {
            return;
        }

        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(string message)
    {
        chatText.text += message;
    }

    [Client]
    public void Send()
    {
        if (!Input.GetKeyDown(KeyCode.Return))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            return;
        }

        if (!isAuthenticated)
        {
            return;
        }

        CmdSendMessage(inputField.text);
        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        if (string.IsNullOrEmpty(login))
        {
            login = "unauthorized";
        }

        string fullMessage = $"[{login}]: {message}";

        if (isServer)
        {
            SaveMessageToDatabase(login, message);
        }

        RpcHandleMessage(fullMessage);
    }

    private void SaveMessageToDatabase(string sender, string message)
    {
        string connString = $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPass};Database={dbName}";

        try
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText =
                        "INSERT INTO chat_messages (sender, message, timestamp) VALUES (@sender, @message, @timestamp)";

                    cmd.Parameters.AddWithValue("sender", sender);
                    cmd.Parameters.AddWithValue("message", message);
                    cmd.Parameters.AddWithValue("timestamp", DateTime.UtcNow);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при сохранении сообщения в БД: {ex.Message}");
        }
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }

    private void LoadLastMessagesFromDatabase()
    {
        string connString = $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPass};Database={dbName}";

        try
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT sender, message FROM chat_messages ORDER BY timestamp DESC LIMIT 20";

                    var messages = new List<string>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sender = reader.GetString(0);
                            string message = reader.GetString(1);
                            messages.Add($"[{sender}]: {message}");
                        }
                    }

                    messages.Reverse();

                    foreach (var msg in messages)
                    {
                        RpcHandleMessage(msg);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при загрузке сообщений из БД: {ex.Message}");
        }
    }
}