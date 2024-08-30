using UnityEngine;

public class RotatingPulsingSquare : MonoBehaviour
{
    public float rotationSpeed = 100f;  // Скорость вращения
    public float pulseSpeed = 2f;       // Скорость пульсации
    public float pulseScale = 0.2f;     // Масштаб пульсации

    private Vector3 initialScale;

    void Start()
    {
        // Сохраняем начальный масштаб квадрата
        initialScale = transform.localScale;
    }

    void Update()
    {
        // Вращаем квадрат
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // Пульсируем размером
        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
        transform.localScale = initialScale * scale;
    }
}
