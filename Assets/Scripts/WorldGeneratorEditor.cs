using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Рендер стандартного инспектора
        DrawDefaultInspector();

        WorldGenerator generator = (WorldGenerator)target;

        // Кнопка для генерации зданий
        if (GUILayout.Button("Generate"))
        {
            generator.GenerateBuildings();
        }

        // Кнопка для удаления зданий
        if (GUILayout.Button("Remove"))
        {
            generator.RemoveGeneratedObjects();
        }
    }
}
