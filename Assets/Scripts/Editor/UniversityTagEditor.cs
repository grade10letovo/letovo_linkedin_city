using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(UniversityTag))]
public class UniversityTagEditor : Editor
{
    private string[] universityOptions;
    private int selectedIndex = 0;

    void OnEnable()
    {
        // Загружаем JSON
        TextAsset jsonAsset = Resources.Load<TextAsset>("alumnus");
        if (jsonAsset != null)
        {
            string wrappedJson = "{\"alumni\":" + jsonAsset.text + "}";
            AlumnusListWrapper wrapper = JsonUtility.FromJson<AlumnusListWrapper>(wrappedJson);
            universityOptions = wrapper.alumni
                .Select(a => a.univ)
                .Where(u => !string.IsNullOrEmpty(u))
                .Distinct()
                .OrderBy(u => u)
                .ToArray();
        }
    }

    public override void OnInspectorGUI()
    {
        UniversityTag tag = (UniversityTag)target;

        if (universityOptions != null && universityOptions.Length > 0)
        {
            selectedIndex = System.Array.IndexOf(universityOptions, tag.universityName);
            if (selectedIndex < 0) selectedIndex = 0;

            selectedIndex = EditorGUILayout.Popup("Университет", selectedIndex, universityOptions);
            tag.universityName = universityOptions[selectedIndex];
        }
        else
        {
            EditorGUILayout.HelpBox("Не удалось загрузить список университетов. Убедись, что alumnus.json в Resources.", MessageType.Warning);
            tag.universityName = EditorGUILayout.TextField("Университет", tag.universityName);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
