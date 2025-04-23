#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldSaveManager))]
public class WorldSaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var worldSaveManager = (WorldSaveManager)target;

        if (GUILayout.Button("Save World to Server"))
        {
            worldSaveManager.SaveWorld();
        }
    }
}
#endif
