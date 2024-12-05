using UnityEngine;
using TMPro;

public class ChatFieldBehaivior : MonoBehaviour
{
    public TMP_InputField chatInputField;
    public FirstPersonController playerController;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            chatInputField.ActivateInputField();
            playerController.SetChatMode(true);
        }
    }

    
    public void QuiteChat()
    {
        chatInputField.DeactivateInputField();
        playerController.SetChatMode(false);
    }
}
