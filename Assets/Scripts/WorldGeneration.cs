using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Globalization;

public class WorldGenerator : MonoBehaviour
{
    [Header("Settings")]
    public GameObject buildingPrefab; // Префаб здания
    public float scaleFactor = 0.1f; // Масштаб зданий
    public float textOffset = 5f; // Высота текста над зданием
    public float positionScale = 100f;

    [Header("Mock")]
    public bool mockAPI = false;
    public TextAsset csvFile;

    private List<GameObject> generatedObjects = new List<GameObject>(); // Хранит ссылки на созданные объекты

    [Header("Dependencies")]
    public UniversityDataFetcher dataFetcher; // Ссылка на скрипт загрузчика данных

    [ContextMenu("Generate Buildings")]
    public async void GenerateBuildings()
    {
        if (buildingPrefab == null)
        {
            Debug.LogError("Building prefab is not assigned!");
            return;
        }

        // Удаляем старые объекты перед генерацией новых
        RemoveGeneratedObjects();
        List<University> universities = new List<University>();
        if (mockAPI)
        {
            if (csvFile == null)
            {
                Debug.LogError("CSV file is not assigned!");
                return;
            }

            // Считываем файл и генерируем здания
            var lines = csvFile.text.Split('\n'); // Читаем текст из TextAsset
            for (int i = 1; i < lines.Length; i++) // Пропускаем первую строку (заголовки)
            {
                var line = lines[i].Trim(); // Убираем лишние пробелы
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split('|');

                if (parts.Length < 3)
                {
                    Debug.LogWarning($"Skipping malformed line: {line}");
                    continue;
                }

                // Замена запятых на точки для корректного парсинга чисел
                string latStr = parts[1].Replace('|', '.');
                string lonStr = parts[2].Replace('|', '.');
                Debug.Log(latStr + " " +  lonStr);
                universities.Add(new University() { latitude = float.Parse(latStr, CultureInfo.InvariantCulture), longitude = float.Parse(lonStr, CultureInfo.InvariantCulture), name = parts[0] });
            }
        }
        else
        {
            if (dataFetcher == null)
            {
                Debug.LogError("DataFetcher is not assigned!");
                return;
            }
            // Загружаем данные из API
            universities = await dataFetcher.GetUniversitiesFromApi();
        }

        if (universities == null || universities.Count == 0)
        {
            Debug.LogError("No university data retrieved from the API.");
            return;
        }

        // Генерация зданий на основе данных
        foreach (var university in universities)
        {
            Vector3 position = new Vector3(university.latitude * positionScale, 0, university.longitude * positionScale);

            // Генерация здания
            GameObject building = PrefabUtility.InstantiatePrefab(buildingPrefab, transform) as GameObject;
            if (building != null)
            {
                building.transform.position = position;
                building.transform.localScale *= scaleFactor;
                building.name = university.name; // Имя университета
                generatedObjects.Add(building);

                // Создание текста с названием университета
                CreateTextLabel(university.name, position, building);
            }
        }

        Debug.Log($"Generated {generatedObjects.Count} objects from the API data.");
    }

    [ContextMenu("Remove Generated Objects")]
    public void RemoveGeneratedObjects()
    {
        foreach (var obj in generatedObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }

        generatedObjects.Clear();
        Debug.Log("All generated objects have been removed.");
    }

    private void CreateTextLabel(string text, Vector3 position, GameObject building)
    {
        GameObject textObject = new GameObject(building.name + " text label");
        textObject.transform.position = position + new Vector3(0, textOffset, 0);
        textObject.transform.parent = building.transform;

        // Добавляем компонент TextMesh
        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 50;
        textMesh.color = Color.white;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;

        // Сохраняем текст как созданный объект
        generatedObjects.Add(textObject);
    }
}
