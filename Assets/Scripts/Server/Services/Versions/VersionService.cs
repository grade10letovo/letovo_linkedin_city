// ������ ��� ������ � �������� ����
/*using System.Threading.Tasks;

public class VersionService : IVersionService
{
    private readonly IWorldDatabase worldDatabase;

    // ����������� �������
    public VersionService(IWorldDatabase worldDatabase)
    {
        this.worldDatabase = worldDatabase;
    }

    /// <summary>
    /// �������� ���������� � ������� ������ ���� ��� ���������� ���������
    /// </summary>
    public async Task<WorldVersionInfo> GetWorldVersionInfo(string worldId, string platform)
    {
        return await worldDatabase.GetVersionInfo(worldId, platform);
    }

    /// <summary>
    /// ��������� ������ ���� � ���� ������
    /// </summary>
    public async Task<bool> UpdateWorldVersion(string worldId, string platform, string version)
    {
        return await worldDatabase.UpdateWorldVersion(worldId, platform, version);
    }
}
*/