using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ThirdPersonCamera : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera; // Должен быть назначен в Inspector
    public float sensitivityX = 1.5f;
    public float sensitivityY = 1.5f;

    private PlayerInputActions inputActions;
    private Vector2 lookInput;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if (freeLookCamera == null)
        {
            Debug.LogError("CinemachineFreeLook НЕ назначен в Inspector!");
            return;
        }

        freeLookCamera.m_XAxis.m_InputAxisValue = lookInput.x * sensitivityX;
        freeLookCamera.m_YAxis.m_InputAxisValue = lookInput.y * sensitivityY;
    }
}
