// Сервис для работы с версиями мира
/*using System.Threading.Tasks;

public class VersionService : IVersionService
{
    private readonly IWorldDatabase worldDatabase;

    // Конструктор сервиса
    public VersionService(IWorldDatabase worldDatabase)
    {
        this.worldDatabase = worldDatabase;
    }

    /// <summary>
    /// Получает информацию о текущей версии мира для конкретной платформы
    /// </summary>
    public async Task<WorldVersionInfo> GetWorldVersionInfo(string worldId, string platform)
    {
        return await worldDatabase.GetVersionInfo(worldId, platform);
    }

    /// <summary>
    /// Обновляет версию мира в базе данных
    /// </summary>
    public async Task<bool> UpdateWorldVersion(string worldId, string platform, string version)
    {
        return await worldDatabase.UpdateWorldVersion(worldId, platform, version);
    }
}
*/