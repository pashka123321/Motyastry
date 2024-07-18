using UnityEngine;

public class MovementController : MonoBehaviour
{
    private const float moveSpeed = -2f; // Скорость движения объекта
    private const float centeringSpeed = 1f; // Скорость центрирования (можно настроить)
    public LayerMask activatorLayer; // Layer объекта-активатора
    public LayerMask deathLayer; // Layer объекта-активатора

    private Transform target; // Целевая позиция для центрирования

    public enum Direction { 
        Up, Left, Down, Right
    };

    public Direction currentDirection;

    void FixedUpdate()
    {
        // Проверяем, касается ли объект какого-либо объекта на нужном layer'е
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f, activatorLayer); // Используем окружность с радиусом 0.2f для проверки касания

        // Двигаем объект в заданном направлении, если он касается активатора
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
                currentDirection = Direction.Down;
            }
            else if (rotationAngle >= 135f && rotationAngle < 225f)
            {
                direction = Vector2.left; // Влево
                currentDirection = Direction.Right; 
            }
            else if (rotationAngle >= 225f && rotationAngle < 315f)
            {
                direction = Vector2.down; // Вниз
                currentDirection = Direction.Up;
            }
            else
            {
                direction = Vector2.right; // Вправо
                currentDirection = Direction.Left;
            }

            // Двигаем объект в заданном направлении
            transform.Translate(direction * moveSpeed * Time.fixedDeltaTime);

            // Центрируем объект постепенно
            if (target != null)
            {
                Vector3 targetPosition = target.position;
                transform.position = Vector3.Lerp(transform.position, targetPosition, centeringSpeed * Time.fixedDeltaTime);
            }
        }
    }
}