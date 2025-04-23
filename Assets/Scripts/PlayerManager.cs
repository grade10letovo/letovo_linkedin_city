using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public GameObject playerPrefab;

    private Dictionary<string, GameObject> players = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnPlayerJoined(string playerId, Vector3 position)
    {
        if (players.ContainsKey(playerId))
        {
            Debug.LogWarning($"[PLAYER_MANAGER] Player {playerId} already exists");
            return;
        }

        GameObject newPlayer = Instantiate(playerPrefab, position, Quaternion.identity);
        newPlayer.name = playerId;
        players[playerId] = newPlayer;

        Debug.Log($"[PLAYER_MANAGER] ➕ Player spawned: {playerId}");
    }

    public void OnPlayerMoved(string playerId, Vector3 position)
    {
        if (!players.ContainsKey(playerId))
        {
            Debug.LogWarning($"[PLAYER_MANAGER] ⚠️ Tried to move unknown player: {playerId}");
            return;
        }

        GameObject playerGO = players[playerId];

        // Попробуем найти SmoothFollower
        var follower = playerGO.GetComponent<SmoothFollower>();
        if (follower != null)
        {
            follower.SetTarget(position);
            Debug.Log($"[PLAYER_MANAGER] 🎯 Smooth move set for {playerId} → {position}");
            return;
        }

        // Если нет — fallback на обычное перемещение
        try
        {
            CharacterController cc = playerGO.GetComponent<CharacterController>();
            Rigidbody rb = playerGO.GetComponent<Rigidbody>();
            Vector3 currentPos = playerGO.transform.position;
            Vector3 delta = position - currentPos;

            if (cc != null)
            {
                cc.Move(delta);
                Debug.Log($"[PLAYER_MANAGER] ✅ Moved {playerId} using CharacterController");
            }
            else if (rb != null)
            {
                rb.MovePosition(position);
                Debug.Log($"[PLAYER_MANAGER] ✅ Moved {playerId} using Rigidbody");
            }
            else
            {
                playerGO.transform.position = position;
                Debug.Log($"[PLAYER_MANAGER] ✅ Moved {playerId} with transform.position");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[PLAYER_MANAGER] ❌ Error moving player {playerId}: {ex.Message}");
        }
    }



    public void OnPlayerLeft(string playerId)
    {
        if (!players.ContainsKey(playerId))
        {
            Debug.LogWarning($"[PLAYER_MANAGER] ⚠️ Tried to remove unknown player: {playerId}");
            return;
        }

        Destroy(players[playerId]);
        players.Remove(playerId);
        Debug.Log($"[PLAYER_MANAGER] ❌ Player removed: {playerId}");
    }

    public bool HasPlayer(string playerId) => players.ContainsKey(playerId);
}
