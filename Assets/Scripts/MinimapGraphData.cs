using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinimapGraphData", menuName = "Minimap/Graph Data")]
public class MinimapGraphData : ScriptableObject
{
    [System.Serializable]
    public struct Vertex
    {
        public Vector2 position;    // x, z
        public string vertexName;   // переименовали из УnameФ
    }

    [System.Serializable]
    public struct Edge
    {
        public int startIndex;
        public int endIndex;
    }

    public List<Vertex> vertices = new List<Vertex>();
    public List<Edge> edges = new List<Edge>();
}
