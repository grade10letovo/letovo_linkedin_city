using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class WorldRuntimeManager : MonoBehaviour
{
    // Зависимости
    private AssetLoader assetLoader;
    private ServerApi serverApi;
    private WorldDataStore worldDataStore;
    //private ZoneLoader zoneLoader;
    //private ErrorHandlingManager errorHandlingManager;

    // Для оптимизации
    private float loadProgress = 0f;

    // Инициализация
    private void Awake()
    {
        // Получаем зависимости из других компонентов
        assetLoader = GetComponent<AssetLoader>();
        serverApi = GetComponent<ServerApi>();
        worldDataStore = GetComponent<WorldDataStore>();
        //errorHandlingManager = GetComponent<ErrorHandlingManager>();
    }

    private async void Start()
    {
        try
        {
            // Проверяем авторизацию перед загрузкой данных мира
            bool isAuthorized = await CheckAuthorization();

            if (!isAuthorized)
            {
                Debug.LogError("Не удалось авторизовать пользователя!");
                return;
            }
            // Шаг 1: Загружаем данные мира
            await LoadWorldData();

            // Шаг 2: Применяем полученные данные в мир
            //ApplyWorldData();

            // Шаг 4: Инициализируем стриминг чанков
            StartCoroutine(LoadChunks());
        }
        catch (System.Exception e)
        {
            // Обрабатываем любые ошибки, которые могут возникнуть при загрузке
            //errorHandlingManager.HandleError(e);
        }
    }
    private async Task<bool> CheckAuthorization()
    {
        return await serverApi.IsAuthorizedAsync();
    }

    // Метод для загрузки данных мира с сервера
    private async Task LoadWorldData()
    {
        Debug.Log("Загружаем данные мира с сервера...");
        string worldDataJson = await serverApi.GetWorldDataJson();  // Получаем данные мира в формате JSON

        // Преобразуем данные в нужный формат и сохраняем
        worldDataStore.LoadWorldData(worldDataJson);
    }

    // Применяем загруженные данные мира
   /* private void ApplyWorldData()
    {
        Debug.Log("Применяем данные мира...");
        var worldObjects = worldDataStore.GetWorldObjects();

        // Для каждого объекта восстанавливаем состояние
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

    // Стриминг чанков в зависимости от положения игрока
    private IEnumerator LoadChunks()
    {
        while (true)
        {
            // Получаем текущую позицию игрока
            Vector3 playerPosition = Camera.main.transform.position;

            // Определяем, какие чанки нужно загрузить
            /*var chunksToLoad = zoneLoader.GetChunksToLoad(playerPosition);

            foreach (var chunk in chunksToLoad)
            {
                // Загружаем каждый чанк
                Debug.Log($"Загружаем чанк: {chunk.name}");
                yield return zoneLoader.LoadChunkAsync(chunk);
            }

            // Выставляем прогресс загрузки
            loadProgress = Mathf.Clamp01(zoneLoader.GetLoadingProgress());
            yield return null;  // Ждём следующего кадра*/
        }
    }

    // Метод для отчёта прогресса загрузки
    private void Update()
    {
        // Логируем прогресс загрузки
        Debug.Log($"Прогресс загрузки: {loadProgress * 100}%");
    }

    // Завершающая очистка данных
    public void UnloadWorldData()
    {
        //worldDataStore.Clear();
        //zoneLoader.ClearLoadedChunks();
    }
}
