using UnityEngine;
using System.Collections.Generic;

public class Conveyor : MonoBehaviour
{
    public float speed = 2f; // Скорость движения объектов
    public float detectionRadius = 2f; // Радиус поиска следующего конвейера
    public LayerMask conveyorLayerMask; // Слой, на котором находятся конвейеры
    public LayerMask itemLayerMask; // Слой, на котором находятся объекты
    private Transform nextConveyor; // Ссылка на следующий конвейер
    private GameObject currentItem; // Текущий объект на конвейере
    private Queue<GameObject> waitingQueue = new Queue<GameObject>(); // Очередь объектов
    private bool isProcessing = false; // Флаг обработки текущего объекта

    void Start()
    {
        FindNextConveyor();
    }

    void Update()
    {
        // Если текущий объект есть и он обрабатывается
        if (currentItem != null && isProcessing)
        {
            // Целевая позиция на следующем конвейере
            Vector3 targetPosition = nextConveyor != null ? nextConveyor.position : transform.position + transform.right;

            // Проекция текущего объекта на центральную линию конвейера
            Vector3 conveyorCenter = transform.position;
            Vector3 conveyorDirection = transform.right;
            Vector3 projectedPosition = conveyorCenter + Vector3.Project(currentItem.transform.position - conveyorCenter, conveyorDirection);

            // Добавляем небольшую силу для удержания объекта ближе к центру
            Vector3 centerOffset = (projectedPosition - currentItem.transform.position) * 10f; // Коэффициент 0.5 можно настроить

            // Двигаем объект
            currentItem.transform.position = Vector3.MoveTowards(
                currentItem.transform.position,
                targetPosition + centerOffset,
                speed * Time.deltaTime
            );

            // Проверяем, достиг ли объект следующего конвейера
            if (nextConveyor != null && Vector3.Distance(currentItem.transform.position, nextConveyor.position) < 0.1f)
            {
                Conveyor nextConveyorScript = nextConveyor.GetComponent<Conveyor>();
                if (nextConveyorScript != null && nextConveyorScript.CanAcceptItem())
                {
                    // Передаем объект следующему конвейеру
                    nextConveyorScript.AcceptItem(currentItem);
                    currentItem = null;
                    isProcessing = false; // Освобождаем конвейер
                }
            }
        }

        // Если текущий объект завершил перемещение, берем следующий из очереди
        if (currentItem == null && waitingQueue.Count > 0 && !isProcessing)
        {
            currentItem = waitingQueue.Dequeue();
            isProcessing = true; // Начинаем обработку
        }
    }

    public bool CanAcceptItem()
    {
        // Проверяем, свободен ли конвейер
        return !isProcessing && currentItem == null;
    }

    public void AcceptItem(GameObject item)
    {
        // Добавляем объект в очередь
        waitingQueue.Enqueue(item);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Когда объект касается конвейера, принимаем его, если он принадлежит слою itemLayerMask
        if (IsInLayerMask(collision.gameObject, itemLayerMask))
        {
            AcceptItem(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Если объект покидает конвейер, отвязываем его
        if (IsInLayerMask(collision.gameObject, itemLayerMask))
        {
            if (currentItem == collision.gameObject)
            {
                currentItem = null;
                isProcessing = false; // Освобождаем место для следующего объекта
            }
            else if (waitingQueue.Contains(collision.gameObject))
            {
                RemoveFromQueue(collision.gameObject);
            }
        }
    }

    private void RemoveFromQueue(GameObject item)
    {
        // Удаляем объект из очереди
        Queue<GameObject> newQueue = new Queue<GameObject>();
        while (waitingQueue.Count > 0)
        {
            GameObject dequeued = waitingQueue.Dequeue();
            if (dequeued != item)
            {
                newQueue.Enqueue(dequeued);
            }
        }
        waitingQueue = newQueue;
    }

    private void FindNextConveyor()
    {
        // Поиск всех объектов в радиусе обнаружения
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, conveyorLayerMask);

        Transform closestConveyor = null;
        float closestDistance = Mathf.Infinity;

        foreach (var collider in colliders)
        {
            if (collider.transform == transform) continue; // Пропускаем себя

            Conveyor conveyor = collider.GetComponent<Conveyor>();
            if (conveyor == null) continue; // Убедимся, что это конвейер

            // Проверяем, направлены ли конвейеры в одну сторону
            Vector2 directionToConveyor = (conveyor.transform.position - transform.position).normalized;
            if (Vector2.Dot(transform.right, directionToConveyor) > 0.9f) // Угол меньше 25 градусов
            {
                float distance = Vector2.Distance(transform.position, conveyor.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestConveyor = conveyor.transform;
                }
            }
        }

        nextConveyor = closestConveyor;
    }

    private void OnDrawGizmosSelected()
    {
        // Отображение радиуса обнаружения в режиме редактирования
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((1 << obj.layer) & layerMask) != 0;
    }
}
