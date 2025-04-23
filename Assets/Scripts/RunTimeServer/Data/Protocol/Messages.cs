using System.Collections.Generic;

[System.Serializable]
public class Vec3
{
    public float x, y, z;
}

[System.Serializable]
public class JoinedMessage
{
    public string playerId;
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
