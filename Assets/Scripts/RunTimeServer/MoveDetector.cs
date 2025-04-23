using UnityEngine;

[RequireComponent (typeof(WebSocketClient))]
/// <summary>
/// ��������� ����������� �������� ������� � ���������� ��������� ����� MessageDispatcher
/// </summary>
public class MoveDetector : MonoBehaviour
{
    [Tooltip("��������� ����� ����������, ����� ��������� �������")]
    public float threshold = 0.01f;

    [Tooltip("�������� ����� ���������� �������� (�������)")]
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
            playerId = WebSocketClient.Instance.playerId, // ����� �������� �� ������� ID
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
