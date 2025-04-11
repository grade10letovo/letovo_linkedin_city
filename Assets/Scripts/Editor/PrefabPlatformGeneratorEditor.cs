using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabPlatformGenerator))]
[CanEditMultipleObjects]
public class PrefabPlatformGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // ������ ����������� ��������� ����������
        DrawDefaultInspector();

        // ���� ������� ��������� ��������, ���������� ������������
        if (targets.Length > 1)
        {
            EditorGUILayout.HelpBox("������-�������������� �� �������������� ��� ��������� ��������. �������� ������ ���� ������.", MessageType.Warning);
            return;
        }

        // ���� ������ ���� ������, ��������� ������ ��� ��������� ��������
        if (GUILayout.Button("Generate Platforms"))
        {
            // �������� ������ �� ������������ ��������� ���������
            PrefabPlatformGenerator generator = (PrefabPlatformGenerator)target;
            generator.GeneratePlatformsForPrefabs();
            EditorUtility.SetDirty(generator);
        }
    }
}
