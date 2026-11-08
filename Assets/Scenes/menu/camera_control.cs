using UnityEngine;
// Обязательно добавляем пространство имен новой системы ввода
using UnityEngine.InputSystem; 

public class MenuCameraParallax : MonoBehaviour
{
    [Header("Настройки движения")]
    [Tooltip("Максимальное смещение камеры по осям X и Y")]
    public Vector2 moveRange = new Vector2(2f, 1f);
    
    [Tooltip("Максимальный угол поворота камеры")]
    public Vector2 rotateRange = new Vector2(5f, 5f);

    [Tooltip("Плавность движения (чем больше, тем медленнее камера догоняет мышь)")]
    public float smoothTime = 0.3f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    
    private Vector3 moveVelocity;
    private float rotationVelocityX;
    private float rotationVelocityY;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        // Проверяем, подключена ли мышь в данный момент
        if (Mouse.current == null) return;

        // Получаем позицию мыши на экране через новый Input System
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Преобразуем в диапазон от -1 до 1
        float mouseX = (mousePos.x / Screen.width) * 2f - 1f;
        float mouseY = (mousePos.y / Screen.height) * 2f - 1f;

        mouseX = Mathf.Clamp(mouseX, -1f, 1f);
        mouseY = Mathf.Clamp(mouseY, -1f, 1f);

        // --- 1. ПЛАВНОЕ ПЕРЕМЕЩЕНИЕ ---
        Vector3 targetPosition = startPosition + new Vector3(mouseX * moveRange.x, mouseY * moveRange.y, 0f);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref moveVelocity, smoothTime);

        // --- 2. ПЛАВНОЕ ВРАЩЕНИЕ ---
        float targetRotX = startRotation.eulerAngles.x - (mouseY * rotateRange.y);
        float targetRotY = startRotation.eulerAngles.y + (mouseX * rotateRange.x);

        float currentRotX = Mathf.SmoothDampAngle(transform.eulerAngles.x, targetRotX, ref rotationVelocityX, smoothTime);
        float currentRotY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotY, ref rotationVelocityY, smoothTime);

        transform.rotation = Quaternion.Euler(currentRotX, currentRotY, startRotation.eulerAngles.z);
    }
}
