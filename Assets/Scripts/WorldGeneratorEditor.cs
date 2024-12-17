using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // ������ ������������ ����������
        DrawDefaultInspector();

        WorldGenerator generator = (WorldGenerator)target;

        // ������ ��� ��������� ������
        if (GUILayout.Button("Generate"))
        {
            generator.GenerateBuildings();
        }

        // ������ ��� �������� ������
        if (GUILayout.Button("Remove"))
        {
            generator.RemoveGeneratedObjects();
        }
    }
}
