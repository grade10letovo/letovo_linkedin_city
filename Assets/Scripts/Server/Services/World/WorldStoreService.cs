// ������ ��� ������ � ���������� ����
using System.Threading.Tasks;

public class WorldStoreService : IWorldStoreService
{
    private readonly IWorldDatabase worldDatabase;

    // ����������� �������
    public WorldStoreService(IWorldDatabase worldDatabase)
    {
        this.worldDatabase = worldDatabase;
    }

    /// <summary>
    /// ��������� ��������� ���� �� ���� ������
    /// </summary>
    public async Task<WorldSnapshot> GetWorldSnapshot(string worldId, string platform)
    {
        return await worldDatabase.GetSnapshot(worldId, platform);
    }

    /// <summary>
    /// ��������� ��������� ���� � ���� ������
    /// </summary>
    public async Task<bool> SaveWorldSnapshot(string worldId, string platform, string jsonData, string worldSnapshotHash)
    {
        return await worldDatabase.SaveSnapshot(worldId, platform, jsonData, worldSnapshotHash);
    }
}
