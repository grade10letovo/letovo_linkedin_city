using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CityGraphData", menuName = "Graph/CityGraphData")]
public class CityGraphData : ScriptableObject
{
    [SerializeField] private List<Vector3> vertices = new List<Vector3>();
    [SerializeField] private List<(int, int)> edges = new List<(int, int)>();

    public List<Vector3> Vertices => vertices;
    public List<(int, int)> Edges => edges;

    public void SetData(List<Vector3> newVertices, List<(int, int)> newEdges)
    {
        vertices = new List<Vector3>(newVertices);
        edges = new List<(int, int)>(newEdges);
    }
}
