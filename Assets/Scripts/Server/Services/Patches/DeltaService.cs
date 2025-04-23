/*// ������ ��� ������ � ������� ����
using System.Threading.Tasks;

public class DeltaService : IDeltaService
{
    private readonly IWorldDatabase worldDatabase;

    // ����������� �������
    public DeltaService(IWorldDatabase worldDatabase)
    {
        this.worldDatabase = worldDatabase;
    }

    /// <summary>
    /// �������� ����, ���������� ������ ��������� (����� �������, ���������) � ������� ��������� ������
    /// </summary>
    public async Task<DeltaResponse> GetDelta(string worldId, string platform, string lastKnownSnapshotHash)
    {
        var currentSnapshot = await worldDatabase.GetSnapshot(worldId, platform);
        var delta = GenerateDelta(currentSnapshot, lastKnownSnapshotHash);
        return delta;
    }

    private DeltaResponse GenerateDelta(WorldSnapshot currentSnapshot, string lastKnownSnapshotHash)
    {
        // ������ ��� ���������� ���������, ��������, ��������� ������ � ����� ������
        return new DeltaResponse
        {
            Bundles = GetChangedBundles(currentSnapshot, lastKnownSnapshotHash),
            Json = GetChangedObjects(currentSnapshot, lastKnownSnapshotHash)
        };
    }
}*/
