// ��������� ��� ������ � ��������
using System.Threading.Tasks;

public interface IAssetStoreService
{
    /// <summary>
    /// �������� �����-����� �� ����
    /// </summary>
    Task<AssetDownloadResponse> GetAssetBundle(string assetBundleHash);

    /// <summary>
    /// ��������� �����-����� �� ������
    /// </summary>
    Task<bool> SaveAssetBundle(string worldId, string platform, string assetBundleHash, byte[] assetBundleData);
}
