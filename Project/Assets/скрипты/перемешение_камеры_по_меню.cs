using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float changeDirectionInterval = 5f;
    public float maxDistanceFromCenter = 3f; // Максимальное расстояние от центра сцены
    public float movementSpeed = 2f;
    public float directionChangeSpeed = 2f; // Скорость изменения направления

    private float timeToChangeDirection;
    private Vector2 targetDirection;
    private Vector2 currentDirection;
    private Vector3 initialPosition;

    void Start()
    {
        timeToChangeDirection = changeDirectionInterval;
        initialPosition = transform.position;
        SetNewDirection();
    }

    void Update()
    {
        timeToChangeDirection -= Time.deltaTime;

        if (timeToChangeDirection <= 0f)
        {
            SetNewDirection();
            timeToChangeDirection = changeDirectionInterval;
        }

        // Плавное изменение направления
        currentDirection = Vector2.Lerp(currentDirection, targetDirection, directionChangeSpeed * Time.deltaTime);

        // Двигаем камеру в текущем направлении
        transform.Translate(new Vector3(currentDirection.x, currentDirection.y, 0) * movementSpeed * Time.deltaTime);

        // Проверяем, не вышла ли камера за пределы максимального расстояния от центра
        Vector3 offset = transform.position - initialPosition;
        if (offset.magnitude > maxDistanceFromCenter)
        {
            // Если вышла, возвращаем камеру в пределы
            transform.position = initialPosition + offset.normalized * maxDistanceFromCenter;
        }
    }

    void SetNewDirection()
    {
        // Получаем новое случайное направление
        do
        {
            targetDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        } while (Vector2.Dot(targetDirection, currentDirection) > 0.75f); // Условие: не лететь дважды в одну и ту же сторону
    }
}
