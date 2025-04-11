using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabPlatformGenerator))]
[CanEditMultipleObjects]
public class PrefabPlatformGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Рисуем стандартный интерфейс инспектора
        DrawDefaultInspector();

        // Если выбрано несколько объектов, уведомляем пользователя
        if (targets.Length > 1)
        {
            EditorGUILayout.HelpBox("Мульти-редактирование не поддерживается для генерации платформ. Выберите только один объект.", MessageType.Warning);
            return;
        }

        // Если выбран один объект, добавляем кнопку для генерации платформ
        if (GUILayout.Button("Generate Platforms"))
        {
            // Получаем ссылку на единственный выбранный экземпляр
            PrefabPlatformGenerator generator = (PrefabPlatformGenerator)target;
            generator.GeneratePlatformsForPrefabs();
            EditorUtility.SetDirty(generator);
        }
    }
}
