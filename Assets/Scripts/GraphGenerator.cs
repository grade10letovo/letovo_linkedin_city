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

        foreach (Transform child in mapParent)
        {
            DestroyImmediate(child.gameObject);
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            Instantiate(islandPrefab, vertices[i], Quaternion.identity, mapParent).name = $"Island {i}";
        }

        foreach (var edge in edges)
        {
            CreateRoad(vertices[edge.Item1], vertices[edge.Item2]);
        }

        Debug.Log("Graph generated in EditMode.");
    }

    private void CreateRoad(Vector3 start, Vector3 end)
    {
        GameObject road = Instantiate(roadPrefab, mapParent);
        road.transform.position = (start + end) / 2f;
        road.transform.rotation = Quaternion.LookRotation(end - start);
        road.transform.rotation = Quaternion.Euler(road.transform.eulerAngles + new Vector3(0, 45, 0));
        Debug.Log(islandPrefab.transform.Find("land autumn atmosphere forest").GetComponent<BoxCollider>().size);
        road.transform.localScale = new Vector3(road.transform.localScale.x, road.transform.localScale.y, (end - start + islandPrefab.transform.Find("land autumn atmosphere forest").GetComponent<BoxCollider>().size).magnitude);
    }
}
