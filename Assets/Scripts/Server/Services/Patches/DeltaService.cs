/*// Сервис для работы с патчами мира
using System.Threading.Tasks;

public class DeltaService : IDeltaService
{
    private readonly IWorldDatabase worldDatabase;

    // Конструктор сервиса
    public DeltaService(IWorldDatabase worldDatabase)
    {
        this.worldDatabase = worldDatabase;
    }

    /// <summary>
    /// Получаем патч, содержащий только изменения (новые объекты, изменения) с момента последней версии
    /// </summary>
    public async Task<DeltaResponse> GetDelta(string worldId, string platform, string lastKnownSnapshotHash)
    {
        var currentSnapshot = await worldDatabase.GetSnapshot(worldId, platform);
        var delta = GenerateDelta(currentSnapshot, lastKnownSnapshotHash);
        return delta;
    }

    private DeltaResponse GenerateDelta(WorldSnapshot currentSnapshot, string lastKnownSnapshotHash)
    {
        // Логика для вычисления изменений, например, сравнение старой и новой версии
        return new DeltaResponse
        {
            Bundles = GetChangedBundles(currentSnapshot, lastKnownSnapshotHash),
            Json = GetChangedObjects(currentSnapshot, lastKnownSnapshotHash)
        };
    }
}*/
