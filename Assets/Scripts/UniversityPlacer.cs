using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UniversityPlacer : MonoBehaviour
{
    [SerializeField] private GameObject universityPrefab;
    [SerializeField] private float circleRadius = 1.0f;
    [SerializeField] private float heightOffset = 0.1f;
    [SerializeField] private string graphJsonPath;
    [SerializeField] private Transform mapParent;

    [System.Serializable]
    private class NodeData
    {
        public string nodeName;
        public string nodeID;
        public float x_position;
        public float y_position;
        public List<string> universities;
    }

    [System.Serializable]
    private class GraphData
    {
        public List<NodeData> nodes = new List<NodeData>();
    }

    public void PlaceUniversities()
    {
        if (string.IsNullOrEmpty(graphJsonPath))
        {
            Debug.LogError("Please specify the path to graph JSON file");
            return;
        }

        if (!File.Exists(graphJsonPath))
        {
            Debug.LogError($"Graph JSON file not found at: {graphJsonPath}");
            return;
        }

        if (universityPrefab == null)
        {
            Debug.LogError("University prefab is not assigned");
            return;
        }

        if (mapParent == null)
        {
            mapParent = GameObject.Find("MapParent")?.transform;
            if (mapParent == null)
            {
                Debug.LogError("MapParent not found in scene");
                return;
            }
        }

        string json = File.ReadAllText(graphJsonPath);
        GraphData graphData = JsonConvert.DeserializeObject<GraphData>(json);

        if (graphData == null || graphData.nodes == null || graphData.nodes.Count == 0)
        {
            Debug.LogError("Invalid or empty graph data");
            return;
        }

        Debug.Log($"Loaded graph with {graphData.nodes.Count} nodes");

        for (int i = 0; i < mapParent.childCount; i++)
        {
            Transform islandTransform = mapParent.GetChild(i);
            if (!islandTransform.name.StartsWith("Island "))
                continue;

            string indexStr = islandTransform.name.Substring(7);
            if (int.TryParse(indexStr, out int index) && index < graphData.nodes.Count)
            {
                NodeData nodeData = graphData.nodes[index];

                if (nodeData.universities != null && nodeData.universities.Count > 0)
                {
                    PlaceUniversitiesOnIsland(islandTransform.gameObject, nodeData.universities);
                }
            }
        }
    }

    private void PlaceUniversitiesOnIsland(GameObject island, List<string> universities)
    {
        // Удаляем существующие университеты
        for (int i = island.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = island.transform.GetChild(i);
            if (child.name.StartsWith("University_"))
            {
                DestroyImmediate(child.gameObject);
            }
        }

        List<string> validUniversities = new List<string>();
        foreach (var uni in universities)
        {
            if (uni != "Not selected")
                validUniversities.Add(uni);
        }

        if (validUniversities.Count == 0)
            return;

        float angleStep = 360f / validUniversities.Count;

        for (int i = 0; i < validUniversities.Count; i++)
        {
            float angle = i * angleStep;

            Vector3 position = island.transform.position;
            position.x += circleRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
            position.z += circleRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
            position.y += heightOffset;

            GameObject universityObj = Instantiate(universityPrefab, position, Quaternion.identity);
            universityObj.name = $"University_{validUniversities[i]}";
            universityObj.transform.SetParent(island.transform);

            Vector3 dirToCenter = island.transform.position - position;
            dirToCenter.y = 0;
            if (dirToCenter != Vector3.zero)
            {
                universityObj.transform.rotation = Quaternion.LookRotation(dirToCenter);
            }
        }

        Debug.Log($"Placed {validUniversities.Count} universities on {island.name}");
    }
}

[CustomEditor(typeof(UniversityPlacer))]
public class UniversityPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UniversityPlacer placer = (UniversityPlacer)target;

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Select a JSON file that contains university data from GraphEditor", MessageType.Info);

        if (GUILayout.Button("Browse JSON File"))
        {
            string path = EditorUtility.OpenFilePanel("Select Graph JSON", "", "json");
            if (!string.IsNullOrEmpty(path))
            {
                SerializedProperty graphJsonPathProp = serializedObject.FindProperty("graphJsonPath");
                graphJsonPathProp.stringValue = path;
                serializedObject.ApplyModifiedProperties();
            }
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Place Universities"))
        {
            placer.PlaceUniversities();
        }
    }
}
