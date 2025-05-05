using System.Collections.Generic;
using UnityEngine;
using Npgsql;

public class MinimapController : MonoBehaviour
{
    [Header("References")]
    public GameObject vertexPrefab;
    public GameObject edgePrefab;
    public RectTransform mapArea;
    public Transform playerTransform;
    public GameObject playerIconPrefab;
    private GameObject playerIconInstance;
    public GameObject minimapPanel;

    [Header("Database Settings")]
    private const string ConnectionString = "Host=localhost;Username=postgres;Password=code1234;Database=city";

    [Header("Map Settings")]
    public float zoomSpeed = 0.1f;
    public float minZoom = 0.5f;
    public float maxZoom = 2.0f;

    private Vector2 dragStartPos;
    private Vector2 mapStartPos;
    private bool dragging = false;

    private List<Vector2> vertices = new List<Vector2>();
    private List<Vector2Int> edges = new List<Vector2Int>();
    private bool isVisible = false;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    private Vector2 minCoord;
    private Vector2 maxCoord;

    private RectTransform panelRect;

    void Start()
    {
        panelRect = minimapPanel.GetComponent<RectTransform>();
        //LoadGraphDataFromDatabase();
        MockGraphData();
        HideMinimap();
        DrawMap();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("M");
            ToggleMinimap();
        }

        if (isVisible && playerIconInstance != null && playerTransform != null)
        {
            Vector2 playerWorldPos = new Vector2(playerTransform.position.x, playerTransform.position.z);
            Vector2 playerMapPos = WorldToMapPosition(playerWorldPos);
            playerIconInstance.GetComponent<RectTransform>().anchoredPosition = playerMapPos;
        }

        if (isVisible)
        {
            HandleDragging();
            HandleZooming();
        }
    }

    void ToggleMinimap()
    {
        isVisible = !isVisible;
        minimapPanel.gameObject.SetActive(isVisible);
    }

    void HideMinimap()
    {
        minimapPanel.gameObject.SetActive(false);
    }


    void LoadGraphDataFromDatabase()
    {
        vertices = new List<Vector2>();
        edges = new List<Vector2Int>();

        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand("SELECT x_coord, y_coord FROM Vertices", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    float x = (float)reader.GetDouble(0);
                    float z = (float)reader.GetDouble(1);
                    vertices.Add(new Vector2(x, z));
                }
            }

            using (var cmd = new NpgsqlCommand("SELECT start_vertex_id, end_vertex_id FROM Edges", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int start = reader.GetInt32(0) - 1; // Adjust for 0-based indexing
                    int end = reader.GetInt32(1) - 1;
                    edges.Add(new Vector2Int(start, end));
                }
            }
        }

        if (vertices.Count > 0)
        {
            minCoord = vertices[0];
            maxCoord = vertices[0];

            foreach (var v in vertices)
            {
                minCoord = Vector2.Min(minCoord, v);
                maxCoord = Vector2.Max(maxCoord, v);
            }
        }

        Debug.Log($"[Minimap] Loaded {vertices.Count} vertices and {edges.Count} edges from database!");
    }

    Vector2 WorldToMapPosition(Vector2 worldPos)
    {
        Vector2 size = maxCoord - minCoord;
        if (size.x == 0) size.x = 1;
        if (size.y == 0) size.y = 1;

        Vector2 normalized = (worldPos - minCoord);
        normalized.x /= size.x;
        normalized.y /= size.y;

        Vector2 mapSize = mapArea.rect.size;
        Vector2 pos = new Vector2(normalized.x * mapSize.x, normalized.y * mapSize.y);
        pos -= mapSize * 0.5f;
        return pos;
    }

    void MockGraphData()
    {
        vertices = new List<Vector2>
    {
        new Vector2( 0f,  0f),
        new Vector2(10f,  0f),
        new Vector2( 5f, 10f),
    };

        edges = new List<Vector2Int>
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 2),
        new Vector2Int(2, 0),
    };

        if (vertices.Count > 0)
        {
            minCoord = vertices[0];
            maxCoord = vertices[0];
            foreach (var v in vertices)
            {
                minCoord = Vector2.Min(minCoord, v);
                maxCoord = Vector2.Max(maxCoord, v);
            }
        }

        Debug.Log($"[Minimap] Mocked {vertices.Count} vertices and {edges.Count} edges.");
    }

    public void DrawMap()
    {
        foreach (var obj in spawnedObjects)
        {
            Destroy(obj);
        }
        spawnedObjects.Clear();

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 pos = vertices[i];
            GameObject v = Instantiate(vertexPrefab, mapArea);
            v.GetComponent<RectTransform>().anchoredPosition = WorldToMapPosition(pos);
            spawnedObjects.Add(v);
        }

        foreach (var edge in edges)
        {
            Vector2 p1 = WorldToMapPosition(vertices[edge.x]);
            Vector2 p2 = WorldToMapPosition(vertices[edge.y]);
            GameObject e = Instantiate(edgePrefab, mapArea);

            RectTransform r = e.GetComponent<RectTransform>();
            Vector2 dir = (p2 - p1).normalized;
            float distance = Vector2.Distance(p1, p2);

            r.sizeDelta = new Vector2(distance, 2);
            r.anchoredPosition = (p1 + p2) / 2f;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            r.rotation = Quaternion.Euler(0, 0, angle);

            spawnedObjects.Add(e);
        }

        if (playerIconInstance == null && playerTransform != null)
        {
            playerIconInstance = Instantiate(playerIconPrefab, mapArea);
        }
    }

    void HandleDragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                panelRect, Input.mousePosition, null, out dragStartPos);
            mapStartPos = mapArea.anchoredPosition;
            dragging = true;
        }
        if (Input.GetMouseButton(0) && dragging)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                panelRect, Input.mousePosition, null, out var currentMousePos);
            Vector2 diff = currentMousePos - dragStartPos;
            Vector2 targetPos = mapStartPos + diff;
            mapArea.anchoredPosition = ClampMapPosition(targetPos);
        }
        if (Input.GetMouseButtonUp(0))
            dragging = false;
    }

    void HandleZooming()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float newScale = Mathf.Clamp(mapArea.localScale.x + scroll * zoomSpeed, minZoom, maxZoom);
            mapArea.localScale = new Vector3(newScale, newScale, 1f);
            mapArea.anchoredPosition = ClampMapPosition(mapArea.anchoredPosition);
        }
    }

    private Vector2 ClampMapPosition(Vector2 targetPos, float overshoot = 50f)
    {
        Vector2 mapSize = mapArea.rect.size * mapArea.localScale.x;
        Vector2 panelSize = panelRect.rect.size;

        float halfMapW = mapSize.x * 0.5f;
        float halfMapH = mapSize.y * 0.5f;
        float halfPanelW = panelSize.x * 0.5f;
        float halfPanelH = panelSize.y * 0.5f;

        float minX = -halfMapW + halfPanelW;
        float maxX = halfMapW - halfPanelW;
        float minY = -halfMapH + halfPanelH;
        float maxY = halfMapH - halfPanelH;

        if (mapSize.x <= panelSize.x) minX = maxX = 0;
        if (mapSize.y <= panelSize.y) minY = maxY = 0;

        minX -= overshoot;
        maxX += overshoot;
        minY -= overshoot;
        maxY += overshoot;

        float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);
        return new Vector2(clampedX, clampedY);
    }
}