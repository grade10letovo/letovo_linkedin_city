using UnityEngine;
using Cinemachine;

public class CameraFollowHead : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera; // Камера
    public Transform targetObject;  // Объект (голова персонажа), за которым будет следить камера
    public float sensitivity = 2f;  // Чувствительность мыши
    public bool invertYAxis = false;  // Инвертировать ли ось Y

    public Vector3 defaultCameraOffset;  // Смещение камеры относительно объекта (головы)
    private Vector3 targetOffset;  // Смещение камеры, которое можно изменять

    private void Start()
    {
        // Устанавливаем начальное смещение камеры с учётом головы
        defaultCameraOffset = new Vector3(0f, 1.6f, -4f); // Можно настроить в зависимости от высоты головы
        targetOffset = defaultCameraOffset;

        // Устанавливаем начальное положение камеры относительно головы
        freeLookCamera.transform.position = targetObject.position + targetOffset;
    }

    void Update()
    {
        // Получаем данные от мыши для вращения камеры
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * (invertYAxis ? 1 : -1);

        // Вращаем камеру по осям X и Y
        freeLookCamera.m_XAxis.Value += mouseX;
        freeLookCamera.m_YAxis.Value = Mathf.Clamp(
            freeLookCamera.m_YAxis.Value + mouseY * 0.01f,
            0.1f,  // Минимальный угол наклона
            0.9f   // Максимальный угол наклона
        );

        // Обновляем позицию камеры относительно цели
        Vector3 desiredPosition = targetObject.position + targetOffset;
        freeLookCamera.transform.position = desiredPosition;

        // Обновляем углы наклона камеры, чтобы всегда была видна голова
        UpdateCameraTilt();
    }

    void UpdateCameraTilt()
    {
        // Если камера слишком низко и теряет голову, наклоняем её вверх
        if (freeLookCamera.m_YAxis.Value < 0.3f)
        {
            freeLookCamera.m_YAxis.Value = Mathf.Lerp(
                freeLookCamera.m_YAxis.Value,
                0.5f, // Угол наклона камеры (можно настроить)
                Time.deltaTime * 2f
            );
        }
    }
}
