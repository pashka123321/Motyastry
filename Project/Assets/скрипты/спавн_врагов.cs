using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Префаб врага
    public Transform player; // Ссылка на объект игрока
    public int maxEnemies = 10; // Максимальное количество врагов
    public float spawnInterval = 2.0f; // Интервал между спаунами
    public float minDistanceFromPlayer = 20.0f; // Минимальное расстояние от игрока

    private int currentEnemies = 0; // Текущее количество врагов

    void Start()
    {
        // Начинаем спаунить врагов с указанным интервалом
        InvokeRepeating("TrySpawnClone", spawnInterval, spawnInterval);
    }

    void TrySpawnClone()
    {
        if (currentEnemies >= maxEnemies)
            return;

        Vector3 spawnPosition = GetRandomSpawnPosition();

        // Проверяем расстояние до игрока
        while (Vector3.Distance(spawnPosition, player.position) < minDistanceFromPlayer)
        {
            spawnPosition = GetRandomSpawnPosition(); // Генерируем новую позицию, если слишком близко к игроку
        }

        GameObject clone = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        // Проверяем, какой компонент EnemyAI установлен на префабе
        EnemyAI enemyAI = clone.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.isClone = true;
            enemyAI.player = player;
        }
        else
        {
            // Если не найден EnemyAI, попробуем получить EnemyAI2D
            EnemyAI2D enemyAI2D = clone.GetComponent<EnemyAI2D>();
            if (enemyAI2D != null)
            {
                enemyAI2D.isClone = true;
                enemyAI2D.player = player;
            }
            else
            {
                //Debug.LogError("Neither EnemyAI nor EnemyAI2D component found on enemyPrefab or its children.");
            }
        }

        currentEnemies++;
    }

    Vector3 GetRandomSpawnPosition()
    {
        float x, y;

        do
        {
            x = Random.Range(0, 200);
            y = Random.Range(0, 200);
        }
        while (x >= 75 && x <= 100 && y >= 75 && y <= 100);

        return new Vector3(x, y, 0);
    }
}
