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

    void Start()
    {
        LoadGraphDataFromDatabase();
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
        if (isVisible)
        {
            CenterMapOnPlayer();
        }
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
        return new Vector2(normalized.x * mapSize.x, normalized.y * mapSize.y);
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
                mapArea.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out dragStartPos);
            mapStartPos = mapArea.anchoredPosition;
            dragging = true;
        }
        if (Input.GetMouseButton(0) && dragging)
        {
            Vector2 currentMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mapArea.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out currentMousePos);

            Vector2 diff = currentMousePos - dragStartPos;
            Vector2 targetPos = mapStartPos + diff;
            mapArea.anchoredPosition = ClampMapPosition(targetPos);
        }
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
    }

    Vector2 ClampMapPosition(Vector2 pos)
    {
        RectTransform parentRect = mapArea.parent.GetComponent<RectTransform>();

        Vector2 mapSize = mapArea.rect.size * mapArea.localScale.x;
        Vector2 viewSize = parentRect.rect.size;

        Vector2 minPos = viewSize - mapSize;
        Vector2 maxPos = Vector2.zero;

        pos.x = Mathf.Clamp(pos.x, minPos.x, maxPos.x);
        pos.y = Mathf.Clamp(pos.y, minPos.y, maxPos.y);

        return pos;
    }

    void HandleZooming()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float newScale = Mathf.Clamp(mapArea.localScale.x + scroll * zoomSpeed, minZoom, maxZoom);
            mapArea.localScale = new Vector3(newScale, newScale, 1f);
        }
    }

    void CenterMapOnPlayer()
    {
        if (playerTransform == null) return;

        Vector2 playerWorldPos = new Vector2(playerTransform.position.x, playerTransform.position.z);
        Vector2 playerMapPos = WorldToMapPosition(playerWorldPos);

        Vector2 screenCenter = mapArea.parent.GetComponent<RectTransform>().rect.size / 2f;
        mapArea.anchoredPosition = screenCenter - playerMapPos;
    }
}