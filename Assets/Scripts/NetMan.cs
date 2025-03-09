using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMan : NetworkManager
{
    bool playerSpawned;
    bool playerConnected;

    public void OnCreateCharacter(NetworkConnectionToClient conn, PosMessage message)
    {
        GameObject go = Instantiate(playerPrefab, message.vector2, Quaternion.identity); //�������� �� ������� ������� gameObject
        NetworkServer.AddPlayerForConnection(conn, go); //������������ gameObject � ���� ������� �������� � ���������� ���������� �� ���� ��������� �������
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<PosMessage>(OnCreateCharacter); //���������, ����� struct ������ ������ �� ������, ����� ���������� �����
    }

    public void ActivatePlayerSpawn()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = 10f;
        pos = Camera.main.ScreenToWorldPoint(pos);

        PosMessage m = new PosMessage() { vector2 = pos }; //������� struct ������������� ����, ����� ������ ����� � ���� ��� ������ ���������
        NetworkClient.Send(m); //�������� ��������� �� ������ � ������������ ������
        playerSpawned = true;
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        playerConnected = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !playerSpawned && playerConnected)
        {
            ActivatePlayerSpawn();
        }
    }
}

public struct PosMessage : NetworkMessage //����������� �� ���������� NetworkMessage, ����� ������� ������ ����� ������ �����������
{
    public Vector2 vector2; //������ ������������ Property
}