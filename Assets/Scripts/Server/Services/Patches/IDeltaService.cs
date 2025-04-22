// ��������� ��� ������ � �������
using System.Threading.Tasks;

public interface IDeltaService
{
    /// <summary>
    /// ���������� ��������� (����) ����� ��������� ������� ���� � �������.
    /// </summary>
    Task<DeltaResponse> GetDelta(string worldId, string platform, string lastKnownSnapshotHash);
}
