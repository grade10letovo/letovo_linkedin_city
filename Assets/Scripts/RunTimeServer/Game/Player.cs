using UnityEngine;

public class Player
{
    public string playerId;
    public string displayName;
    public GameObject gameObject;

    public Player(string playerId, string displayName, GameObject gameObject)
    {
        this.playerId = playerId;
        this.displayName = displayName;
        this.gameObject = gameObject;
    }
}