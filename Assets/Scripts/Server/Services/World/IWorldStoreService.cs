// Интерфейс для работы с состоянием мира
using System.Threading.Tasks;

public interface IWorldStoreService
{
    /// <summary>
    /// Загружает состояние мира для конкретной платформы
    /// </summary>
    Task<WorldSnapshot> GetWorldSnapshot(string worldId, string platform);

    /// <summary>
    /// Сохраняет состояние мира для указанной платформы
    /// </summary>
    Task<bool> SaveWorldSnapshot(string worldId, string platform, string jsonData, string worldSnapshotHash);
}
