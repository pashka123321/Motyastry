using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    // Скорость движения конвейера
    public float conveyorSpeed = 2f;

    // Префабы для основного и запоминающего конвейеров
    public GameObject mainConveyorPrefab;      // Префаб для основного конвейера
    public GameObject rememberConveyorPrefab;  // Префаб для запоминающего конвейера

    // Переменная для хранения текущего направления движения
    private Vector2 currentDirection = Vector2.left;
    private bool isOnMainConveyor = false;  // Флаг, находится ли объект на основном конвейере

    private void Start()
    {
        // Изначально направление движения влево
        currentDirection = Vector2.left;
    }

    // Общий триггер для обработки и основного, и запоминающего триггеров
    private void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Проверяем, на каком триггере находится объект: основном или запоминающем
            if (other.gameObject == mainConveyorPrefab)
            {
                // Основной триггер: изменяет направление движения
                currentDirection = -transform.right; // Устанавливаем новое направление
                rb.velocity = currentDirection * conveyorSpeed;
                isOnMainConveyor = true;  // Устанавливаем, что объект на основном конвейере
            }
            else if (other.gameObject == rememberConveyorPrefab)
            {
                // Запоминающий триггер: продолжает движение в том же направлении
                if (isOnMainConveyor)
                {
                    // Продолжаем движение в сохраненном направлении
                    rb.velocity = currentDirection * conveyorSpeed;
                }
            }
        }
    }

    // Остановка движения, когда объект покидает основной триггер
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == mainConveyorPrefab)
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Останавливаем объект, если он покидает основной триггер
                rb.velocity = Vector2.zero;
                isOnMainConveyor = false;  // Объект больше не на конвейере
            }
        }
    }
}
