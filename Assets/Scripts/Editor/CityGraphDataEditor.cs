using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CityGraphData))]
public class CityGraphDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // �������� ������ CityGraphData
        CityGraphData cityGraphData = (CityGraphData)target;

        // ���������� ������ ������
        EditorGUILayout.LabelField("Vertices", EditorStyles.boldLabel);
        for (int i = 0; i < cityGraphData.Vertices.Count; i++)
        {
            EditorGUILayout.Vector3Field($"Vertex {i}", cityGraphData.Vertices[i]);
        }

        // ���������� ������ ����
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Edges", EditorStyles.boldLabel);
        for (int i = 0; i < cityGraphData.Edges.Count; i++)
        {
            var edge = cityGraphData.Edges[i];
            EditorGUILayout.LabelField($"Edge {i}: {edge.Item1} -> {edge.Item2}");
        }

        // ���� �����, �������� ������ ��� �������������� ������
        if (GUILayout.Button("Refresh Graph"))
        {
            // ����� �������� ������ ��� ���������� ��� ����������� ������
            Debug.Log("Graph refreshed");
        }

        // ������ ����������� ��������� ��� ������ �����
        DrawDefaultInspector();
    }
}
