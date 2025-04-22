// ��������� ��� ������ � �������� ����
using System.Threading.Tasks;

public interface IVersionService
{
    /// <summary>
    /// �������� ���������� � ������� ������ ����
    /// </summary>
    Task<WorldVersionInfo> GetWorldVersionInfo(string worldId, string platform);

    /// <summary>
    /// ��������� ������ ����
    /// </summary>
    Task<bool> UpdateWorldVersion(string worldId, string platform, string version);
}
