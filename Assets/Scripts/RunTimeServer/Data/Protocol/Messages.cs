using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vec3
{
    public float x, y, z;
}
[System.Serializable]
public class Vec2
{
    public float x, y;
}


[System.Serializable]
public class InputMessage
{
    public string playerId;
    public Vec2 move;
    public Vec2 look;
    public bool jump;
    public bool sprint;
    public bool dance;
}

[System.Serializable]
public class JoinedMessage
{
    public string playerId;
    public Vec3 playerPosition;
}

[System.Serializable]
public class LeftMessage
{
    public string playerId;
}


[System.Serializable]
public class MoveMessage
{
    public string playerId;
    public Vec3 position;
}

[System.Serializable]
public class WorldSnapshotMessage
{
    public List<JoinedMessage> players;
}

[System.Serializable]
public class NetworkMessage
{
    public string type;
    public object data;
}
