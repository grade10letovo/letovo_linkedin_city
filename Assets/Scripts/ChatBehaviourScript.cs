using Mirror;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ChatBehavior : NetworkBehaviour
{
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputField = null;
    [SerializeField] private GameObject canvas = null;
    public string login = "";

    private static event Action<string> OnMessage;
    private static readonly Dictionary<string, ChatBehavior> playersByLogin = new();

    private string apiAuth = "https://linkedin.veconomics.ru/api/auth/check/";

    void Start()
    {
        StartCoroutine(GetDataFromApi());
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
                login = userResponse.user;
            }
            if (isServer)
            {
                playersByLogin[login] = this;
            }
            else if (isOwned)
            {
                CmdRegisterLogin(login);
            }
        }
    }

    [Command]
    private void CmdRegisterLogin(string loginName)
    {
        if (!playersByLogin.ContainsKey(loginName))
        {
            playersByLogin.Add(loginName, this);
        }
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (!isOwned) { return; }

        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(string message)
    {
        chatText.text += message;
    }

    [Client]
    public void Send()
    {
        if (!Input.GetKeyDown(KeyCode.Return)) { return; }
        if (string.IsNullOrWhiteSpace(inputField.text)) { return; }
        CmdSendMessage(inputField.text);
        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        if (message.StartsWith("@"))
        {
            int spaceIndex = message.IndexOf(' ');
            if (spaceIndex > 1)
            {
                string targetLogin = message.Substring(1, spaceIndex - 1);
                string privateMessage = message.Substring(spaceIndex + 1);
                
                Debug.Log("Trying to find: " + targetLogin);

                if (playersByLogin.TryGetValue(targetLogin, out ChatBehavior targetPlayer))
                {
                    // отправляем приватные сообщения в желтом цвете
                    string formattedMessage = $"<color=#FFD700>[Private from {login}]: {privateMessage}</color>";
                    targetPlayer.TargetReceivePrivateMessage(formattedMessage);

                    string confirmationMessage = $"<color=#FFD700>[Private to {targetLogin}]: {privateMessage}</color>";
                    TargetReceivePrivateMessage(confirmationMessage);
                    return;
                }
                else
                {
                    // если не нашли пользователя
                    string errorMessage = $"<color=#FF0000>[Error]: User '{targetLogin}' not found.</color>";
                    TargetReceivePrivateMessage(errorMessage);
                    return;
                }
            }
        }

        RpcHandleMessage($"[{login}]: {message}");
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }

    [TargetRpc]
    private void TargetReceivePrivateMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }
}
