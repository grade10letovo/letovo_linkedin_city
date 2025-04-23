/*// Сервис для работы с ассетами
using System.Threading.Tasks;
using UnityEditor;

public class AssetStoreService : IAssetStoreService
{
    private readonly IAssetDatabase assetDatabase;

    // Конструктор сервиса
    public AssetStoreService(IAssetDatabase assetDatabase)
    {
        this.assetDatabase = assetDatabase;
    }

    /// <summary>
    /// Загружает ассет-бандл по хэшу
    /// </summary>
    public async Task<AssetDownloadResponse> GetAssetBundle(string assetBundleHash)
    {
        var bundle = await assetDatabase.GetAssetBundleByHash(assetBundleHash);
        return new AssetDownloadResponse
        {
            Hash = assetBundleHash,
            Data = bundle.Data
        };
    }

    /// <summary>
    /// Загружает ассет-бандл на сервер
    /// </summary>
    public async Task<bool> SaveAssetBundle(string worldId, string platform, string assetBundleHash, byte[] assetBundleData)
    {
        return await assetDatabase.SaveAssetBundle(worldId, platform, assetBundleHash, assetBundleData);
    }
}
*/