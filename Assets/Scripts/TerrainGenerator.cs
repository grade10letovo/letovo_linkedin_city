using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    [Tooltip("���������� ������� �� ����� ������� heightmap")]
    public int terrainResolution = 256;

    [Tooltip("������ �������� �� ���� X � Z")]
    public float terrainSize = 1000f;

    [Tooltip("������������ ������ ��������")]
    public float terrainHeight = 100f;

    [Tooltip("������, ������������ �������� ���� �������")]
    public float noiseScale = 0.01f;

    [Tooltip("�������� ��������")]
    public TerrainLayer terrainLayer;

    /// <summary>
    /// ������ � ���������� GameObject �������� � ���������� ����� �������.
    /// </summary>
    public GameObject GenerateTerrain()
    {
        // ���� ����� �������������� ���������, �� ����� ����������:
        // newLayer.tileOffset = new Vector2(0, 0);
        // newLayer.specular = ... (���� ����������� ��������������� ������������ ���������)
        // � �.�.
        // ������ ����� TerrainData
        TerrainData terrainData = new TerrainData
        {
            heightmapResolution = terrainResolution,
            size = new Vector3(terrainSize, terrainHeight, terrainSize)
        };

        // ���������� ����� ����� � �������������� ���� �������
        float[,] heights = new float[terrainResolution, terrainResolution];
        for (int z = 0; z < terrainResolution; z++)
        {
            for (int x = 0; x < terrainResolution; x++)
            {
                float noiseX = x * noiseScale;
                float noiseZ = z * noiseScale;
                float perlinValue = Mathf.PerlinNoise(noiseX, noiseZ);
                heights[z, x] = perlinValue; // �������� 0..1
            }
        }

        // ��������� ��������������� ������ � TerrainData
        terrainData.SetHeights(0, 0, heights);
        // �������� ������� ���� � ��������� �����
        List<TerrainLayer> layers = new List<TerrainLayer>(terrainData.terrainLayers);
        layers.Add(terrainLayer);

        // ��������� ���������� ������ ����� ������� � TerrainData
        terrainData.terrainLayers = layers.ToArray();

        // ������ � ����� ��� ������ ��������
        GameObject terrainObject = Terrain.CreateTerrainGameObject(terrainData);
        terrainObject.name = "ProceduralTerrain";

        // ���������� ��������� ������ �� ������, ���� MapGenerator ������� � ��� ���-�� ��� ������
        return terrainObject;
    }
    /// <summary>
    /// ������ ������ �������� �� �������� � ������� ��������� � ����������� ���������.
    /// </summary>
    /// <param name="terrain">Terrain, �� ������� ����� ������� ��������.</param>
    /// <param name="platformCenter">������� ���������� ������ �������� (������������ X � Z).</param>
    /// <param name="platformSize">������ �������� � ������� �������� (X � ������, Y � �������).</param>
    /// <param name="smoothingRadius">������ (� ������ heightmap), �������� ������� ����������� ������ ��������.</param>
    public static void CreateFlatPlatform(Terrain terrain, Vector3 platformCenter, Vector2 platformSize, int smoothingRadius = 5)
    {
        TerrainData terrainData = terrain.terrainData;
        int hmWidth = terrainData.heightmapResolution;
        int hmHeight = terrainData.heightmapResolution;
        float terrainWidth = terrainData.size.x;
        float terrainLength = terrainData.size.z;

        // �������� ������� ����� �����
        float[,] heights = terrainData.GetHeights(0, 0, hmWidth, hmHeight);

        // ������� �������� � ������� ����������� (����� ������ ����)
        Vector3 terrainPos = terrain.transform.position;

        // ����������� ������� ������� ������ �������� � ���������� heightmap (�������)
        float relativeX = (platformCenter.x - terrainPos.x) / terrainWidth;
        float relativeZ = (platformCenter.z - terrainPos.z) / terrainLength;
        int centerX = Mathf.RoundToInt(relativeX * (hmWidth - 1));
        int centerZ = Mathf.RoundToInt(relativeZ * (hmHeight - 1));

        // ����������, ������� ����� heightmap �������� �������� ������ ��������
        int platformWidthPoints = Mathf.RoundToInt(platformSize.x / terrainWidth * (hmWidth - 1));
        int platformLengthPoints = Mathf.RoundToInt(platformSize.y / terrainLength * (hmHeight - 1));

        // ��������� ������� �������� (�� �������� clamping, ����� �� ����� �� ������� �������)
        int startX = Mathf.Clamp(centerX - platformWidthPoints / 2, 0, hmWidth - 1);
        int endX = Mathf.Clamp(centerX + platformWidthPoints / 2, 0, hmWidth - 1);
        int startZ = Mathf.Clamp(centerZ - platformLengthPoints / 2, 0, hmHeight - 1);
        int endZ = Mathf.Clamp(centerZ + platformLengthPoints / 2, 0, hmHeight - 1);

        // ��������� ������� �������� ������ � �������� ��������
        float sum = 0f;
        int count = 0;
        for (int z = startZ; z <= endZ; z++)
        {
            for (int x = startX; x <= endX; x++)
            {
                sum += heights[z, x];  // �������� ��������: ������ ������� �� Z, ����� �� X
                count++;
            }
        }
        float averageHeight = sum / count;

        // ������������� ������ ��������: ����� ���� ������ ������ �������� ������� ������
        for (int z = startZ; z <= endZ; z++)
        {
            for (int x = startX; x <= endX; x++)
            {
                heights[z, x] = averageHeight;
            }
        }

        // ������ ����� ����� ����� ��� ����������� ��������� (����� �� ������ �� �������� ������ ��� ��������)
        float[,] newHeights = (float[,])heights.Clone();

        // ���������� ������� ����������� (��������� ������� �������� �� smoothingRadius)
        int smoothStartX = Mathf.Clamp(startX - smoothingRadius, 0, hmWidth - 1);
        int smoothEndX = Mathf.Clamp(endX + smoothingRadius, 0, hmWidth - 1);
        int smoothStartZ = Mathf.Clamp(startZ - smoothingRadius, 0, hmHeight - 1);
        int smoothEndZ = Mathf.Clamp(endZ + smoothingRadius, 0, hmHeight - 1);

        // ���������� �������: ��� ����� � ���� ����������� (�� �� ��������� ��������) ��������� ����������
        for (int z = smoothStartZ; z <= smoothEndZ; z++)
        {
            for (int x = smoothStartX; x <= smoothEndX; x++)
            {
                // ���� ����� ����������� ��� ������ ��������, ���������� �
                if (x >= startX && x <= endX && z >= startZ && z <= endZ)
                    continue;

                // ��������� ����������� ���������� �� ���� �������� �� ��� X
                int dx = 0;
                if (x < startX)
                    dx = startX - x;
                else if (x > endX)
                    dx = x - endX;

                // ���������� �� ��� Z
                int dz = 0;
                if (z < startZ)
                    dz = startZ - z;
                else if (z > endZ)
                    dz = z - endZ;

                // ���������� ���������� (� �������� ���������)
                float distance = Mathf.Sqrt(dx * dx + dz * dz);

                // ���� ���������� ������ ������� �����������, ��������� ����� ��� ���������
                if (distance >= smoothingRadius)
                    continue;

                // ��������� ����������� ���������� �� 0 �� 1 (��� ����� � ��������, ��� ������ ������� ������ ������)
                float blendFactor = (smoothingRadius - distance) / smoothingRadius;

                // ��������� ������������ ������ � ������ �������� � ������ ������������
                float originalHeight = heights[z, x];  // �������� ������ (�� ���������)
                newHeights[z, x] = Mathf.Lerp(originalHeight, averageHeight, blendFactor);
            }
        }

        // ��������� ����� �������� ����� � ��������
        terrainData.SetHeights(0, 0, newHeights);
    }
}