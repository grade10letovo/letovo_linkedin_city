// Интерфейс для работы с подписаниями
using System.Threading.Tasks;

public interface ISignatureService
{
    /// <summary>
    /// Проверяет подпись данных
    /// </summary>
    Task<bool> ValidateWorldSignature(string worldId, string platform, string signature, string payload);
}
