using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class WorldStateManager : MonoBehaviour
{
    private Dictionary<string, GameObject> players = new();
    public GameObject playerPrefab;

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
            case "player_joined":
                var join = JsonConvert.DeserializeObject<JoinedMessage>(json);
                if (!players.ContainsKey(join.playerId))
                {
                    GameObject _go = Instantiate(playerPrefab);
                    _go.name = join.playerId;
                    players[join.playerId] = _go;
                }
                break;

            case "player_moved":
                var move = JsonConvert.DeserializeObject<MoveMessage>(json);
                if (players.TryGetValue(move.playerId, out var target))
                {
                    target.GetComponent<SmoothFollower>()?.SetTarget(new Vector3(
                        move.position.x, move.position.y, move.position.z));
                }
                break;

            case "player_left":
                var leave = JsonConvert.DeserializeObject<LeftMessage>(json);
                if (players.TryGetValue(leave.playerId, out var go))
                {
                    Destroy(go);
                    players.Remove(leave.playerId);
                }
                break;
        }
    }
}