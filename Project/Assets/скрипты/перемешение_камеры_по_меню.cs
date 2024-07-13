using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float changeDirectionInterval = 5f;
    public float maxDistanceFromCenter = 3f; // Максимальное расстояние от центра сцены
    public float movementSpeed = 2f;

    private float timeToChangeDirection;
    private Vector2 currentDirection;
    private Vector3 initialPosition;

    void Start()
    {
        timeToChangeDirection = changeDirectionInterval;
        initialPosition = transform.position;
        ChangeDirection();
    }

    void Update()
    {
        timeToChangeDirection -= Time.deltaTime;
        
        if (timeToChangeDirection <= 0f)
        {
            ChangeDirection();
            timeToChangeDirection = changeDirectionInterval;
        }

        // Двигаем камеру в текущем направлении
        transform.Translate(currentDirection * movementSpeed * Time.deltaTime);

        // Проверяем, не вышла ли камера за пределы максимального расстояния от центра
        Vector3 offset = transform.position - initialPosition;
        if (offset.magnitude > maxDistanceFromCenter)
        {
            // Если вышла, возвращаем камеру в пределы
            transform.position = initialPosition + offset.normalized * maxDistanceFromCenter;
        }
    }

    void ChangeDirection()
    {
        // Получаем новое случайное направление
        do
        {
            currentDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        } while (Vector2.Dot(currentDirection, (Vector2)transform.right) > 0.75f); // Условие: не лететь дважды в одну и ту же сторону
    }
}
