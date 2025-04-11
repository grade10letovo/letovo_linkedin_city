using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    [Tooltip("Количество выборок по одной стороне heightmap")]
    public int terrainResolution = 256;

    [Tooltip("Размер террейна по осям X и Z")]
    public float terrainSize = 1000f;

    [Tooltip("Максимальная высота террейна")]
    public float terrainHeight = 100f;

    [Tooltip("Скаляр, определяющий «частоту» шума Перлина")]
    public float noiseScale = 0.01f;

    [Tooltip("Текстура террейна")]
    public TerrainLayer terrainLayer;

    /// <summary>
    /// Создаёт и возвращает GameObject террейна с применённым шумом Перлина.
    /// </summary>
    public GameObject GenerateTerrain()
    {
        // Если нужны дополнительные настройки, их можно установить:
        // newLayer.tileOffset = new Vector2(0, 0);
        // newLayer.specular = ... (если используете соответствующую конфигурацию материала)
        // и т.д.
        // Создаём новый TerrainData
        TerrainData terrainData = new TerrainData
        {
            heightmapResolution = terrainResolution,
            size = new Vector3(terrainSize, terrainHeight, terrainSize)
        };

        // Генерируем карту высот с использованием шума Перлина
        float[,] heights = new float[terrainResolution, terrainResolution];
        for (int z = 0; z < terrainResolution; z++)
        {
            for (int x = 0; x < terrainResolution; x++)
            {
                float noiseX = x * noiseScale;
                float noiseZ = z * noiseScale;
                float perlinValue = Mathf.PerlinNoise(noiseX, noiseZ);
                heights[z, x] = perlinValue; // Значение 0..1
            }
        }

        // Применяем сгенерированные высоты к TerrainData
        terrainData.SetHeights(0, 0, heights);
        // Получаем текущие слои и добавляем новый
        List<TerrainLayer> layers = new List<TerrainLayer>(terrainData.terrainLayers);
        layers.Add(terrainLayer);

        // Назначаем обновлённый массив слоев обратно в TerrainData
        terrainData.terrainLayers = layers.ToArray();

        // Создаём в сцене сам объект террейна
        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainObject.name = "ProceduralTerrain";

        // Возвращаем созданный объект на случай, если MapGenerator захочет с ним что-то ещё делать
        return terrainObject;
    }
    /// <summary>
    /// Создаёт ровную площадку на террейне с плавным переходом к окружающему ландшафту.
    /// </summary>
    /// <param name="terrain">Terrain, на котором нужно создать площадку.</param>
    /// <param name="platformCenter">Мировая координата центра площадки (используются X и Z).</param>
    /// <param name="platformSize">Размер площадки в мировых единицах (X — ширина, Y — глубина).</param>
    /// <param name="smoothingRadius">Радиус (в точках heightmap), задающий область сглаживания вокруг площадки.</param>
    public static void CreateFlatPlatform(Terrain terrain, Vector3 platformCenter, Vector2 platformSize, int smoothingRadius = 5)
    {
        TerrainData terrainData = terrain.terrainData;
        int hmWidth = terrainData.heightmapResolution;
        int hmHeight = terrainData.heightmapResolution;
        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;

        // Получаем текущую карту высот
        float[,] heights = terrainData.GetHeights(0, 0, hmWidth, hmHeight);

        // Позиция террейна в мировых координатах (левый нижний угол)
        Vector3 terrainPos = terrain.transform.position;

        // Преобразуем мировую позицию центра площадки в координаты heightmap (индексы)
        float relativeX = (platformCenter.x - terrainPos.x) / terrainWidth;
        float relativeZ = (platformCenter.z - terrainPos.z) / terrainLength;
        int centerX = Mathf.RoundToInt(relativeX * (hmWidth - 1));
        int centerZ = Mathf.RoundToInt(relativeZ * (hmHeight - 1));

        // Определяем, сколько точек heightmap занимают заданный размер площадки
        int platformWidthPoints = Mathf.RoundToInt(platformSize.x / terrainWidth * (hmWidth - 1));
        int platformLengthPoints = Mathf.RoundToInt(platformSize.y / terrainLength * (hmHeight - 1));

        // Вычисляем границы площадки (не забываем clamping, чтобы не выйти за границы массива)
        int startX = Mathf.Clamp(centerX - platformWidthPoints / 2, 0, hmWidth - 1);
        int endX = Mathf.Clamp(centerX + platformWidthPoints / 2, 0, hmWidth - 1);
        int startZ = Mathf.Clamp(centerZ - platformLengthPoints / 2, 0, hmHeight - 1);
        int endZ = Mathf.Clamp(centerZ + platformLengthPoints / 2, 0, hmHeight - 1);

        // Вычисляем среднее значение высоты в пределах площадки
        float sum = 0f;
        int count = 0;
        for (int z = startZ; z <= endZ; z++)
        {
            for (int x = startX; x <= endX; x++)
            {
                sum += heights[z, x];  // Обратите внимание: индекс сначала по Z, потом по X
                count++;
            }
        }
        float averageHeight = sum / count;

        // Устанавливаем ровную площадку: задаём всем точкам внутри площадки среднюю высоту
        for (int z = startZ; z <= endZ; z++)
        {
            for (int x = startX; x <= endX; x++)
            {
                heights[z, x] = averageHeight;
            }
        }

        // Создаём копию карты высот для сглаживания переходов (чтобы не влиять на исходные данные при итерации)
        float[,] newHeights = (float[,])heights.Clone();

        // Определяем область сглаживания (расширяем границы площадки на smoothingRadius)
        int smoothStartX = Mathf.Clamp(startX - smoothingRadius, 0, hmWidth - 1);
        int smoothEndX = Mathf.Clamp(endX + smoothingRadius, 0, hmWidth - 1);
        int smoothStartZ = Mathf.Clamp(startZ - smoothingRadius, 0, hmHeight - 1);
        int smoothEndZ = Mathf.Clamp(endZ + smoothingRadius, 0, hmHeight - 1);

        // Сглаживаем переход: для точек в зоне сглаживания (но за пределами площадки) вычисляем расстояние
        for (int z = smoothStartZ; z <= smoothEndZ; z++)
        {
            for (int x = smoothStartX; x <= smoothEndX; x++)
            {
                // Если точка принадлежит уже ровной площадке, пропускаем её
                if (x >= startX && x <= endX && z >= startZ && z <= endZ)
                    continue;

                // Вычисляем минимальное расстояние до края площадки по оси X
                int dx = 0;
                if (x < startX)
                    dx = startX - x;
                else if (x > endX)
                    dx = x - endX;

                // Аналогично по оси Z
                int dz = 0;
                if (z < startZ)
                    dz = startZ - z;
                else if (z > endZ)
                    dz = z - endZ;

                // Евклидовое расстояние (в единицах высотмапа)
                float distance = Mathf.Sqrt(dx * dx + dz * dz);

                // Если расстояние больше радиуса сглаживания, оставляем точку без изменений
                if (distance >= smoothingRadius)
                    continue;

                // Вычисляем коэффициент смешивания от 0 до 1 (чем ближе к площадке, тем больше влияние ровной высоты)
                float blendFactor = (smoothingRadius - distance) / smoothingRadius;

                // Смешиваем оригинальную высоту и высоту площадки с учётом коэффициента
                float originalHeight = heights[z, x];  // исходная высота (до изменения)
                newHeights[z, x] = Mathf.Lerp(originalHeight, averageHeight, blendFactor);
            }
        }

        // Применяем новые значения высот к террейну
        terrainData.SetHeights(0, 0, newHeights);
    }
}