using NativeWebSocket;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket websocket;
    public string playerId = "player-" + System.Guid.NewGuid().ToString().Substring(0, 6);

    private Dictionary<string, Player> players = new();

    public Transform myPlayer;

    public static WebSocketClient Instance;

    float moveTimer = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("You have several WebSocketClient on the scene. It must be only 1. WebSocketClient deleted other objects.");
            Destroy(gameObject); // Защита от дубликатов
        }
    }

    public void Start()
    {
        StartClient();
    }

    public void StartClient() 
    { 
        AuthService auth = FindObjectOfType<AuthService>();
        auth.OnLoginSuccess += OnAuthSuccess;
        auth.OnLoginError += OnAuthFailed;

        auth.Authenticate(); // ← запускаем авторизацию
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (websocket != null)
            websocket.DispatchMessageQueue();
#endif
    }

    private void OnEnable()
    {
        MessageDispatcher.OnMessageSend += Send;
    }

    private void OnDisable()
    {
        MessageDispatcher.OnMessageSend -= Send;
    }

    private void OnAuthSuccess(string id)
    {
        playerId = id;
        ConnectWebSocket();
    }

    private void OnAuthFailed(string reason)
    {
        Debug.LogError("[CLIENT] ❌ Auth failed, reason: " + reason);
    }
    private async void ConnectWebSocket()
    {
        string url = ConfigLoader.LoadUrl("ws", "ws");
        websocket = new WebSocket(url);

        websocket.OnOpen += () =>
        {
            Debug.Log("[CLIENT] ✅ Connected");
            MessageDispatcher.Send("player_joined", new JoinedMessage
            {
                playerId = playerId,
                playerPosition = new Vec3
                {
                    x = myPlayer.position.x,
                    y = myPlayer.position.y
                }
            });
        };

        websocket.OnMessage += (bytes) => {
            string raw = System.Text.Encoding.UTF8.GetString(bytes);
            var wrapper = JsonConvert.DeserializeObject<NetworkMessage>(raw);
            MessageDispatcher.Receive(wrapper.type, JsonConvert.SerializeObject(wrapper.data));
            Debug.Log($"[CLIENT] 📤 Received {wrapper.type}: {wrapper.data}");
        };

        websocket.OnError += (err) => Debug.LogError($"[CLIENT] ❌ Error: {err}");
        websocket.OnClose += (e) => Debug.LogWarning("[CLIENT] 🔌 Disconnected");

        await websocket.Connect();
        MessageDispatcher.OnMessageSend += SendMessage;
    }
    public async void Send(string type, object payload)
    {
        try
        {
            if (websocket == null || websocket.State != WebSocketState.Open)
            {
                Debug.LogWarning("[CLIENT] ⚠️ WebSocket not ready, skipping send");
                return;
            }

            var wrapper = new NetworkMessage { type = type, data = payload };
            string full = JsonConvert.SerializeObject(wrapper);

            await websocket.SendText(full);
            Debug.Log($"[CLIENT] 📤 Sent {type}: {JsonConvert.SerializeObject(payload)}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[CLIENT] ❌ Failed to send '{type}': {ex.Message}");
        }
    }


    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            Debug.Log("[CLIENT] 🔚 Closing WebSocket connection...");
            await websocket.Close();
        }
    }
}
