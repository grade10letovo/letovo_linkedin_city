using System.Threading.Tasks;

// Интерфейс для логирования
public interface ILoggingService
{
    /// <summary>
    /// Логирует ошибку
    /// </summary>
    Task LogError(string errorType, string message, string stackTrace);
}
