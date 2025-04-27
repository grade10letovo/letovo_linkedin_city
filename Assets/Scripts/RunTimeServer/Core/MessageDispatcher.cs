using System;

public static class MessageDispatcher
{
    public static Action<string, object> OnMessageSend;
    public static Action<string, string> OnMessageReceive; // string: raw JSON

    public static void Send(string type, object data)
    {
        OnMessageSend?.Invoke(type, data);
    }

    public static void Receive(string type, string json)
    {
        OnMessageReceive?.Invoke(type, json);
    }
}