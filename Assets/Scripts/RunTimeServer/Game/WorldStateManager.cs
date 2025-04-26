using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class WorldStateManager : MonoBehaviour
{
    private Dictionary<string, Player> players = new();
    public GameObject playerPrefab;
    [SerializeField] private GameObject nameTagPrefab;
    [SerializeField] private Canvas worldCanvas;

    private void OnEnable()
    {
        MessageDispatcher.OnMessageReceive += OnMessage;
    }

    private void OnDisable()
    {
        MessageDispatcher.OnMessageReceive -= OnMessage;
    }

    private void OnMessage(string type, string json)
    {
        switch (type)
        {
            case "world_snapshot":
                var snapshot = JsonConvert.DeserializeObject<WorldSnapshotMessage>(json);
                if (snapshot.players == null)
                    snapshot.players = new List<JoinedMessage>();


                Debug.Log($"[WORLD] 🌍 Received snapshot with {snapshot.players.Count} players");

                foreach (var _player in snapshot.players)
                {
                    SpawnPlayer(_player.playerId, new Vector3(
                        _player.playerPosition.x,
                        _player.playerPosition.y,
                        _player.playerPosition.z));
                }
                break;

            case "player_joined":
                var join = JsonConvert.DeserializeObject<JoinedMessage>(json);
                SpawnPlayer(join.playerId, Vector3.zero); // или передавать позицию если есть
                break;

            case "player_moved":
                var move = JsonConvert.DeserializeObject<MoveMessage>(json);
                if (players.TryGetValue(move.playerId, out var playerMoved))
                {
                    playerMoved.gameObject.GetComponent<SmoothFollower>()?.SetTarget(new Vector3(
                        move.position.x, move.position.y, move.position.z));
                }
                break;

            case "player_input":
                var input = JsonConvert.DeserializeObject<InputMessage>(json);
                if (players.TryGetValue(input.playerId, out var inputTarget))
                {
                    var remote = inputTarget.gameObject.GetComponent<SharedPlayerController>();
                    remote?.ApplyNetworkInputToPlayer(input);
                }
                break;

            case "player_left":
                var leave = JsonConvert.DeserializeObject<LeftMessage>(json);
                if (players.TryGetValue(leave.playerId, out var player))
                {
                    Destroy(player.gameObject);
                    players.Remove(leave.playerId);
                }
                break;
        }
    }

    private void SpawnPlayer(string playerId, Vector3 position)
    {
        if (playerId == WebSocketClient.Instance.playerId)
        {
            Debug.Log($"[WORLD] 🧍‍♂️ Skipping spawn of local player: {playerId}");
            return;
        }

        if (players.ContainsKey(playerId)) return;

        GameObject go = Instantiate(playerPrefab);
        go.name = playerId;
        go.transform.position = position;
        var remote = go.GetComponent<SharedPlayerController>();
        players[playerId] = new Player(playerId, "", go);
        remote.player = players[playerId];
        remote.isLocalPlayer = false;

        // === UI нэймтэг ===
        if (nameTagPrefab != null && worldCanvas != null)
        {
            GameObject tag = Instantiate(nameTagPrefab, worldCanvas.transform);
            NameTag nameTag = tag.GetComponent<NameTag>();
            nameTag.target = go.transform;

            // используем display name, если есть
            string displayName = players[playerId].displayName.Length > 0  ? players[playerId].displayName : playerId;
            nameTag.SetName(displayName);
        }

        Debug.Log($"[WORLD] 🧍 Spawned remote player: {playerId} at {position}");
    }

}