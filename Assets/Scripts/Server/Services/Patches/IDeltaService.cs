// Интерфейс для работы с патчами
using System.Threading.Tasks;

public interface IDeltaService
{
    /// <summary>
    /// Возвращает изменения (патч) между последней версией мира и текущей.
    /// </summary>
    Task<DeltaResponse> GetDelta(string worldId, string platform, string lastKnownSnapshotHash);
}
