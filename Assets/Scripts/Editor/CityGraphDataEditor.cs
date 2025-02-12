using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CityGraphData))]
public class CityGraphDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Получаем объект CityGraphData
        CityGraphData cityGraphData = (CityGraphData)target;

        // Отображаем список вершин
        EditorGUILayout.LabelField("Vertices", EditorStyles.boldLabel);
        for (int i = 0; i < cityGraphData.Vertices.Count; i++)
        {
            EditorGUILayout.Vector3Field($"Vertex {i}", cityGraphData.Vertices[i]);
        }

        // Отображаем список рёбер
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Edges", EditorStyles.boldLabel);
        for (int i = 0; i < cityGraphData.Edges.Count; i++)
        {
            var edge = cityGraphData.Edges[i];
            EditorGUILayout.LabelField($"Edge {i}: {edge.Item1} -> {edge.Item2}");
        }

        // Если нужно, добавьте кнопку для редактирования данных
        if (GUILayout.Button("Refresh Graph"))
        {
            // Можно добавить логику для обновления или перерасчёта данных
            Debug.Log("Graph refreshed");
        }

        // Рисуем стандартный интерфейс для других полей
        DrawDefaultInspector();
    }
}
