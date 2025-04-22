// Интерфейс для работы с версиями мира
using System.Threading.Tasks;

public interface IVersionService
{
    /// <summary>
    /// Получает информацию о текущей версии мира
    /// </summary>
    Task<WorldVersionInfo> GetWorldVersionInfo(string worldId, string platform);

    /// <summary>
    /// Обновляет версию мира
    /// </summary>
    Task<bool> UpdateWorldVersion(string worldId, string platform, string version);
}
