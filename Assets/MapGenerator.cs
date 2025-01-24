using UnityEngine;
using System.Collections.Generic;
using System.IO;

// Интерфейс для работы с источником данных
public interface IDataReader
{
    List<Vector3> GetVertices();
    List<(int, int)> GetEdges();
}

// Mock-реализация источника данных
public class MockDataReader : IDataReader
{
    public List<Vector3> GetVertices()
    {
        return new List<Vector3>
        {
            new Vector3(-5, 0, -5),
            new Vector3(5, 0, -5),
            new Vector3(0, 0, 5)
        };
    }

    public List<(int, int)> GetEdges()
    {
        return new List<(int, int)>
        {
            (0, 1),
            (1, 2),
            (2, 0)
        };
    }
}

// Чтение данных из файла (пока файл не готов, используем MockDataReader)
public class FileDataReader : IDataReader
{
    private string filePath;

    public FileDataReader(string filePath)
    {
        this.filePath = filePath;
    }

    public List<Vector3> GetVertices()
    {
        List<Vector3> vertices = new List<Vector3>();
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            if (line.StartsWith("v")) // Строки вершин начинаются с "v"
            {
                string[] parts = line.Split(',');
                float x = float.Parse(parts[1]);
                float y = float.Parse(parts[2]);
                float z = float.Parse(parts[3]);
                vertices.Add(new Vector3(x, y, z));
            }
        }

        return vertices;
    }

    public List<(int, int)> GetEdges()
    {
        List<(int, int)> edges = new List<(int, int)>();
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            if (line.StartsWith("e")) // Строки рёбер начинаются с "e"
            {
                string[] parts = line.Split(',');
                int start = int.Parse(parts[1]);
                int end = int.Parse(parts[2]);
                edges.Add((start, end));
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
    private IDataReader dataReader; // Источник данных

    private List<Vector3> vertices = new List<Vector3>();
    private List<(int, int)> edges = new List<(int, int)>();

    void Start()
    {
        // Используем mock-данные. Позже заменим на FileDataReader, когда файл будет готов.
        dataReader = new MockDataReader();

        // Загрузка данных
        LoadData();

        // Подключение кнопки
        var generateButton = GameObject.Find("GenerateButton").GetComponent<UnityEngine.UI.Button>();
        generateButton.onClick.AddListener(GenerateMap);
    }

    private void LoadData()
    {
        vertices = dataReader.GetVertices();
        edges = dataReader.GetEdges();

        Debug.Log($"Loaded Vertices: {vertices.Count}, Edges: {edges.Count}");
    }

    private void GenerateMap()
    {
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
