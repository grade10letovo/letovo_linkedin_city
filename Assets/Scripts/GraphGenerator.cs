using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : MonoBehaviour
{
    [SerializeField] private CityGraphData cityGraphData;
    [SerializeField] private GameObject islandPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private Transform mapParent;

    public void GenerateGraph()
    {
        if (cityGraphData == null)
        {
            Debug.LogError("CityGraphData is missing!");
            return;
        }

        List<Vector3> vertices = cityGraphData.Vertices;
        List<(int, int)> edges = cityGraphData.Edges;

        // Clear previous map
        foreach (Transform child in mapParent)
        {
            DestroyImmediate(child.gameObject);
        }

        // Instantiate islands
        for (int i = 0; i < vertices.Count; i++)
        {
            Instantiate(islandPrefab, vertices[i], Quaternion.identity, mapParent).name = $"Island {i}";
        }

        // Create roads
        foreach (var edge in edges)
        {
            CreateRoad(vertices[edge.Item1], vertices[edge.Item2]);
        }

        Debug.Log("Graph generated in EditMode.");
    }

    private void CreateRoad(Vector3 start, Vector3 end)
    {
        if (roadPrefab == null || islandPrefab == null)
        {
            Debug.LogError("Missing Prefabs!");
            return;
        }

        // Получаем коллайдер острова
        BoxCollider islandCollider = islandPrefab.GetComponent<BoxCollider>();
        float offset = 0f;

        if (islandCollider != null)
        {
            // Берём половину диагонали XY (плоскость XZ для плоской карты)
            Vector3 colliderSize = Vector3.Scale(islandCollider.size, islandPrefab.transform.localScale);
            offset = Mathf.Max(colliderSize.x, colliderSize.z) / 2f;
        }
        else
        {
            Debug.LogWarning("No BoxCollider found on IslandPrefab, using zero offset.");
        }

        Vector3 direction = (end - start).normalized;

        // Смещаем точки от центра в сторону края острова
        Vector3 adjustedStart = start + direction * offset;
        Vector3 adjustedEnd = end - direction * offset;

        GameObject road = Instantiate(roadPrefab, mapParent);
        road.transform.position = (adjustedStart + adjustedEnd) / 2f;
        road.transform.rotation = Quaternion.LookRotation(adjustedEnd - adjustedStart);

        float distance = Vector3.Distance(adjustedStart, adjustedEnd);

        road.transform.localScale = new Vector3(
            road.transform.localScale.x,
            road.transform.localScale.y,
            distance
        );
    }


}
