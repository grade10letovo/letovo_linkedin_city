using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class WorldRuntimeManager : MonoBehaviour
{
    // �����������
    private AssetLoader assetLoader;
    private ServerApi serverApi;
    private WorldDataStore worldDataStore;
    //private ZoneLoader zoneLoader;
    //private ErrorHandlingManager errorHandlingManager;

    // ��� �����������
    private float loadProgress = 0f;

    // �������������
    private void Awake()
    {
        // �������� ����������� �� ������ �����������
        assetLoader = GetComponent<AssetLoader>();
        serverApi = GetComponent<ServerApi>();
        worldDataStore = GetComponent<WorldDataStore>();
        //errorHandlingManager = GetComponent<ErrorHandlingManager>();
    }

    private async void Start()
    {
        try
        {
            // ��������� ����������� ����� ��������� ������ ����
            bool isAuthorized = await CheckAuthorization();

            if (!isAuthorized)
            {
                Debug.LogError("�� ������� ������������ ������������!");
                return;
            }
            // ��� 1: ��������� ������ ����
            await LoadWorldData();

            // ��� 2: ��������� ���������� ������ � ���
            //ApplyWorldData();

            // ��� 4: �������������� �������� ������
            StartCoroutine(LoadChunks());
        }
        catch (System.Exception e)
        {
            // ������������ ����� ������, ������� ����� ���������� ��� ��������
            //errorHandlingManager.HandleError(e);
        }
    }
    private async Task<bool> CheckAuthorization()
    {
        return await serverApi.IsAuthorizedAsync();
    }

    // ����� ��� �������� ������ ���� � �������
    private async Task LoadWorldData()
    {
        Debug.Log("��������� ������ ���� � �������...");
        string worldDataJson = await serverApi.GetWorldDataJson();  // �������� ������ ���� � ������� JSON

        // ����������� ������ � ������ ������ � ���������
        worldDataStore.LoadWorldData(worldDataJson);
    }

    // ��������� ����������� ������ ����
   /* private void ApplyWorldData()
    {
        Debug.Log("��������� ������ ����...");
        var worldObjects = worldDataStore.GetWorldObjects();

        // ��� ������� ������� ��������������� ���������
        foreach (var worldObject in worldObjects)
        {
            if (worldObject != null)
            {
                IWorldComponent component = worldObject.GetComponent<IWorldComponent>();
                if (component != null)
                {
                    var state = worldDataStore.GetObjectState(worldObject);
                    component.LoadRuntimeState(state);
                }
            }
        }
    }*/

    // �������� ������ � ����������� �� ��������� ������
    private IEnumerator LoadChunks()
    {
        while (true)
        {
            // �������� ������� ������� ������
            Vector3 playerPosition = Camera.main.transform.position;

            // ����������, ����� ����� ����� ���������
            /*var chunksToLoad = zoneLoader.GetChunksToLoad(playerPosition);

            foreach (var chunk in chunksToLoad)
            {
                // ��������� ������ ����
                Debug.Log($"��������� ����: {chunk.name}");
                yield return zoneLoader.LoadChunkAsync(chunk);
            }

            // ���������� �������� ��������
            loadProgress = Mathf.Clamp01(zoneLoader.GetLoadingProgress());
            yield return null;  // ��� ���������� �����*/
        }
    }

    // ����� ��� ������ ��������� ��������
    private void Update()
    {
        // �������� �������� ��������
        Debug.Log($"�������� ��������: {loadProgress * 100}%");
    }

    // ����������� ������� ������
    public void UnloadWorldData()
    {
        //worldDataStore.Clear();
        //zoneLoader.ClearLoadedChunks();
    }
}
