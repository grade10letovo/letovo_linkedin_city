using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThirdPersonController : MonoBehaviour
{
    public Text modeText;
    public CharacterController characterController;
    public Animator animator;
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float sprintSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Audio Settings")]
    public AudioSource footstepSource;
    public AudioClip footstepSounds;
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
    private float currentSpeed;

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
        Vector3 rayStart = transform.position + Vector3.up * 0.5f; // Сдвигаем начало вверх
        float rayDistance = 0.6f; // Чуть больше, чем радиус коллайдера
        return Physics.Raycast(rayStart, transform.forward, out hit, rayDistance);
    }



    private void Update()
    {
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

        // 🔹 Определяем скорость передвижения
        if (moveInput.magnitude > 0.001f && isGrounded)
        {
            if (inputActions.Player.Sprint.IsPressed() && !IsFacingWall())
            {
                modeText.text = "Спринт";
                currentSpeed = sprintSpeed;
            }
            else if (inputActions.Player.Run.IsPressed())
            {
                modeText.text = "Бег";
                currentSpeed = runSpeed;
            }
            else
            {
                modeText.text = "Ходьба";
                currentSpeed = walkSpeed;
            }

            // 🔊 Запускаем звук шагов, если персонаж двигается
            if (!footstepSource.isPlaying && landSource != null)
            {
                footstepSource.PlayOneShot(footstepSounds);
            }
        }
        else
        {
            modeText.text = "Остановка";
            currentSpeed = 0;
        }

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // 🔹 Передаём скорость в Blend Tree
        animator.SetFloat("Speed", currentSpeed);

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
