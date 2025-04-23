using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WorldDataStore
{
    // Хранилища данных для компонентов
    private Dictionary<string, IWorldComponentData> componentDataStore;

    // Хранилище состояния объектов мира (позиции, состояния)
    private Dictionary<string, ObjectState> objectStateStore;

    // Хранилище ссылок на ассеты
    private Dictionary<string, AssetReference> assetReferences;

    // Конструктор
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
            // Парсим JSON в объект
            Snapshot snapshot = JsonUtility.FromJson<Snapshot>(snapshotJson);

            // Обрабатываем Bundles (загрузка через Addressables)
            List<object> assets = await LoadBundles(snapshot.bundles);

            // Обрабатываем объекты
            foreach (var obj in snapshot.objects)
            {
                // Восстановление состояния объекта
                //RestoreObjectState(obj);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error processing snapshot: {ex.Message}");
        }
    }
    // Метод для обработки снапшота JSON
    public async void LoadWorldData(string snapshotJson)
    {
        try
        {
            // Парсим JSON в объект
            Snapshot snapshot = JsonUtility.FromJson<Snapshot>(snapshotJson);

            // Обрабатываем Bundles (загрузка через Addressables)
            List<object> assets = await LoadBundles(snapshot.bundles);

            // Обрабатываем объекты
            foreach (var obj in snapshot.objects)
            {
                // Восстановление состояния объекта
                //RestoreObjectState(obj);
            }
        }   
        catch (System.Exception ex)
        {
            Debug.LogError($"Error processing snapshot: {ex.Message}");
        }
    }

    // Метод для загрузки bundles
    private async System.Threading.Tasks.Task<List<object>> LoadBundles(List<BundleInfo> bundles)
    {
        List<object> result = new List<object>();
        foreach (var bundle in bundles)
        {
            try
            {
                // Загружаем bundle по URL через Addressables
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
    // Метод для обработки загруженных ассетов (например, передать их в другие компоненты)
    private void ProcessLoadedAssets(AsyncOperationHandle handle)
    {
        // Пример обработки загруженных ассетов
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var loadedAssets = handle.Result;
            // Здесь вы можете передать ассеты в другие компоненты, например, в компоненты IWorldComponent
            // или в состояние объектов.

            Debug.Log($"Loaded assets: {loadedAssets}");

            // Пример передачи ассетов в IWorldComponent:
            // IWorldComponent component = ...;
            // component.ApplyLoadedAssets(loadedAssets);
        }
    }

    // Метод для восстановления состояния объекта
    /*private void RestoreObjectState(ObjectInfo objectInfo)
    {
        // Преобразуем данные объекта и восстанавливаем состояние с помощью LoadRuntimeState
        var componentData = obj.data; // Предположим, что data — это сериализованные данные для компонента
        if (components.TryGetValue(obj.id, out var component))
        {
            try
            {
                // Восстанавливаем состояние компонента через его метод LoadRuntimeState
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
    // Структуры для парсинга JSON
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
        public int v; // версия объекта
        public ObjectData data;
    }

    [System.Serializable]
    public class ObjectData
    {
        public int gold; // Для примера с TreasureChest
    }
}

// Пример структуры состояния объекта
[Serializable]
public class ObjectState
{
    public string Id;
    public Vector3 Position;
    public Quaternion Rotation;

    // Дополнительные данные для конкретных объектов
    public void ApplyCustomState(GameObject obj)
    {
        // Применяем состояние конкретного объекта
        // Это может быть, например, активность или какие-то специфические свойства
    }
}

// Пример структуры ассета
[Serializable]
public class AssetReference
{
    public string Id;
    public string AssetPath;

    public void ApplyLoadedData(object assetData)
    {
        // Применение загруженных данных
        // Например, установка текстуры или модели
    }
}

// Пример интерфейса для компонентов мира
public interface IWorldComponentData
{
    string Id { get; }
    int Version { get; }
    int LatestVersion { get; }
    object SaveState();
    void LoadState(object state);
    object Migrate(object oldData, int fromVersion);
}
