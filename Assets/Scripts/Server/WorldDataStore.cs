using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WorldDataStore
{
    // ��������� ������ ��� �����������
    private Dictionary<string, IWorldComponentData> componentDataStore;

    // ��������� ��������� �������� ���� (�������, ���������)
    private Dictionary<string, ObjectState> objectStateStore;

    // ��������� ������ �� ������
    private Dictionary<string, AssetReference> assetReferences;

    // �����������
    public WorldDataStore()
    {
        componentDataStore = new Dictionary<string, IWorldComponentData>();
        objectStateStore = new Dictionary<string, ObjectState>();
        assetReferences = new Dictionary<string, AssetReference>();
        
    }
    public async void GetWorldObjects(string snapshotJson)
    {
        try
        {
            // ������ JSON � ������
            Snapshot snapshot = JsonUtility.FromJson<Snapshot>(snapshotJson);

            // ������������ Bundles (�������� ����� Addressables)
            List<object> assets = await LoadBundles(snapshot.bundles);

            // ������������ �������
            foreach (var obj in snapshot.objects)
            {
                // �������������� ��������� �������
                //RestoreObjectState(obj);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error processing snapshot: {ex.Message}");
        }
    }
    // ����� ��� ��������� �������� JSON
    public async void LoadWorldData(string snapshotJson)
    {
        try
        {
            // ������ JSON � ������
            Snapshot snapshot = JsonUtility.FromJson<Snapshot>(snapshotJson);

            // ������������ Bundles (�������� ����� Addressables)
            List<object> assets = await LoadBundles(snapshot.bundles);

            // ������������ �������
            foreach (var obj in snapshot.objects)
            {
                // �������������� ��������� �������
                //RestoreObjectState(obj);
            }
        }   
        catch (System.Exception ex)
        {
            Debug.LogError($"Error processing snapshot: {ex.Message}");
        }
    }

    // ����� ��� �������� bundles
    private async System.Threading.Tasks.Task<List<object>> LoadBundles(List<BundleInfo> bundles)
    {
        List<object> result = new List<object>();
        foreach (var bundle in bundles)
        {
            try
            {
                // ��������� bundle �� URL ����� Addressables
                var handle = Addressables.DownloadDependenciesAsync(bundle.url);
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Successfully loaded bundle: {bundle.label}");

                    result.Add(handle);
                }
                else
                {
                    Debug.LogError($"Failed to load bundle: {bundle.label}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error loading bundle {bundle.label}: {ex.Message}");
            }
        }
        return result;
    }
    // ����� ��� ��������� ����������� ������� (��������, �������� �� � ������ ����������)
    private void ProcessLoadedAssets(AsyncOperationHandle handle)
    {
        // ������ ��������� ����������� �������
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var loadedAssets = handle.Result;
            // ����� �� ������ �������� ������ � ������ ����������, ��������, � ���������� IWorldComponent
            // ��� � ��������� ��������.

            Debug.Log($"Loaded assets: {loadedAssets}");

            // ������ �������� ������� � IWorldComponent:
            // IWorldComponent component = ...;
            // component.ApplyLoadedAssets(loadedAssets);
        }
    }

    // ����� ��� �������������� ��������� �������
    /*private void RestoreObjectState(ObjectInfo objectInfo)
    {
        // ����������� ������ ������� � ��������������� ��������� � ������� LoadRuntimeState
        var componentData = obj.data; // �����������, ��� data � ��� ��������������� ������ ��� ����������
        if (components.TryGetValue(obj.id, out var component))
        {
            try
            {
                // ��������������� ��������� ���������� ����� ��� ����� LoadRuntimeState
                component.LoadRuntimeState(componentData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load runtime state for component {obj.id}: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Component {obj.id} not found.");
        }
    }
    */
    // ��������� ��� �������� JSON
    [System.Serializable]
    public class Snapshot
    {
        public string snapshotVersion;
        public string platform;
        public List<BundleInfo> bundles;
        public List<ObjectInfo> objects;
    }

    [System.Serializable]
    public class BundleInfo
    {
        public string label;
        public string crc;
        public string url;
    }

    [System.Serializable]
    public class ObjectInfo
    {
        public string id;
        public string type;
        public int v; // ������ �������
        public ObjectData data;
    }

    [System.Serializable]
    public class ObjectData
    {
        public int gold; // ��� ������� � TreasureChest
    }
}

// ������ ��������� ��������� �������
[Serializable]
public class ObjectState
{
    public string Id;
    public Vector3 Position;
    public Quaternion Rotation;

    // �������������� ������ ��� ���������� ��������
    public void ApplyCustomState(GameObject obj)
    {
        // ��������� ��������� ����������� �������
        // ��� ����� ����, ��������, ���������� ��� �����-�� ������������� ��������
    }
}

// ������ ��������� ������
[Serializable]
public class AssetReference
{
    public string Id;
    public string AssetPath;

    public void ApplyLoadedData(object assetData)
    {
        // ���������� ����������� ������
        // ��������, ��������� �������� ��� ������
    }
}

// ������ ���������� ��� ����������� ����
public interface IWorldComponentData
{
    string Id { get; }
    int Version { get; }
    int LatestVersion { get; }
    object SaveState();
    void LoadState(object state);
    object Migrate(object oldData, int fromVersion);
}
