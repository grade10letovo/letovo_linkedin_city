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
}
