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
        try
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT x_coord, y_coord FROM vertices", conn))
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
                Debug.Log($"✅ Loaded {vertices.Count} vertices from database.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error loading vertices from database: " + ex.Message);
        }
        return vertices;
    }

    public List<(int, int)> GetEdgeData()
    {
        List<(int, int)> edges = new List<(int, int)>();
        try
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT start_vertex_id, end_vertex_id FROM edges", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int start = reader.GetInt32(0) - 1;
                        int end = reader.GetInt32(1) - 1;
                        edges.Add((start, end));
                    }
                }
                Debug.Log($"✅ Loaded {edges.Count} edges from database.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error loading edges from database: " + ex.Message);
        }
        return edges;
    }
}

public class MapGenerator : MonoBehaviour
{
    [Header("Terrain Generator Reference")]
    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private Transform mapParent;
    [SerializeField] private CityGraphData cityGraphData;

    private IMapDataReader dataReader;
    private List<Vector3> vertices;
    private List<(int, int)> edges;

    public void GenerateMapFromEditor()
    {
        try
        {
            dataReader = new PostgresMapDataReader();
            LoadData();
            GenerateMap();
            SaveGraphData();
            Debug.Log("✅ Map generated from editor!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error generating map: " + ex.Message);
        }
    }

    private void LoadData()
    {
        vertices = dataReader.GetVertexData();
        edges = dataReader.GetEdgeData();
        Debug.Log($"📦 Loaded Vertices: {vertices.Count}, Edges: {edges.Count}");
    }

    private void GenerateMap()
    {
        if (mapParent == null)
        {
            mapParent = new GameObject("MapParent").transform;
        }
        for (int i = mapParent.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(mapParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            // Генерируем террейн с помощью TerrainGenerator вместо префабов островов
            if (terrainGenerator != null)
            {
                GameObject terrainObj = terrainGenerator.GenerateTerrain();
                if (terrainObj != null && mapParent != null)
                {
                    terrainObj.transform.SetParent(mapParent, true);
                }
                terrainObj.name = $"Island {i}";
                terrainObj.transform.position = vertices[i];
            }
            else
            {
                Debug.LogError("TerrainGenerator is not assigned!");
            }
        }

        foreach (var edge in edges)
        {
            if (edge.Item1 < 0 || edge.Item2 < 0 || edge.Item1 >= vertices.Count || edge.Item2 >= vertices.Count)
            {
                Debug.LogError($"❗ Invalid edge: {edge.Item1} -> {edge.Item2}");
                continue;
            }
            CreateRoad(vertices[edge.Item1], vertices[edge.Item2]);
        }

        Debug.Log("🗺️ Map generation complete.");
    }

    private void CreateRoad(Vector3 start, Vector3 end)
    {
        if (roadPrefab == null)
        {
            Debug.LogWarning("⚠️ Road prefab is missing!");
            return;
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
            Debug.Log("💾 Graph data saved to CityGraphData asset.");
#if UNITY_EDITOR
            EditorUtility.SetDirty(cityGraphData);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}