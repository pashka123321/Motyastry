using System.Collections;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private const float moveSpeed = -2f; // Скорость движения объекта
    private const float delay = 0.07f; // Задержка перед переключением на следующий активатор
    public LayerMask activatorLayer; // Layer объекта-активатора

    private Transform target; // Целевая позиция для движения
    private bool isWaiting = false; // Флаг, указывающий, что происходит задержка

    void FixedUpdate()
    {
        // Проверяем, касается ли объект какого-либо объекта на нужном layer'е
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f, activatorLayer); // Используем окружность с радиусом 0.2f для проверки касания

        // Если объект касается активатора и не в ожидании
        if (colliders.Length > 0 && !isWaiting)
        {
            // Обрабатываем только первый найденный активатор
            Collider2D collider = colliders[0];

            // Запускаем корутину для обработки задержки
            StartCoroutine(SwitchTargetAfterDelay(collider.transform));
        }
        else if (colliders.Length == 0)
        {
            // Если нет активаторов под объектом, сбрасываем целевую позицию
            target = null;
        }

        // Если есть целевая позиция, двигаем объект в заданном направлении
        if (target != null)
        {
            // Получаем угол поворота активатора
            float rotationAngle = target.eulerAngles.z;

            // Определяем направление движения в зависимости от угла поворота
            Vector2 direction = Vector2.zero;
            if (rotationAngle >= 45f && rotationAngle < 135f)
            {
                direction = Vector2.up; // Вверх
            }
            else if (rotationAngle >= 135f && rotationAngle < 225f)
            {
                direction = Vector2.left; // Влево
            }
            else if (rotationAngle >= 225f && rotationAngle < 315f)
            {
                direction = Vector2.down; // Вниз
            }
            else
            {
                direction = Vector2.right; // Вправо
            }

            // Двигаем объект в заданном направлении
            transform.Translate(direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator SwitchTargetAfterDelay(Transform newTarget)
    {
        isWaiting = true; // Устанавливаем флаг ожидания

        // Ожидаем заданное время
        yield return new WaitForSeconds(delay);

        // Обновляем целевую позицию
        target = newTarget;

        isWaiting = false; // Сбрасываем флаг ожидания
    }
}
