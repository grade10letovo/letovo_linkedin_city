// Интерфейс для работы с ассетами
using System.Threading.Tasks;

public interface IAssetStoreService
{
    /// <summary>
    /// Получает ассет-бандл по хэшу
    /// </summary>
    Task<AssetDownloadResponse> GetAssetBundle(string assetBundleHash);

    /// <summary>
    /// Загружает ассет-бандл на сервер
    /// </summary>
    Task<bool> SaveAssetBundle(string worldId, string platform, string assetBundleHash, byte[] assetBundleData);
}
