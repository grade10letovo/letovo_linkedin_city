using Mirror;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;



public class ChatBehavior : NetworkBehaviour
{
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputField = null;
    [SerializeField] private GameObject canvas = null;
    public string login = "";
    private static event Action<string> OnMessage;
    private string apiAuth = "http://127.0.0.1:8000/api/auth/check/";
    
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
        }
    }

    [ClientCallback]
    private void OnDestroy()
    {
        if(!isOwned) { return; }

        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(string message)
    {
        chatText.text += message;
    }

    [Client]
    public void Send()
    {
        if(!Input.GetKeyDown(KeyCode.Return)) { return; }
        if (string.IsNullOrWhiteSpace(inputField.text)) { return; }
        CmdSendMessage(inputField.text);
        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        RpcHandleMessage($"[{login}]: {message}");
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }

}