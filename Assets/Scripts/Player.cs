using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour //���� ������� ������, ��� ��� ������� ������
{
    [SyncVar(hook = nameof(SyncHealth))] //������ �����, ������� ����� ����������� ��� ������������� ����������
    int _SyncHealth;
    public int Health;
    public GameObject[] HealthGos;


    SyncList<Vector3> _SyncVector3Vars = new SyncList<Vector3>(); //� ������ SyncList �� ����� ������� SyncVar � �������� �����, ��� �������� �����
    public List<Vector3> Vector3Vars;


    public GameObject PointPrefab; //���� ������ ����� ��������� ������ Point
    public LineRenderer LineRenderer; //���� ������ ��� �� ���������
    int pointsCount;

    void Update()
    {
        if (isOwned) //���������, ���� �� � ��� ����� �������� ���� ������
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            float speed = 5f * Time.deltaTime;
            transform.Translate(new Vector2(h * speed, v * speed)); //������ ���������� ��������

            if (Input.GetKeyDown(KeyCode.H)) //�������� � ���� ����� �� ������� ������� H
            {
                if (isServer) //���� �� �������� ��������, �� ��������� � ����������������� ��������� ����������
                    ChangeHealthValue(Health - 1);
                else
                    CmdChangeHealth(Health - 1); //� ��������� ������ ������ �� ������ ������ �� ��������� ����������
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (isServer)
                    ChangeVector3Vars(transform.position);
                else
                    CmdChangeVector3Vars(transform.position);
            }
        }

        for (int i = 0; i < HealthGos.Length; i++)
        {
            HealthGos[i].SetActive(!(Health - 1 < i));
        }

        for (int i = pointsCount; i < Vector3Vars.Count; i++)
        {
            Instantiate(PointPrefab, Vector3Vars[i], Quaternion.identity);
            pointsCount++;

            LineRenderer.positionCount = Vector3Vars.Count;
            LineRenderer.SetPositions(Vector3Vars.ToArray());
        }
    }

    //����� �� ����������, ���� ������ �������� ����� ������
    void SyncHealth(int oldValue, int newValue) //����������� ������ ��� �������� - ������ � �����. 
    {
        Health = newValue;
    }

    [Server] //����������, ��� ���� ����� ����� ���������� � ����������� ������ �� �������
    public void ChangeHealthValue(int newValue)
    {
        _SyncHealth = newValue;
    }

    [Command] //����������, ��� ���� ����� ������ ����� ����������� �� ������� �� ������� �������
    public void CmdChangeHealth(int newValue) //����������� ������ Cmd � ������ �������� ������
    {
        ChangeHealthValue(newValue); //��������� � ����������������� ��������� ����������
    }


    [Server]
    void ChangeVector3Vars(Vector3 newValue)
    {
        _SyncVector3Vars.Add(newValue);
    }

    [Command]
    public void CmdChangeVector3Vars(Vector3 newValue)
    {
        ChangeVector3Vars(newValue);
    }

    void SyncVector3Vars(SyncList<Vector3>.Operation op, int index, Vector3 oldItem, Vector3 newItem)
    {
        switch (op)
        {
            case SyncList<Vector3>.Operation.OP_ADD:
                {
                    Vector3Vars.Add(newItem);
                    break;
                }
            case SyncList<Vector3>.Operation.OP_CLEAR:
                {

                    break;
                }
            case SyncList<Vector3>.Operation.OP_INSERT:
                {

                    break;
                }
            case SyncList<Vector3>.Operation.OP_REMOVEAT:
                {

                    break;
                }
            case SyncList<Vector3>.Operation.OP_SET:
                {

                    break;
                }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();


        Vector3Vars = new List<Vector3>(_SyncVector3Vars.Count); //��� ��� Callback ��������� ������ �� ��������� �������,  
        for (int i = 0; i < _SyncVector3Vars.Count; i++) //� � ��� �� ������ ����������� ��� ����� ���� �����-�� ������ � �������, ��� ����� ��� ������ ������ � ��������� ������
        {
            Vector3Vars.Add(_SyncVector3Vars[i]);
        }
    }
}