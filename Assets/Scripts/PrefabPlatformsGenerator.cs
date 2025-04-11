using UnityEngine;
using System.Collections.Generic;
using UnityEngine.TerrainUtils;

public class PrefabPlatformGenerator : MonoBehaviour
{
    [Header("������ �� Terrain")]
    public Terrain terrain;

    [Header("������� ��� ���������� �� ����������")]
    public GameObject[] prefabs;

    [Header("��������� ��������")]
    public Vector2 minPlatformSize = new Vector2(30f, 30f);
    public Vector2 maxPlatformSize = new Vector2(60f, 60f);
    [Tooltip("������ ����������� �������� � ������ heightmap")]
    public int smoothingRadius = 5;
    [Tooltip("������������ ����� ������� ��������� ������� ��� ������ ��������")]
    public int maxAttempts = 100;

    [Header("�����������: �������� ��� ��������� ��������")]
    public Transform platformsParent;

    // ������ ��� ������� �������� (��� �������� �� ���������)
    private List<Rect> placedPlatforms = new List<Rect>();

    // ����� ��������� �������� � ���������� ��������
    public void GeneratePlatformsForPrefabs()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain �� �����!");
            return;
        }

        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("������ �������� ����!");
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

                // ��������� ������ ��������
                float platformWidth = Random.Range(minPlatformSize.x, maxPlatformSize.x);
                float platformLength = Random.Range(minPlatformSize.y, maxPlatformSize.y);
                platformSize = new Vector2(platformWidth, platformLength);

                // ������������ ������� ������, ����� �������� ��������� ���������� �� ��������
                float minX = terrainPos.x + platformWidth / 2f;
                float maxX = terrainPos.x + terrainWidth - platformWidth / 2f;
                float minZ = terrainPos.z + platformLength / 2f;
                float maxZ = terrainPos.z + terrainLength - platformLength / 2f;

                float randomX = Random.Range(minX, maxX);
                float randomZ = Random.Range(minZ, maxZ);
                platformCenter = new Vector3(randomX, 0f, randomZ);

                // ��������� ������������� �������� (���������� ����� � �������� �� XZ)
                newPlatformRect = new Rect(randomX - platformWidth / 2f, randomZ - platformLength / 2f, platformWidth, platformLength);

                // ��������� ��������� � ��� ������������ ����������
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
                Debug.LogWarning($"�� ������� ��������� ����� ��� ������� {prefab.name} ����� {attempts} �������.");
                continue;
            }

            // ������ ������ ��������� ��� ������� ������� �� ��������� �������.
            // ������� TerrainUtility.CreateFlatPlatform �������� ������ � ������� � �������� �����������.
            TerrainGenerator.CreateFlatPlatform(terrain, platformCenter, platformSize, smoothingRadius);

            // ���������� ������������� ������ ��������� (����� ��������� ��������)
            float platformHeight = terrain.SampleHeight(platformCenter);
            Vector3 prefabPosition = new Vector3(platformCenter.x, platformHeight, platformCenter.z);

            // ������������� ������. ��� ������������� ����� parent ��� �������� �����������
            GameObject instance = Instantiate(prefab, prefabPosition, Quaternion.identity, platformsParent);
            instance.name = prefab.name + "_OnPlatform";

            // ���������� �������������, ����� ����������� �������� �� ������������
            placedPlatforms.Add(newPlatformRect);
        }
    }
}
