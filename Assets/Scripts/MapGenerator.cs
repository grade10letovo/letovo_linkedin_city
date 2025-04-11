using UnityEngine;
using System.Collections.Generic;
using Npgsql;
using UnityEngine.TerrainUtils;

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

    [Header("Flat Generator")]
    [SerializeField] private int numberOfFlats;
    [SerializeField] private Vector2 minSize;
    [SerializeField] private Vector2 maxSize;

    [Header("University Generator")]
    [SerializeField] private GameObject universityPrefab;

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
                GenerateRandomPlatforms(terrainObj.GetComponent<Terrain>(), 5, new Vector2(10, 10), new Vector2(100, 100));
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
    /// <summary>
    /// Генерирует случайное размещение площадок на террейне.
    /// </summary>
    /// <param name="terrain">Объект Terrain, на котором будут создаваться площадки.</param>
    /// <param name="platformCount">Желаемое количество площадок.</param>
    /// <param name="minPlatformSize">Минимальный размер площадки (ширина по оси X и длина по оси Z).</param>
    /// <param name="maxPlatformSize">Максимальный размер площадки (ширина по оси X и длина по оси Z).</param>
    /// <param name="smoothingRadius">Радиус (в точках heightmap) для сглаживания перехода между площадкой и окружающим террейном.</param>
    /// <param name="maxAttempts">Максимальное число попыток для расстановки (чтобы избежать бесконечного цикла, если площадки не могут быть расставлены без пересечений).</param>
    public static void GenerateRandomPlatforms(Terrain terrain, int platformCount, Vector2 minPlatformSize, Vector2 maxPlatformSize, int smoothingRadius = 5, int maxAttempts = 100)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;

        // Для хранения уже расставленных площадок (в мировых координатах)
        List<Rect> placedPlatforms = new List<Rect>();

        int createdPlatforms = 0;
        int attempts = 0;

        // Пока не создано нужное число площадок или пока не превышены максимум попыток
        while (createdPlatforms < platformCount && attempts < maxAttempts)
        {
            attempts++;

            // Случайным образом выбираем размер площадки в заданном диапазоне
            float platformWidth = Random.Range(minPlatformSize.x, maxPlatformSize.x);
            float platformLength = Random.Range(minPlatformSize.y, maxPlatformSize.y);

            // Чтобы площадка полностью помещалась внутри террейна,
            // генерируем случайный центр так, чтобы отступ от границ не меньше половины размера площадки.
            float minX = terrainPos.x + platformWidth / 2f;
            float maxX = terrainPos.x + terrainWidth - platformWidth / 2f;
            float minZ = terrainPos.z + platformLength / 2f;
            float maxZ = terrainPos.z + terrainLength - platformLength / 2f;

            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 platformCenter = new Vector3(randomX, 0f, randomZ);

            // Представляем площадку как прямоугольник, где:
            // - x: левая граница (randomX - platformWidth/2),
            // - y: нижняя граница (randomZ - platformLength/2),
            // - width: platformWidth,
            // - height: platformLength.
            Rect newPlatformRect = new Rect(randomX - platformWidth / 2f, randomZ - platformLength / 2f, platformWidth, platformLength);

            // Проверка на пересечения с уже созданными площадками
            bool overlaps = false;
            foreach (Rect placed in placedPlatforms)
            {
                if (newPlatformRect.Overlaps(placed))
                {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps)
            {
                // Если перекрытие обнаружено, пропускаем эту итерацию
                continue;
            }

            // Если площадка не пересекается с другими, создаём её с помощью функции CreateFlatPlatform.
            // Функция сама определит среднее значение высоты выбранной области и произведёт сглаживание.
            TerrainGenerator.CreateFlatPlatform(terrain, platformCenter, new Vector2(platformWidth, platformLength), smoothingRadius);

            // Добавляем прямоугольник новой площадки для дальнейшей проверки наложений.
            placedPlatforms.Add(newPlatformRect);
            createdPlatforms++;
        }

        if (createdPlatforms < platformCount)
        {
            Debug.LogWarning($"Было создано только {createdPlatforms} площадок из {platformCount} после {attempts} попыток.");
        }
        else
        {
            Debug.Log($"Успешно размещено {createdPlatforms} площадок.");
        }
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