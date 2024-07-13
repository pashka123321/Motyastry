using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float speed = 1.0f;  // Скорость движения конвейера
    public bool moveRight = true;  // Направление движения конвейера (true - вправо, false - влево)
    public GameObject conveyorPrefab;  // Префаб объекта конвейера
    private bool isMoving;  // Флаг для определения, движется ли конвейер

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Conveyor"))
        {
            isMoving = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Conveyor"))
        {
            isMoving = false;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // Определяем направление движения конвейера
            float direction = moveRight ? 1.0f : -1.0f;

            // Вычисляем смещение на основе скорости и направления
            float moveDistance = speed * Time.deltaTime * direction;

            // Перемещаем каждый объект в пределах коллайдера конвейера
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.CompareTag("ConveyorMoveableObject"))
                {
                    collider.transform.Translate(moveDistance, 0, 0);
                }
            }
        }
    }
}
