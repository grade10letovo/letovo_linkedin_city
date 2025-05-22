using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Mirror;

public class ThirdPersonController : NetworkBehaviour
{
    public Text modeText;
    public CharacterController characterController;
    public Animator animator;

    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float sprintSpeed = 6f;
    public float acceleration = 4f;  // Как быстро набираем скорость
    public float deceleration = 6f;  // Как быстро тормозим
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Audio Settings")]
    public AudioSource footstepSource;
    public AudioClip footstepSound;
    public float footstepIntervalWalk = 0.6f;
    public float footstepIntervalRun = 0.4f;
    public float footstepIntervalSprint = 0.3f;

    public AudioSource jumpSource;
    public AudioClip jumpSound;
    public AudioSource landSource;
    public AudioClip landSound;

    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool wasGroundedLastFrame;
    private bool isJumping;
    private float targetSpeed;
    private float currentSpeed = 0f;
    private float footstepTimer = 0f;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private bool IsFacingWall()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        float rayDistance = 0.6f;
        return Physics.Raycast(rayStart, transform.forward, out hit, rayDistance);
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 🔹 Обрабатываем анимации приземления и падения
        if (!isGrounded && velocity.y < -0.5f)
        {
            animator.SetBool("IsFalling", true);
            animator.SetBool("IsLanding", false);
        }
        else if (isGrounded && !wasGroundedLastFrame)
        {
            animator.SetBool("IsFalling", false);
            animator.SetBool("IsLanding", true);
            isJumping = false;

            // 🔊 Звук приземления
            if (landSound != null)
            {
                landSource.PlayOneShot(landSound);
            }
        }

        // 🔹 Определяем целевую скорость
        if (moveInput.magnitude > 0.001f && isGrounded)
        {
            if (inputActions.Player.Sprint.IsPressed() && !IsFacingWall())
            {
                // modeText.text = "Спринт";
                targetSpeed = sprintSpeed;
            }
            else if (inputActions.Player.Run.IsPressed())
            {
                // modeText.text = "Бег";
                targetSpeed = runSpeed;
            }
            else
            {
                // modeText.text = "Ходьба";
                targetSpeed = walkSpeed;
            }
        }
        else
        {
            // modeText.text = "Остановка";
            targetSpeed = 0;
        }

        // 🎯 Плавное изменение скорости
        if (currentSpeed < targetSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
            if (currentSpeed > targetSpeed) currentSpeed = targetSpeed;
        }
        else if (currentSpeed > targetSpeed)
        {
            currentSpeed -= deceleration * Time.deltaTime;
            if (currentSpeed < 0) currentSpeed = 0;
        }

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // 🔹 Передаём скорость в Blend Tree (увеличивает скорость анимации)
        animator.SetFloat("Speed", currentSpeed);

        // 🔊 Управление звуками шагов
        if (isGrounded && moveInput.magnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;

            float stepInterval = footstepIntervalWalk;
            if (currentSpeed > walkSpeed) stepInterval = footstepIntervalRun;
            if (currentSpeed > runSpeed) stepInterval = footstepIntervalSprint;

            if (footstepTimer <= 0f)
            {
                if (footstepSound != null)
                {
                    footstepSource.PlayOneShot(footstepSound);
                }
                footstepTimer = stepInterval;
            }
        }

        // 🔥 Улучшенная логика прыжка
        if (inputActions.Player.Jump.triggered && isGrounded && !isJumping)
        {
            isJumping = true;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");

            // 🔊 Звук прыжка
            if (jumpSound != null)
            {
                jumpSource.PlayOneShot(jumpSound);
            }
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        wasGroundedLastFrame = isGrounded;
    }
}
