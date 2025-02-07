using UnityEngine;
using System.Collections.Generic;
using Npgsql;
using Unity.VisualScripting;

// Интерфейс для работы с источником данных
public interface IDataReader
{
    List<Vector3> GetVertices();
    List<(int, int)> GetEdges();
}

// Основной генератор карты
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject islandPrefab;
    [SerializeField] private GameObject roadPrefab;
    private Transform mapParent;
    private PostgresDataReader dataReader;

    private List<Vector3> vertices = new List<Vector3>();
    private List<(int, int)> edges = new List<(int, int)>();
    private string connectionString = "Host=localhost;Username=postgres;Password=code1234;Database=city";
    public void GenerateMapFromEditor()
    {
        dataReader = new PostgresDataReader(connectionString);
        LoadData(); 
        GenerateMap();
        Debug.Log("Map generated from editor!");
    }
    public List<Vector3> GetVertices()
    {
        string query = "SELECT x_coord, y_coord FROM Vertices";
        return dataReader.ExecuteQuery(query, reader =>
            new Vector3((float)reader.GetDouble(0), 0, (float)reader.GetDouble(1)));
    }

    public List<(int, int)> GetEdges()
    {
        string query = "SELECT start_vertex_id, end_vertex_id FROM Edges";
        return dataReader.ExecuteQuery(query, reader => (reader.GetInt32(0) - 1, reader.GetInt32(1) - 1));
    }

    private void LoadData()
    {
        vertices = GetVertices();
        edges = GetEdges();

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
        mapParent = transform;
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
