using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GraphData", menuName = "Graph Editor/Graph Data")]
public class GraphData : ScriptableObject
{
    public List<NodeData> nodes = new List<NodeData>();
    public List<EdgeData> edges = new List<EdgeData>();
}

[System.Serializable]
public class NodeData
{
    public Vector2 position;
    public string nodeName;
}

[System.Serializable]
public class EdgeData
{
    public int startNodeIndex;
    public int endNodeIndex;
    public float distance;
}
