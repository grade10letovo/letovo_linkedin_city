using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Globalization;

public class WorldGenerator : MonoBehaviour
{
    [Header("Settings")]
    public GameObject buildingPrefab; // ������ ������
    public float scaleFactor = 0.1f; // ������� ������
    public float textOffset = 5f; // ������ ������ ��� �������
    public float positionScale = 100f;

    [Header("Mock")]
    public bool mockAPI = false;
    public TextAsset csvFile;

    private List<GameObject> generatedObjects = new List<GameObject>(); // ������ ������ �� ��������� �������

    [Header("Dependencies")]
    public UniversityDataFetcher dataFetcher; // ������ �� ������ ���������� ������

    [ContextMenu("Generate Buildings")]
    public async void GenerateBuildings()
    {
        if (buildingPrefab == null)
        {
            Debug.LogError("Building prefab is not assigned!");
            return;
        }

        // ������� ������ ������� ����� ���������� �����
        RemoveGeneratedObjects();
        List<University> universities = new List<University>();
        if (mockAPI)
        {
            if (csvFile == null)
            {
                Debug.LogError("CSV file is not assigned!");
                return;
            }

            // ��������� ���� � ���������� ������
            var lines = csvFile.text.Split('\n'); // ������ ����� �� TextAsset
            for (int i = 1; i < lines.Length; i++) // ���������� ������ ������ (���������)
            {
                var line = lines[i].Trim(); // ������� ������ �������
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split('|');

                if (parts.Length < 3)
                {
                    Debug.LogWarning($"Skipping malformed line: {line}");
                    continue;
                }

                // ������ ������� �� ����� ��� ����������� �������� �����
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
            // ��������� ������ �� API
            universities = await dataFetcher.GetUniversitiesFromApi();
        }

        if (universities == null || universities.Count == 0)
        {
            Debug.LogError("No university data retrieved from the API.");
            return;
        }

        // ��������� ������ �� ������ ������
        foreach (var university in universities)
        {
            Vector3 position = new Vector3(university.latitude * positionScale, 0, university.longitude * positionScale);

            // ��������� ������
            GameObject building = PrefabUtility.InstantiatePrefab(buildingPrefab, transform) as GameObject;
            if (building != null)
            {
                building.transform.position = position;
                building.transform.localScale *= scaleFactor;
                building.name = university.name; // ��� ������������
                generatedObjects.Add(building);

                // �������� ������ � ��������� ������������
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

        // ��������� ��������� TextMesh
        TextMesh textMesh = textObject.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 50;
        textMesh.color = Color.white;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;

        // ��������� ����� ��� ��������� ������
        generatedObjects.Add(textObject);
    }
}
