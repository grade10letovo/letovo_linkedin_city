using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GraphGenerator))]
public class EditorGraphGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GraphGenerator graphGenerator = (GraphGenerator)target;
        if (GUILayout.Button("Generate Graph"))
        {
            graphGenerator.GenerateGraph();
        }
    }
}
