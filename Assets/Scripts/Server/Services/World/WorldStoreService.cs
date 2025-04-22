// Сервис для работы с состоянием мира
using System.Threading.Tasks;

public class WorldStoreService : IWorldStoreService
{
    private readonly IWorldDatabase worldDatabase;

    // Конструктор сервиса
    public WorldStoreService(IWorldDatabase worldDatabase)
    {
        this.worldDatabase = worldDatabase;
    }

    /// <summary>
    /// Загружает состояние мира из базы данных
    /// </summary>
    public async Task<WorldSnapshot> GetWorldSnapshot(string worldId, string platform)
    {
        return await worldDatabase.GetSnapshot(worldId, platform);
    }

    /// <summary>
    /// Сохраняет состояние мира в базу данных
    /// </summary>
    public async Task<bool> SaveWorldSnapshot(string worldId, string platform, string jsonData, string worldSnapshotHash)
    {
        return await worldDatabase.SaveSnapshot(worldId, platform, jsonData, worldSnapshotHash);
    }
}
