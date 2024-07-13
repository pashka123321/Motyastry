using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    [Header("Pulse Settings")]
    public Vector3 minScale = new Vector3(1f, 1f, 1f);  // Минимальный размер
    public Vector3 maxScale = new Vector3(2f, 2f, 2f);  // Максимальный размер
    public float pulseSpeed = 2f;  // Скорость пульсации

    private float pulseTimer = 0f;  // Таймер для отслеживания пульсации

    void Update()
    {
        // Увеличиваем таймер с учетом времени
        pulseTimer += Time.deltaTime * pulseSpeed;

        // Вычисляем синусоиду для плавного перехода между minScale и maxScale
        float scale = (Mathf.Sin(pulseTimer) + 1f) / 2f;  // Значение от 0 до 1
        transform.localScale = Vector3.Lerp(minScale, maxScale, scale);
    }
}
