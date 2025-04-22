// Интерфейс для валидации ассетов
using System.Threading.Tasks;

public interface IAssetValidationService
{
    /// <summary>
    /// Проверяет целостность и корректность ассет‑бандла
    /// </summary>
    Task<bool> ValidateAssetBundle(string assetBundleHash);
}
