using UnityEditor;
using UnityEngine;

public class MapGeneratorEditor : EditorWindow
{
    private MapGenerator mapGenerator;

    [MenuItem("Tools/Map Generator")]
    public static void ShowWindow()
    {
        GetWindow<MapGeneratorEditor>("Map Generator");
    }

    private void OnEnable()
    {
        if (mapGenerator == null)
        {
            mapGenerator = FindObjectOfType<MapGenerator>();
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Map Generator", EditorStyles.boldLabel);

        if (mapGenerator == null)
        {
            EditorGUILayout.HelpBox("MapGenerator не найден в сцене!", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Generate Map from Database"))
        {
            mapGenerator.GenerateMapFromEditor(); // Вызов генерации
        }
    }
}
