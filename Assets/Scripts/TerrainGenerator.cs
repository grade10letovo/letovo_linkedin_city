using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
    public Texture2D heightmap; // �������� ����� (���������)
    public Vector3 terrainSize = new Vector3(1000, 100, 1000); // ������ ��������
    public float waterLevel = 0.3f; // ������� ���� (0-1)
    public Material landMaterial; // �������� ��� ����
    public Material waterMaterial; // �������� ��� ����

    private GameObject terrainObject; // ������ �� �������
    private GameObject waterObject; // ������ �� ����

    public void GenerateTerrain()
    {
        if (heightmap == null)
        {
            Debug.LogError("Heightmap not set!");
            return;
        }

        // ������� ������ ������� � ����
        if (terrainObject != null)
            DestroyImmediate(terrainObject);

        if (waterObject != null)
            DestroyImmediate(waterObject);

        // ������� �������
        terrainObject = new GameObject("Generated Terrain");
        Terrain terrain = terrainObject.AddComponent<Terrain>();
        terrain.terrainData = new TerrainData();

        // ��������� ��������
        terrain.terrainData.size = terrainSize;

        // ��������� �����
        int resolution = heightmap.width; // ���������� �������� �����
        terrain.terrainData.heightmapResolution = resolution;

        float[,] heights = new float[resolution, resolution];
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                // �������� ������� ������� (0-1)
                float height = heightmap.GetPixel(x, y).grayscale;
                heights[x, y] = height * 100;
                Debug.Log(height);
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);

        // ������������� �������� ��� ����
        if (landMaterial != null)
        {
            terrain.materialTemplate = landMaterial;
        }

        // ������� ��������� ����
        CreateWater();
    }

    private void CreateWater()
    {
        waterObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        waterObject.name = "Generated Water";
        waterObject.transform.position = new Vector3(terrainSize.x / 2, waterLevel * terrainSize.y, terrainSize.z / 2);
        waterObject.transform.localScale = new Vector3(terrainSize.x / 10, 1, terrainSize.z / 10);

        // ������������� �������� ����
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
