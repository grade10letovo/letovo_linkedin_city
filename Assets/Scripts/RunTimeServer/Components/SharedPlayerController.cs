// SharedPlayerController.cs
using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(ThirdPersonController), typeof(StarterAssetsInputs))]
public class SharedPlayerController : MonoBehaviour
{
    public bool isLocalPlayer = true;
    public float inputUpdateInterval = 0.1f;

    private ThirdPersonController controller;
    private StarterAssetsInputs input;
    private float timer;

    public Player player;

    private void Awake()
    {
        controller = GetComponent<ThirdPersonController>();
        input = GetComponent<StarterAssetsInputs>();
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            input.move = Vector2.zero;
            input.jump = false;
            input.sprint = false;
        }
        else input.OnInputChanged += SendInputMessage;
    }
    private void SendInputMessage()
    {
        var msg = new InputMessage
        {
            playerId = WebSocketClient.Instance.playerId,
            move = new Vec2
            {
                x = input.move.x,
                y = input.move.y
            },
            look = new Vec2
            {
                x = input.look.x,
                y = input.look.y
            },
            jump = input.jump,
            sprint = input.sprint,
            dance = input.dance
        };

        MessageDispatcher.Send("player_input", msg);
    }

    public void ApplyNetworkInputToPlayer(InputMessage input)
    {
        if (player.playerId != input.playerId)
        {
            Debug.Log("[SharedPlayerController] player ID in message is not like in system");
            return;
        }

        var inputs = player.gameObject.GetComponent<StarterAssetsInputs>();
        if (inputs == null) return;

        inputs.MoveInput(new Vector2(input.move.x, input.move.y));
        inputs.JumpInput(input.jump);
        inputs.SprintInput(input.sprint);
        inputs.DanceInput(input.dance);
        inputs.LookInput(new Vector2(input.look.x, input.look.y));
    }
}