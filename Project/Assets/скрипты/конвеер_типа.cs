using UnityEngine;

public class MovementController : MonoBehaviour
{
    private const float moveSpeed = -2f; // Скорость движения объекта
    private const float centeringSpeed = 1f; // Скорость центрирования
    private const float centeringThreshold = 0.1f; // Порог для определения, достаточно ли близко объект к центру
    public LayerMask activatorLayer; // Layer объекта-активатора

    private Transform target; // Целевая позиция для центрирования

    void FixedUpdate()
    {
        // Проверяем, касается ли объект какого-либо объекта на нужном layer'е
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f, activatorLayer); // Используем окружность с радиусом 0.2f для проверки касания

        // Если объект касается активатора
        if (colliders.Length > 0)
        {
            // Обрабатываем только первый найденный активатор
            Collider2D collider = colliders[0];

            // Устанавливаем целевую позицию для центрирования
            target = collider.transform;

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

            // Центрируем объект на целевой позиции, если активатор найден
            if (target != null)
            {
                // Проверяем, достаточно ли близко объект к целевой позиции
                if (Vector3.Distance(transform.position, target.position) > centeringThreshold)
                {
                    // Центрируем объект на целевой позиции
                    transform.position = Vector3.MoveTowards(transform.position, target.position, centeringSpeed * Time.fixedDeltaTime);
                }
            }
        }
    }
}
