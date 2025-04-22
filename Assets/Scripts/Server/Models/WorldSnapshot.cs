// Модель для хранения состояния мира
public class WorldSnapshot
{
    public string WorldId { get; set; } // Идентификатор мира
    public string JsonData { get; set; } // Состояние мира в формате JSON
    public string Hash { get; set; }     // Хэш состояния мира (для проверки целостности)
}
