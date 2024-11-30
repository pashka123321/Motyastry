using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Router : MonoBehaviour
{
    public float speed = 2f; // Скорость перемещения объекта
    public float detectionRadius = 2f; // Радиус поиска следующего конвейера
    public LayerMask conveyorLayerMask; // Слой конвейеров
    public LayerMask itemLayerMask; // Слой объектов для маршрутизации

    public Transform leftTrigger, rightTrigger, topTrigger, bottomTrigger; // Ссылки на триггеры

    private List<GameObject> currentItems = new List<GameObject>(); // Список обрабатываемых объектов
    private Dictionary<GameObject, Transform> targetConveyors = new Dictionary<GameObject, Transform>(); // Целевые конвейеры
    private Dictionary<GameObject, string> entrySides = new Dictionary<GameObject, string>(); // Сторона входа ресурса
    private int currentDirectionIndex = 0; // Индекс текущего направления

    private string[] directions = { "Left", "Top", "Right", "Bottom" }; // Последовательность направлений

    private void Start()
    {
        // Запускаем корутину для проверки объектов каждые 1 секунду
        StartCoroutine(CheckItemsInColliderRoutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, itemLayerMask) && !currentItems.Contains(collision.gameObject))
        {
            AddItemToRouting(collision.gameObject);
        }
    }

    private void Update()
    {
        for (int i = currentItems.Count - 1; i >= 0; i--)
        {
            GameObject item = currentItems[i];
            if (targetConveyors.ContainsKey(item) && targetConveyors[item] != null)
            {
                MoveItemToTarget(item, targetConveyor: targetConveyors[item]);
            }
        }
    }

    private void AddItemToRouting(GameObject item)
    {
        currentItems.Add(item);
        item.transform.parent = transform;

        // Определяем, через какой триггер вошел ресурс
        string entrySide = DetectEntrySide(item.transform.position);
        entrySides[item] = entrySide;

        SelectConveyorByRotation(item, entrySide);
    }

    private void MoveItemToTarget(GameObject item, Transform targetConveyor)
    {
        item.transform.position = Vector3.MoveTowards(
            item.transform.position,
            targetConveyor.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(item.transform.position, targetConveyor.position) < 0.1f)
        {
            item.transform.parent = null;
            currentItems.Remove(item);
            targetConveyors.Remove(item);
            entrySides.Remove(item);
        }
    }

    private string DetectEntrySide(Vector2 itemPosition)
    {
        float leftDist = Vector2.Distance(itemPosition, leftTrigger.position);
        float rightDist = Vector2.Distance(itemPosition, rightTrigger.position);
        float topDist = Vector2.Distance(itemPosition, topTrigger.position);
        float bottomDist = Vector2.Distance(itemPosition, bottomTrigger.position);

        float minDistance = Mathf.Min(leftDist, rightDist, topDist, bottomDist);

        if (minDistance == leftDist) return "Left";
        if (minDistance == rightDist) return "Right";
        if (minDistance == topDist) return "Top";
        return "Bottom";
    }

    private void SelectConveyorByRotation(GameObject item, string entrySide)
    {
        Collider2D[] conveyors = Physics2D.OverlapCircleAll(transform.position, detectionRadius, conveyorLayerMask);
        List<Transform> validConveyors = new List<Transform>();

        // Циклический выбор направления, пропуская сторону входа
        for (int i = 0; i < directions.Length; i++)
        {
            currentDirectionIndex = (currentDirectionIndex + 1) % directions.Length;
            string targetDirection = directions[currentDirectionIndex];

            // Пропускаем сторону входа
            if (targetDirection == entrySide) continue;

            foreach (var conveyor in conveyors)
            {
                Vector2 directionToConveyor = (conveyor.transform.position - transform.position).normalized;

                switch (targetDirection)
                {
                    case "Left":
                        if (directionToConveyor.x < -0.5f) validConveyors.Add(conveyor.transform);
                        break;
                    case "Right":
                        if (directionToConveyor.x > 0.5f) validConveyors.Add(conveyor.transform);
                        break;
                    case "Top":
                        if (directionToConveyor.y > 0.5f) validConveyors.Add(conveyor.transform);
                        break;
                    case "Bottom":
                        if (directionToConveyor.y < -0.5f) validConveyors.Add(conveyor.transform);
                        break;
                }
            }

            if (validConveyors.Count > 0) break; // Нашли подходящий конвейер, выходим из цикла
        }

        if (validConveyors.Count > 0)
        {
            targetConveyors[item] = validConveyors[0]; // Выбираем первый подходящий конвейер
        }
        else
        {
            item.transform.parent = null;
            currentItems.Remove(item);
            entrySides.Remove(item);
        }
    }

private IEnumerator CheckItemsInColliderRoutine()
{
    while (true)
    {
        yield return new WaitForSeconds(1f); // Интервал проверки - 1 секунда

        // Получаем коллайдер текущего объекта
        Collider2D currentCollider = GetComponent<Collider2D>();
        if (currentCollider == null) yield break;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(itemLayerMask);
        filter.useTriggers = true;

        Collider2D[] results = new Collider2D[10]; // Максимальное число объектов для проверки
        int count = currentCollider.OverlapCollider(filter, results);

        for (int i = 0; i < count; i++)
        {
            GameObject item = results[i].gameObject;

            if (!currentItems.Contains(item))
            {
                AddItemToRouting(item);
            }
        }

        // Удаляем объекты, которые больше не активны или вышли из коллайдера
        for (int i = currentItems.Count - 1; i >= 0; i--)
        {
            GameObject item = currentItems[i];
            if (!item || !item.activeInHierarchy || !IsInCollider(item, currentCollider))
            {
                currentItems.RemoveAt(i);
                targetConveyors.Remove(item);
                entrySides.Remove(item);
            }
        }
    }
}

// Проверка, находится ли объект внутри текущего коллайдера
private bool IsInCollider(GameObject item, Collider2D currentCollider)
{
    Collider2D itemCollider = item.GetComponent<Collider2D>();
    return itemCollider != null && currentCollider.IsTouching(itemCollider);
}



    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((1 << obj.layer) & layerMask) != 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
