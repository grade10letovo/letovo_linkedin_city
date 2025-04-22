// Модель для патча, который содержит изменения между мирами
public class DeltaResponse
{
    public string[] Bundles { get; set; }  // Список ассет‑бандлов, которые были изменены
    public string Json { get; set; }        // JSON с изменёнными объектами в мире
}
