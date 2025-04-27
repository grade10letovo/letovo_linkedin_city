// Модель для ответа с данными об ассете
public class AssetDownloadResponse
{
    public string Hash { get; set; }  // Хэш ассет‑бандла
    public byte[] Data { get; set; }  // Данные ассет‑бандла в виде массива байтов
}
