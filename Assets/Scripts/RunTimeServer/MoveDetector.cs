using UnityEngine;

[RequireComponent (typeof(WebSocketClient))]
/// <summary>
/// Компонент отслеживает движение объекта и отправляет сообщение через MessageDispatcher
/// </summary>
public class MoveDetector : MonoBehaviour
{
    [Tooltip("Насколько нужно сдвинуться, чтобы сработало событие")]
    public float threshold = 0.01f;

    [Tooltip("Интервал между проверками движения (секунды)")]
    public float checkInterval = 0.1f;

    private Vector3 lastPosition;
    private float timer;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            Vector3 currentPosition = transform.position;
            float distance = Vector3.Distance(currentPosition, lastPosition);

            if (distance > threshold)
            {
                SendMove(currentPosition);
                lastPosition = currentPosition;
            }

            timer = 0;
        }
    }

    void SendMove(Vector3 pos)
    {
        var moveMessage = new MoveMessage
        {
            playerId = WebSocketClient.Instance.playerId, // можно заменить на внешний ID
            position = new Vec3
            {
                x = pos.x,
                y = pos.y,
                z = pos.z
            }
        };

        MessageDispatcher.Send("player_moved", moveMessage);
    }
}
