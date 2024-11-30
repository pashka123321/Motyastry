using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    [System.Serializable]
    public class OreToIngotMapping
    {
        public string oreTag;          // Тег руды
        public GameObject ingotPrefab; // Префаб слитка
    }

    public List<OreToIngotMapping> oreToIngotMappings; // Список сопоставлений руды и слитков
    public Transform[] spawnPoints;                   // Точки спавна слитка
    public bool[] activeSP;                           // Активность спавн-точек
    public float unloadInterval = 1.0f;               // Интервал выгрузки (в секундах)

    private Dictionary<string, int> inventory = new Dictionary<string, int>(); // Инвентарь
    private Dictionary<GameObject, GameObject> oreInputConveyors = new Dictionary<GameObject, GameObject>(); // Соответствие руды и конвейеров
    private bool isUnloading = false; // Флаг выгрузки
    private GameObject lastInputConveyor; // Последний конвейер, подавший ресурс

    private void Start()
    {
        // Запускаем периодическую выгрузку
        InvokeRepeating(nameof(UnloadInventory), unloadInterval, unloadInterval);
    }
private Dictionary<Transform, GameObject> inputConveyors = new Dictionary<Transform, GameObject>();

private void OnTriggerEnter2D(Collider2D collision)
{
    GameObject oreObject = collision.gameObject;

    foreach (var mapping in oreToIngotMappings)
    {
        if (oreObject.CompareTag(mapping.oreTag))
        {
            // Найти ближайший спавн-поинт
            Transform closestSpawnPoint = FindClosestSpawnPoint(oreObject.transform.position);

            // Найти входной конвейер
            GameObject inputConveyor = FindConveyorAtPosition(oreObject.transform.position);
            if (inputConveyor != null)
            {
                inputConveyors[closestSpawnPoint] = inputConveyor; // Сохраняем входной конвейер для этого спавн-поинта
            }

            Destroy(oreObject); // Удаляем руду из сцены

            // Перерабатываем руду
            ProcessOre(mapping.ingotPrefab, closestSpawnPoint, mapping.oreTag);
            return;
        }
    }
}


private void ProcessOre(GameObject ingotPrefab, Transform entryPoint, string oreTag)
{
    Transform exitPoint = FindExitPoint(entryPoint);

    if (exitPoint == null)
    {
        if (!inventory.ContainsKey(oreTag))
            inventory[oreTag] = 0;

        inventory[oreTag]++;
        Debug.Log($"Added {oreTag} to inventory. Total: {inventory[oreTag]}");
    }
    else
    {
        Instantiate(ingotPrefab, exitPoint.position, Quaternion.identity);
    }

    // Удаляем запись о входном конвейере
    if (inputConveyors.ContainsKey(entryPoint))
    {
        inputConveyors.Remove(entryPoint);
    }
}


    private Transform FindClosestSpawnPoint(Vector2 position)
    {
        Transform closestPoint = spawnPoints[0];
        float minDistance = Vector2.Distance(position, closestPoint.position);

        foreach (var point in spawnPoints)
        {
            float distance = Vector2.Distance(position, point.position);
            if (distance < minDistance)
            {
                closestPoint = point;
                minDistance = distance;
            }
        }

        return closestPoint;
    }

    private GameObject FindConveyorAtPosition(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f); // Радиус можно настроить
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Conveer"))
            {
                return collider.gameObject;
            }
        }

        return null;
    }

private Transform FindExitPoint(Transform entryPoint)
{
    if (entryPoint == null)
    {
        Debug.LogWarning("Entry point is null. Skipping exit point search.");
        return null;
    }

    List<Transform> availablePoints = new List<Transform>();

    for (int i = 0; i < spawnPoints.Length; i++)
    {
        if (spawnPoints[i] != entryPoint && activeSP[i])
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPoints[i].position, 0.5f); // Настройка радиуса
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Conveer"))
                {
                    GameObject conveyor = collider.gameObject;

                    // Исключаем входной конвейер и конвейеры, направленные на печь
                    if (inputConveyors.ContainsKey(entryPoint) && conveyor != inputConveyors[entryPoint] && !IsConveyorFacingFurnace(conveyor))
                    {
                        availablePoints.Add(spawnPoints[i]);
                    }
                    break;
                }
            }
        }
    }

    if (availablePoints.Count == 0)
    {
        Debug.LogWarning("No valid exit points available.");
        return null;
    }

    return availablePoints[Random.Range(0, availablePoints.Count)];
}

// Метод для проверки направления конвейера
private bool IsConveyorFacingFurnace(GameObject conveyor)
{
    Vector2 directionToFurnace = (transform.position - conveyor.transform.position).normalized;
    Vector2 conveyorForward = conveyor.transform.right.normalized; // Или другой осевой вектор

    float dotProduct = Vector2.Dot(directionToFurnace, conveyorForward);
    return dotProduct > 0.7f; // Пороговое значение для определения направления
}

private void UnloadInventory()
{
    if (isUnloading || inventory.Count == 0)
        return;

    isUnloading = true;

    foreach (var mapping in oreToIngotMappings)
    {
        if (inventory.ContainsKey(mapping.oreTag) && inventory[mapping.oreTag] > 0)
        {
            Transform exitPoint = FindExitPoint(null);
            if (exitPoint == null)
            {
                isUnloading = false;
                return;
            }

            // Выгружаем объект на конвейер
            Instantiate(mapping.ingotPrefab, exitPoint.position, Quaternion.identity);
            inventory[mapping.oreTag]--;

            Debug.Log($"Unloaded 1 {mapping.oreTag}. Remaining: {inventory[mapping.oreTag]}");

            if (inventory[mapping.oreTag] == 0)
                inventory.Remove(mapping.oreTag);

            break; // Выгружаем по одному за раз
        }
    }

    isUnloading = false;
}



    public void ActivateSpawnPoint(int spIndex)
    {
        if (spIndex >= 0 && spIndex < activeSP.Length)
        {
            activeSP[spIndex] = true;
        }
    }

    public void DeactivateSpawnPoint(int spIndex)
    {
        if (spIndex >= 0 && spIndex < activeSP.Length)
        {
            activeSP[spIndex] = false;
        }
    }
}
