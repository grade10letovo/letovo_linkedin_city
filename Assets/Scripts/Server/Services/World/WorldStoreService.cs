/*// —ервис дл€ работы с состо€нием мира
using System.Threading.Tasks;

public class WorldStoreService : IWorldStoreService
{

    /// <summary>
    /// «агружает состо€ние мира из базы данных
    /// </summary>
    public async Task<WorldSnapshot> GetWorldSnapshot(string worldId, string platform)
    {
        return await worldDatabase.GetSnapshot(worldId, platform);
    }

    /// <summary>
    /// —охран€ет состо€ние мира в базу данных
    /// </summary>
    public async Task<bool> SaveWorldSnapshot(string worldId, string platform, string jsonData, string worldSnapshotHash)
    {
        return await worldDatabase.SaveSnapshot(worldId, platform, jsonData, worldSnapshotHash);
    }
}*/
