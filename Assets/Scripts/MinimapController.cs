using System.Collections.Generic;
using UnityEngine;
using TMPro;              // подключаем TextMeshPro
using UnityEngine.UI;
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

    [Header("Graph Data")]
    public MinimapGraphData graphData;  // ScriptableObject с данными

    [Header("Map Settings")]
    public float zoomSpeed = 0.1f;
    public float minZoom = 0.5f;
    public float maxZoom = 2.0f;

    private Vector2 dragStartPos, mapStartPos;
    private bool dragging = false, isVisible = false;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Vector2 minCoord, maxCoord;
    private RectTransform panelRect;

    void Start()
    {
        panelRect = minimapPanel.GetComponent<RectTransform>();
        InitializeCoords();
        HideMinimap();
        DrawMap();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) ToggleMinimap();

        if (isVisible && playerIconInstance != null && playerTransform != null)
            UpdatePlayerIcon();

        if (isVisible)
        {
            //HandleDragging();
            HandleZooming();
        }
    }

    void ToggleMinimap()
    {
        isVisible = !isVisible;
        minimapPanel.SetActive(isVisible);
    }

    void HideMinimap()
    {
        minimapPanel.SetActive(false);
    }

    void InitializeCoords()
    {
        if (graphData.vertices.Count == 0) return;
        minCoord = maxCoord = graphData.vertices[0].position;
        foreach (var v in graphData.vertices)
        {
            minCoord = Vector2.Min(minCoord, v.position);
            maxCoord = Vector2.Max(maxCoord, v.position);
        }
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
        return pos - (mapSize * 0.5f);
    }

    public void DrawMap()
    {
        // очистка
        foreach (var obj in spawnedObjects)
            Destroy(obj);
        spawnedObjects.Clear();

        // вершины
        for (int i = 0; i < graphData.vertices.Count; i++)
        {
            var vData = graphData.vertices[i];
            var vObj = Instantiate(vertexPrefab, mapArea);
            vObj.GetComponent<RectTransform>().anchoredPosition = WorldToMapPosition(vData.position);

            // теперь TMP_Text
            var label = vObj.GetComponentInChildren<TMP_Text>();
            if (label != null)
            {
                label.text = vData.vertexName;
                label.rectTransform.anchoredPosition += Vector2.up * 15f;
            }

            spawnedObjects.Add(vObj);
        }

        // рёбра
        foreach (var eData in graphData.edges)
        {
            Vector2 p1 = WorldToMapPosition(graphData.vertices[eData.startIndex].position);
            Vector2 p2 = WorldToMapPosition(graphData.vertices[eData.endIndex].position);
            var eObj = Instantiate(edgePrefab, mapArea);
            var r = eObj.GetComponent<RectTransform>();

            Vector2 dir = (p2 - p1).normalized;
            float dist = Vector2.Distance(p1, p2);
            r.sizeDelta = new Vector2(dist, 2f);
            r.anchoredPosition = (p1 + p2) / 2f;
            r.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

            spawnedObjects.Add(eObj);
        }

        // иконка игрока
        if (playerIconInstance == null && playerTransform != null)
            playerIconInstance = Instantiate(playerIconPrefab, mapArea);
    }

    void UpdatePlayerIcon()
    {
        Vector2 worldPos = new Vector2(playerTransform.position.x, playerTransform.position.z);
        playerIconInstance.GetComponent<RectTransform>().anchoredPosition = WorldToMapPosition(worldPos);
    }

    //void HandleDragging()
    //{
    //    // Нажатие ЛКМ
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //            panelRect, Input.mousePosition, null, out dragStartPos);

    //        Debug.Log($"[Minimap] MouseDown at screen {Input.mousePosition} → insidePanel: {inside}, dragStartPos: {dragStartPos}");

    //        if (inside)
    //        {
    //            mapStartPos = mapArea.anchoredPosition;
    //            dragging = true;
    //            Debug.Log($"[Minimap] Begin dragging. mapStartPos: {mapStartPos}");
    //        }
    //    }
    //    // Удержание ЛКМ
    //    else if (Input.GetMouseButton(0))
    //    {
    //        Debug.Log($"[Minimap] Mouse held. dragging={dragging}");
    //        if (dragging)
    //        {
    //            bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //                panelRect, Input.mousePosition, null, out Vector2 currentMousePos);

    //            Vector2 diff = currentMousePos - dragStartPos;
    //            Vector2 newAnchored = mapStartPos + diff;

    //            Debug.Log($"[Minimap] insidePanel: {inside}, currentMousePos: {currentMousePos}, diff: {diff}, newAnchored: {newAnchored}");

    //            mapArea.anchoredPosition = ClampMapPosition(newAnchored);
    //        }
    //    }
    //    // Отпускание ЛКМ
    //    else if (Input.GetMouseButtonUp(0))
    //    {
    //        Debug.Log($"[Minimap] MouseUp. dragging was {dragging}");
    //        dragging = false;
    //    }
    //}




    void HandleZooming()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float newScale = Mathf.Clamp(mapArea.localScale.x + scroll * zoomSpeed, minZoom, maxZoom);
            mapArea.localScale = Vector3.one * newScale;
            mapArea.anchoredPosition = ClampMapPosition(mapArea.anchoredPosition);
        }
    }

    Vector2 ClampMapPosition(Vector2 targetPos, float overshoot = 50f)
    {
        Vector2 mapSize = mapArea.rect.size * mapArea.localScale.x;
        Vector2 panelSize = panelRect.rect.size;

        float halfMapW = mapSize.x * 0.5f, halfMapH = mapSize.y * 0.5f;
        float halfPanelW = panelSize.x * 0.5f, halfPanelH = panelSize.y * 0.5f;

        float minX = -halfMapW + halfPanelW - overshoot;
        float maxX = halfMapW - halfPanelW + overshoot;
        float minY = -halfMapH + halfPanelH - overshoot;
        float maxY = halfMapH - halfPanelH + overshoot;

        if (mapSize.x <= panelSize.x) { minX = maxX = 0; }
        if (mapSize.y <= panelSize.y) { minY = maxY = 0; }

        return new Vector2(
            Mathf.Clamp(targetPos.x, minX, maxX),
            Mathf.Clamp(targetPos.y, minY, maxY)
        );
    }
}
