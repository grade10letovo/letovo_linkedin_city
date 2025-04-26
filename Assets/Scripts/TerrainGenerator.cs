using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
    public Texture2D heightmap; // Текстура высот (грейскейл)
    public Vector3 terrainSize = new Vector3(1000, 100, 1000); // Размер террейна
    public float waterLevel = 0.3f; // Уровень воды (0-1)
    public Material landMaterial; // Материал для суши
    public Material waterMaterial; // Материал для воды

    private GameObject terrainObject; // Ссылка на террейн
    private GameObject waterObject; // Ссылка на воду

    public void GenerateTerrain()
    {
        if (heightmap == null)
        {
            Debug.LogError("Heightmap not set!");
            return;
        }

        // Удаляем старый террейн и воду
        if (terrainObject != null)
            DestroyImmediate(terrainObject);

        if (waterObject != null)
            DestroyImmediate(waterObject);

        // Создаем террейн
        terrainObject = new GameObject("Generated Terrain");
        Terrain terrain = terrainObject.AddComponent<Terrain>();
        terrain.terrainData = new TerrainData();

        // Настройка размеров
        terrain.terrainData.size = terrainSize;

        // Генерация высот
        int resolution = heightmap.width; // Разрешение высотной карты
        terrain.terrainData.heightmapResolution = resolution;

        float[,] heights = new float[resolution, resolution];
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                // Получаем яркость пикселя (0-1)
                float height = heightmap.GetPixel(x, y).grayscale;
                heights[x, y] = height * 100;
                Debug.Log(height);
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);

        // Устанавливаем материал для суши
        if (landMaterial != null)
        {
            terrain.materialTemplate = landMaterial;
        }

        // Создаем плоскость воды
        CreateWater();
    }

    private void CreateWater()
    {
        waterObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        waterObject.name = "Generated Water";
        waterObject.transform.position = new Vector3(terrainSize.x / 2, waterLevel * terrainSize.y, terrainSize.z / 2);
        waterObject.transform.localScale = new Vector3(terrainSize.x / 10, 1, terrainSize.z / 10);

        // Устанавливаем материал воды
        if (waterMaterial != null)
        {
            waterObject.GetComponent<Renderer>().material = waterMaterial;
        }
    }

    public void ClearTerrain()
    {
        if (terrainObject != null)
            DestroyImmediate(terrainObject);

        if (waterObject != null)
            DestroyImmediate(waterObject);
    }
}
