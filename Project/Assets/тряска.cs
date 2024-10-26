using UnityEngine;
using System.Collections;

// Класс для управления тряской камеры
public class CameraShakeController : MonoBehaviour
{
    public static CameraShakeController instance;
    private Vector3 originalPosition; // Исходная позиция камеры

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Метод для запуска тряски камеры
    public void ShakeCamera(float duration, float magnitude)
    {
        // Сохраняем исходную позицию камеры при начале тряски
        originalPosition = transform.position;
        StartCoroutine(Shake(duration, magnitude));
    }

    // Корутина для тряски камеры
    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Получаем случайные значения x и y для тряски
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            // Применяем смещение к текущей позиции камеры
            transform.position = originalPosition + new Vector3(offsetX, offsetY, 0);

            // Увеличиваем прошедшее время
            elapsed += Time.deltaTime;

            yield return null; // Ждём до следующего кадра
        }

        // Возвращаем камеру в её исходную позицию
        transform.position = originalPosition;
    }
}
