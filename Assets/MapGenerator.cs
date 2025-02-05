using UnityEngine;
using System.Collections.Generic;
using Npgsql;

// Интерфейс для работы с источником данных
public interface IDataReader
{
    List<Vector3> GetVertices();
    List<(int, int)> GetEdges();
}

// Реализация источника данных для PostgreSQL
public class PostgresDataReader : IDataReader
{
    private string connectionString = "Host=localhost;Username=postgres;Password=code1234;Database=city";

    public List<Vector3> GetVertices()
    {
        List<Vector3> vertices = new List<Vector3>();

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("SELECT x_coord, y_coord FROM Vertices", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    float x = (float)reader.GetDouble(0);
                    float y = (float)reader.GetDouble(1);
                    float z = 0; // Пусть Z остаётся 0, если его нет в БД
                    vertices.Add(new Vector3(x, y, z));
                }
            }
        }

        return vertices;
    }


    public List<(int, int)> GetEdges()
    {
        List<(int, int)> edges = new List<(int, int)>();
        using (var conn = new NpgsqlConnection(connectionString))
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

// Основной генератор карты
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject islandPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private Transform mapParent;
    private IDataReader dataReader;

    private List<Vector3> vertices = new List<Vector3>();
    private List<(int, int)> edges = new List<(int, int)>();
    public void GenerateMapFromEditor()
    {
        LoadData();
        GenerateMap();
        Debug.Log("Map generated from editor!");
    }

    void Start()
    {
        dataReader = new PostgresDataReader(); // Подключаемся к БД
        LoadData();
    }

    private void LoadData()
    {
        vertices = dataReader.GetVertices();
        edges = dataReader.GetEdges();

        Debug.Log($"Loaded Vertices: {vertices.Count}, Edges: {edges.Count}");

        foreach (var vertex in vertices)
        {
            Debug.Log($"Vertex: {vertex}");
        }

        foreach (var edge in edges)
        {
            Debug.Log($"Edge: {edge.Item1} -> {edge.Item2}");
        }
    }


    private void GenerateMap()
    {
        if (mapParent == null)
        {
            Debug.LogError("mapParent is not assigned! Assigning a new empty GameObject.");
            mapParent = new GameObject("MapParent").transform;
        }
        if (vertices == null || vertices.Count == 0)
        {
            Debug.LogError("❌ Ошибка: Список vertices пуст! Убедитесь, что данные загружаются.");
            return;
        }

        if (edges == null || edges.Count == 0)
        {
            Debug.LogWarning("⚠️ Предупреждение: Список edges пуст! Граф может быть неполным.");
        }

        Debug.Log("GenerateMap called");

        // Очистка текущей карты
        foreach (Transform child in mapParent)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("Children cleared");

        // Создание островов
        for (int i = 0; i < vertices.Count; i++)
        {
            GameObject island = Instantiate(islandPrefab, vertices[i], Quaternion.identity, mapParent);
            island.name = $"Island {i}";
            Debug.Log($"Island {i} created at {vertices[i]}");
        }

        // Создание дорог
        foreach (var edge in edges)
        {
            if (edge.Item1 < 0 || edge.Item1 >= vertices.Count || edge.Item2 < 0 || edge.Item2 >= vertices.Count)
            {
                Debug.LogError($"❌ Ошибка: Некорректный индекс рёбер ({edge.Item1}, {edge.Item2})");
                Debug.LogError("Количество вершин: ");
                Debug.LogError(vertices.Count);
                continue;
            }

            Vector3 start = vertices[edge.Item1];
            Vector3 end = vertices[edge.Item2];
            Vector3 position = (start + end) / 2;
            Vector3 direction = end - start;

            GameObject road = Instantiate(roadPrefab, position, Quaternion.identity, mapParent);
            road.transform.rotation = Quaternion.LookRotation(direction);
            road.transform.localScale = new Vector3(0.2f, 0.2f, direction.magnitude);

            Debug.Log($"Road created between {start} and {end}");
        }

        Debug.Log("GenerateMap completed");
    }


}
