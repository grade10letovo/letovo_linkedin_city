using System.Threading.Tasks;

// ��������� ��� �����������
public interface ILoggingService
{
    /// <summary>
    /// �������� ������
    /// </summary>
    Task LogError(string errorType, string message, string stackTrace);
}
