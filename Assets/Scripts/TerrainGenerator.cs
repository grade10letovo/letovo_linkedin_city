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
}
