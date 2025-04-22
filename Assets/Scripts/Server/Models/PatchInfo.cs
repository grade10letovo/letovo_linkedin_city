// Модель для хранения информации о патче
public class PatchInfo
{
    public string PatchId { get; set; }    // Идентификатор патча
    public string WorldId { get; set; }    // Идентификатор мира
    public string Platform { get; set; }   // Платформа, для которой создан патч
    public string Version { get; set; }    // Версия патча
    public string PatchData { get; set; }  // Данные патча в формате JSON
}
