using UnityEngine;
using System.Collections.Generic;
using Npgsql;
#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IMapDataReader
{
    List<Vector3> GetVertexData();
    List<(int, int)> GetEdgeData();
}

public class PostgresMapDataReader : IMapDataReader
{
    private const string ConnectionString = "Host=localhost;Username=postgres;Password=code1234;Database=city";

    public List<Vector3> GetVertexData()
    {
        List<Vector3> vertices = new List<Vector3>();
        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("SELECT x_coord, y_coord FROM Vertices", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    float x = (float)reader.GetDouble(0);
                    float y = 0;
                    float z = (float)reader.GetDouble(1);
                    vertices.Add(new Vector3(x, y, z));
                }
            }
        }
        return vertices;
    }

    public List<(int, int)> GetEdgeData()
    {
        List<(int, int)> edges = new List<(int, int)>();
        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("SELECT start_vertex_id, end_vertex_id FROM Edges", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int start = reader.GetInt32(0) - 1;
                    int end = reader.GetInt32(1) - 1;
                    edges.Add((start, end));
                }
            }
        }
        return edges;
    }
}

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject islandPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private Transform mapParent;
    [SerializeField] private CityGraphData cityGraphData;

    private IMapDataReader dataReader;
    private List<Vector3> vertices;
    private List<(int, int)> edges;

    public void GenerateMapFromEditor()
    {
        LoadData();
        GenerateMap();
        SaveGraphData();
        Debug.Log("Map generated from editor!");
    }

    void Start()
    {
        dataReader = new PostgresMapDataReader();
        LoadData();
    }

    private void LoadData()
    {
        vertices = dataReader.GetVertexData();
        edges = dataReader.GetEdgeData();

        Debug.Log($"Loaded Vertices: {vertices.Count}, Edges: {edges.Count}");
    }

    private void GenerateMap()
    {
        if (mapParent == null)
        {
            mapParent = new GameObject("MapParent").transform;
        }

        foreach (Transform child in mapParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            Instantiate(islandPrefab, vertices[i], Quaternion.identity, mapParent).name = $"Island {i}";
        }

        foreach (var edge in edges)
        {
            if (edge.Item1 < 0 || edge.Item2 < 0 || edge.Item1 >= vertices.Count || edge.Item2 >= vertices.Count)
            {
                Debug.LogError($"Invalid edge: {edge.Item1} -> {edge.Item2}");
                continue;
            }

            CreateRoad(vertices[edge.Item1], vertices[edge.Item2]);
        }
    }

    private void CreateRoad(Vector3 start, Vector3 end)
    {
        if (roadPrefab == null)
        {
            Debug.LogWarning("Road prefab is missing!!");

        }
        GameObject road = Instantiate(roadPrefab, mapParent);
        road.transform.position = (start + end) / 2f;
        road.transform.rotation = Quaternion.LookRotation(end - start);
        road.transform.localScale = new Vector3(0.2f, 0.2f, (end - start).magnitude);
    }

    private void SaveGraphData()
    {
        if (cityGraphData)
        {
            cityGraphData.SetData(vertices, edges);
            Debug.Log("Graph data saved!");
#if UNITY_EDITOR
            EditorUtility.SetDirty(cityGraphData);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}
