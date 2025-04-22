using System.Threading.Tasks;

// ������ ��� ����������� ������
using System;

public class LoggingService : ILoggingService
{
    public async Task LogError(string errorType, string message, string stackTrace)
    {
        // �������� ������ (��������, � ���� ������ ��� ���������� �� ������)
        Console.WriteLine($"Error: {errorType}, Message: {message}, StackTrace: {stackTrace}");
        await Task.CompletedTask;
    }
}
