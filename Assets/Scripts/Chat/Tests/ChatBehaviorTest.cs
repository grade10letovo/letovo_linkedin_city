using UnityEngine;
using NUnit.Framework;
using TMPro;

public class ChatBehaviorTest
{
    private ChatBehavior chatBehavior;
    private GameObject canvas;
    private TMP_Text chatText;
    private TMP_InputField inputField;

    [SetUp]
    public void SetUp()
    {
        var chatGameObject = new GameObject();
        chatBehavior = chatGameObject.AddComponent<ChatBehavior>();

        canvas = new GameObject();
        chatText = new GameObject().AddComponent<TMP_Text>();
        inputField = new GameObject().AddComponent<TMP_InputField>();

        chatBehavior.GetType()
            .GetField("chatText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(chatBehavior, chatText);

        chatBehavior.GetType()
            .GetField("inputField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(chatBehavior, inputField);

        chatBehavior.GetType()
            .GetField("canvas", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(chatBehavior, canvas);
    }

    [Test]
    public void TestSendMessage()
    {
        inputField.text = "Hello, world!";
        
        chatBehavior.Send();

        Assert.IsEmpty(inputField.text, "поле ввода должно быть пустым.");
    }

    [Test]
    public void TestUIActivation()
    {
        chatBehavior.OnStartAuthority();

        Assert.IsTrue(canvas.activeSelf, "canvas должен быть активирован для владельца.");
    }
}
