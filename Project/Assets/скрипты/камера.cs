using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Объект, за которым будет следить камера
    public float smoothing = 5f; // Скорость сглаживания движения камеры

    public float zoomSpeed = 2f; // Скорость изменения масштаба
    public float minZoom = 2f; // Минимальный масштаб
    public float maxZoom = 10f; // Максимальный масштаб
    private float targetZoom; // Целевой масштаб камеры

    private bool isFollowingPlayer = true; // Флаг, указывающий, следует ли камера за игроком
    private bool isCameraStopped = false; // Флаг для остановки камеры

void Start()
{
    targetZoom = 8f; // Установка начального масштаба камеры на 8 единиц
    Camera.main.orthographicSize = targetZoom; // Применение начального масштаба
}


    void LateUpdate()
    {
        if (isCameraStopped)
        {
            return; // Останавливаем камеру
        }

        // Если камера следует за игроком
        if (isFollowingPlayer && target != null)
        {
            // Новая позиция камеры (центрируем камеру на цели)
            Vector3 targetCamPos = target.position;
            targetCamPos.z = transform.position.z; // Сохраняем текущее значение Z

            // Плавное движение камеры к новой позиции
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
        // Добавленное условие, чтобы MoveCameraToMouse выполнялся только при наличии цели
        else if (!isFollowingPlayer && target != null)
        {
            MoveCameraToMouse();
        }

        // Обработка изменения масштаба
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scrollData * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, smoothing * Time.deltaTime);

        // Отсоединение камеры от игрока при нажатии на клавишу "ё"
        if (Input.GetKey(KeyCode.BackQuote)) // Или другая клавиша по вашему выбору
        {
            isFollowingPlayer = false;
        }
        // Возвращаем камеру к игроку при отпускании клавиши "ё"
        else if (!Input.GetKey(KeyCode.BackQuote) && !isFollowingPlayer)
        {
            isFollowingPlayer = true;
        }

        // Проверка и коррекция координаты Z после всех операций
        Vector3 currentPos = transform.position;
        currentPos.z = Mathf.Clamp(currentPos.z, -10f, -1f); // Запрет изменения Z в заданном диапазоне
        transform.position = currentPos;
    }

    // Функция для установки нового объекта для отслеживания
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        isFollowingPlayer = true; // Возвращаем камеру к следованию за игроком
    }

    // Функция для перемещения камеры к мыши
    private void MoveCameraToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetCamPos = new Vector3(mousePos.x, mousePos.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);

        // Проверка, чтобы камера не уходила слишком далеко от выбранной модели
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > maxZoom)
            {
                transform.position = target.position + (transform.position - target.position).normalized * maxZoom;
            }
        }

        // Проверка и коррекция координаты Z после всех операций
        Vector3 currentPos = transform.position;
        currentPos.z = Mathf.Clamp(currentPos.z, -10f, -1f); // Запрет изменения Z в заданном диапазоне
        transform.position = currentPos;
    }

    // Корутин для тряски камеры
    public IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    // Функция для остановки камеры
    public void StopCamera()
    {
        isCameraStopped = true;
    }

    // Функция для возобновления следования камеры
    public void ResumeCamera()
    {
        isCameraStopped = false;
    }
}
