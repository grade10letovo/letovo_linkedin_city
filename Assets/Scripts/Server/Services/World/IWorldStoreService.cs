// ��������� ��� ������ � ���������� ����
using System.Threading.Tasks;

public interface IWorldStoreService
{
    /// <summary>
    /// ��������� ��������� ���� ��� ���������� ���������
    /// </summary>
    Task<WorldSnapshot> GetWorldSnapshot(string worldId, string platform);

    /// <summary>
    /// ��������� ��������� ���� ��� ��������� ���������
    /// </summary>
    Task<bool> SaveWorldSnapshot(string worldId, string platform, string jsonData, string worldSnapshotHash);
}
