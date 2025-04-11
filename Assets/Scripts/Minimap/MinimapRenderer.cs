using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MiniMapRenderer : MonoBehaviour
{
    [SerializeField] private RectTransform miniMapRect;
    [SerializeField] private GameObject islandIconPrefab;
    [SerializeField] private GameObject playerIconPrefab;
    [SerializeField] private Transform player;

    private List<Vector3> vertices;
    private List<(int, int)> edges;
    private float minX, maxX, minZ, maxZ;

    private RectTransform playerIconRect;

    public void Init(List<Vector3> vertices, List<(int, int)> edges)
    {
        this.vertices = vertices;
        this.edges = edges;

        CalculateBounds();
        SpawnIslands();
        SpawnEdges();
        SpawnPlayerIcon(); // ������ ������ ������
    }

    private void CalculateBounds()
    {
        minX = float.MaxValue; maxX = float.MinValue;
        minZ = float.MaxValue; maxZ = float.MinValue;

        foreach (var v in vertices)
        {
            if (v.x < minX) minX = v.x;
            if (v.x > maxX) maxX = v.x;
            if (v.z < minZ) minZ = v.z;
            if (v.z > maxZ) maxZ = v.z;
        }
    }

    private void SpawnIslands()
    {
        foreach (var vertex in vertices)
        {
            Vector2 uiPos = WorldToMiniMap(vertex);
            var iconGO = Instantiate(islandIconPrefab, miniMapRect);
            iconGO.GetComponent<RectTransform>().anchoredPosition = uiPos;
        }
    }

    private void SpawnEdges()
    {
        // ������� �� ������ ������� �������� ����� � UI.
        // ����� ��������������� - �� ������ ��������� prefab + ���������� UILineRenderer
    }

    private void SpawnPlayerIcon()
    {
        var iconGO = Instantiate(playerIconPrefab, miniMapRect);
        playerIconRect = iconGO.GetComponent<RectTransform>();
        // ��������� �������
        playerIconRect.anchoredPosition = WorldToMiniMap(player.position);
    }

    private Vector2 WorldToMiniMap(Vector3 worldPos)
    {
        float width = miniMapRect.rect.width;
        float height = miniMapRect.rect.height;

        float u = (worldPos.x - minX) / (maxX - minX);
        float v = (worldPos.z - minZ) / (maxZ - minZ);
        v = 1f - v; // ���� ���� ������������� ���

        float uiX = u * width;
        float uiY = v * height;
        return new Vector2(uiX, uiY);
    }

    void LateUpdate()
    {
        if (playerIconRect != null && player != null)
        {
            // ��������� ������� ������ �� ���������
            playerIconRect.anchoredPosition = WorldToMiniMap(player.position);
            // ���� ���� �������
            // float angle = player.eulerAngles.y;
            // playerIconRect.localEulerAngles = new Vector3(0, 0, -angle);
        }
    }
}
