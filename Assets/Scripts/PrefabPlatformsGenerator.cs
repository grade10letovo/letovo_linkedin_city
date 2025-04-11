using UnityEngine;
using System.Collections.Generic;
using UnityEngine.TerrainUtils;

public class PrefabPlatformGenerator : MonoBehaviour
{
    [Header("Ссылка на Terrain")]
    public Terrain terrain;

    [Header("Префабы для размещения на платформах")]
    public GameObject[] prefabs;

    [Header("Настройки платформ")]
    public Vector2 minPlatformSize = new Vector2(30f, 30f);
    public Vector2 maxPlatformSize = new Vector2(60f, 60f);
    [Tooltip("Радиус сглаживания перехода в точках heightmap")]
    public int smoothingRadius = 5;
    [Tooltip("Максимальное число попыток подобрать позицию для каждой площадки")]
    public int maxAttempts = 100;

    [Header("Опционально: родитель для созданных префабов")]
    public Transform platformsParent;

    // Список уже занятых площадок (для проверки на наложения)
    private List<Rect> placedPlatforms = new List<Rect>();

    // Метод генерации платформ и размещения префабов
    public void GeneratePlatformsForPrefabs()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain не задан!");
            return;
        }

        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("Список префабов пуст!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;

        placedPlatforms.Clear();

        foreach (GameObject prefab in prefabs)
        {
            bool placed = false;
            int attempts = 0;
            Vector3 platformCenter = Vector3.zero;
            Vector2 platformSize = Vector2.zero;
            Rect newPlatformRect = new Rect();

            while (!placed && attempts < maxAttempts)
            {
                attempts++;

                // Случайный размер площадки
                float platformWidth = Random.Range(minPlatformSize.x, maxPlatformSize.x);
                float platformLength = Random.Range(minPlatformSize.y, maxPlatformSize.y);
                platformSize = new Vector2(platformWidth, platformLength);

                // Ограничиваем область центра, чтобы площадка полностью помещалась на террейне
                float minX = terrainPos.x + platformWidth / 2f;
                float maxX = terrainPos.x + terrainWidth - platformWidth / 2f;
                float minZ = terrainPos.z + platformLength / 2f;
                float maxZ = terrainPos.z + terrainLength - platformLength / 2f;

                float randomX = Random.Range(minX, maxX);
                float randomZ = Random.Range(minZ, maxZ);
                platformCenter = new Vector3(randomX, 0f, randomZ);

                // Формируем прямоугольник площадки (координаты здесь – проекция на XZ)
                newPlatformRect = new Rect(randomX - platformWidth / 2f, randomZ - platformLength / 2f, platformWidth, platformLength);

                // Проверяем наложения с уже размещёнными площадками
                bool overlaps = false;
                foreach (Rect placedRect in placedPlatforms)
                {
                    if (newPlatformRect.Overlaps(placedRect))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    placed = true;
                }
            }

            if (!placed)
            {
                Debug.LogWarning($"Не удалось подобрать место для префаба {prefab.name} после {attempts} попыток.");
                continue;
            }

            // Создаём ровную платформу для данного префаба на выбранной позиции.
            // Функция TerrainUtility.CreateFlatPlatform подгонит высоту в области и выполнит сглаживание.
            TerrainGenerator.CreateFlatPlatform(terrain, platformCenter, platformSize, smoothingRadius);

            // Определяем окончательную высоту платформы (после изменения террейна)
            float platformHeight = terrain.SampleHeight(platformCenter);
            Vector3 prefabPosition = new Vector3(platformCenter.x, platformHeight, platformCenter.z);

            // Инстанциируем префаб. При необходимости задаём parent для удобства организации
            GameObject instance = Instantiate(prefab, prefabPosition, Quaternion.identity, platformsParent);
            instance.name = prefab.name + "_OnPlatform";

            // Запоминаем прямоугольник, чтобы последующие площадки не пересекались
            placedPlatforms.Add(newPlatformRect);
        }
    }
}
