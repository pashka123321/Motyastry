using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    public GameObject[] enemyPrefabs; // Массив префабов врагов
    public Transform player; // Ссылка на объект игрока
    public int maxEnemies = 10; // Максимальное количество врагов
    public float spawnInterval = 2.0f; // Интервал между спаунами
    public float minDistanceFromPlayer = 20.0f; // Минимальное расстояние от игрока
    public LayerMask blockLayer; // Слой для блоков, где нельзя спаунить врагов
    public Text enemyCountText; // Ссылка на текстовый компонент UI

    private int currentEnemies = 0; // Текущее количество врагов

    void Awake()
    {
        // Настраиваем singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Начинаем спаунить врагов с указанным интервалом
        InvokeRepeating("TrySpawnClone", spawnInterval, spawnInterval);
        UpdateEnemyCountText();
    }

    void TrySpawnClone()
    {
        if (currentEnemies >= maxEnemies)
            return;

        Vector3 spawnPosition = GetRandomSpawnPosition();

        while (Vector3.Distance(spawnPosition, player.position) < minDistanceFromPlayer ||
               IsPositionOccupiedByBlock(spawnPosition))
        {
            spawnPosition = GetRandomSpawnPosition(); // Генерируем новую позицию, если слишком близко к игроку или занята блоком
        }

        GameObject selectedEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject clone = Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity);

        // Добавляем EnemyTracker к клону врага
        clone.AddComponent<EnemyTracker>();

        // Настраиваем врага (например, связываем с игроком)
        EnemyAI enemyAI = clone.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.isClone = true;
            enemyAI.player = player;
        }
        else
        {
            EnemyAI2D enemyAI2D = clone.GetComponent<EnemyAI2D>();
            if (enemyAI2D != null)
            {
                enemyAI2D.isClone = true;
                enemyAI2D.player = player;
            }
        }

        currentEnemies++;
        UpdateEnemyCountText();
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

    bool IsPositionOccupiedByBlock(Vector3 position)
    {
        // Проверяем, находится ли позиция в коллайдере блока
        Collider2D collider = Physics2D.OverlapPoint(position, blockLayer);
        return collider != null;
    }

    void UpdateEnemyCountText()
    {
        enemyCountText.text = "Врагов: " + currentEnemies.ToString();
    }

    public void OnEnemyDestroyed()
    {
        currentEnemies--;
        UpdateEnemyCountText();
    }
}