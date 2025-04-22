// ��������� ��� ������ � ������������
using System.Threading.Tasks;

public interface ISignatureService
{
    /// <summary>
    /// ��������� ������� ������
    /// </summary>
    Task<bool> ValidateWorldSignature(string worldId, string platform, string signature, string payload);
}
