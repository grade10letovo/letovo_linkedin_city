using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CityGraphData", menuName = "Graph/CityGraphData")]
public class CityGraphData : ScriptableObject
{
    [SerializeField] private List<Vector3> vertices = new List<Vector3>();
    [SerializeField] private List<Vector2Int> edges = new List<Vector2Int>();

    public List<Vector3> Vertices => vertices;
    public List<(int, int)> Edges
    {  get
        {
            List<(int, int)> edges_list = new List<(int, int)>();
            foreach (var edge in edges)
            {
                edges_list.Add((edge.x, edge.y));
            }
            return edges_list;
        }
    }

    public void SetData(List<Vector3> newVertices, List<(int, int)> newEdges)
    {
        vertices = new List<Vector3>(newVertices);
        edges = new List<Vector2Int>();
        foreach (var edge in newEdges)
        {
            edges.Add(new Vector2Int(edge.Item1, edge.Item2));
        }
    }
}
