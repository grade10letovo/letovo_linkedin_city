using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainGenerator generator = (TerrainGenerator)target;

        if (GUILayout.Button("Generate Terrain"))
        {
            generator.GenerateTerrain();
        }

        if (GUILayout.Button("Clear Terrain"))
        {
            generator.ClearTerrain();
        }
    }
}
