using System.Threading.Tasks;

// Сервис для логирования ошибок
using System;

public class LoggingService : ILoggingService
{
    public async Task LogError(string errorType, string message, string stackTrace)
    {
        // Логируем ошибку (например, в базу данных или отправляем на сервер)
        Console.WriteLine($"Error: {errorType}, Message: {message}, StackTrace: {stackTrace}");
        await Task.CompletedTask;
    }
}
