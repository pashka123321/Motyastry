using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    // Ссылка на кнопку пропуска времени ожидания
    public Button skipWaveButton;

    [System.Serializable]
    public class EnemyPrefab
    {
        public GameObject prefab;
        public bool isBoss;
    }

    public EnemyPrefab[] enemyPrefabs;
    public Transform player;
    public int initialEnemiesPerWave = 2;
    public int enemyIncrementPerWave = 2;
    public float waveInterval = 90.0f;
    public float restInterval = 120.0f;
    public float minDistanceFromPlayer = 20.0f;
    public LayerMask blockLayer;
    public Text enemyCountText;
    public Text waveCountText;
    public Text waveTimerText;

    public int maxEnemies = 50;

    private int currentEnemies = 0;
    private int currentWave = 0;
    private int enemiesPerWave;
    private bool isResting = false;
    private bool isWaveActive = false;
    private float timeUntilNextWave;

    void Awake()
    {
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
        enemiesPerWave = initialEnemiesPerWave;
        timeUntilNextWave = waveInterval;

        UpdateEnemyCountText();
        UpdateWaveCountText();
        UpdateWaveTimerText();

        // Привязываем метод SkipWave к кнопке
        skipWaveButton.onClick.AddListener(SkipWave);

        InvokeRepeating("UpdateWaveTimer", 1.0f, 1.0f);
    }

    void Update()
    {
        // Проверяем, нажата ли клавиша F7
        if (Input.GetKeyDown(KeyCode.F7))
        {
            ForceSkipWave();
        }
    }

    void UpdateWaveTimer()
    {
        if (currentEnemies > 0)
        {
            isWaveActive = true;
            waveTimerText.text = "Идет волна";
            return;
        }

        isWaveActive = false;
        timeUntilNextWave -= 1.0f;

        if (timeUntilNextWave <= 0)
        {
            SpawnWave();
            timeUntilNextWave = waveInterval;
        }

        UpdateWaveTimerText();
    }

    public void SkipWave()
    {
        if (currentEnemies > 0)
        {
            // Если есть оставшиеся враги, нельзя пропустить волну через UI
            return;
        }

        ForceSkipWave(); // Используем метод принудительного пропуска волны
    }

    void ForceSkipWave()
    {
        // Удаляем всех врагов на сцене
        foreach (EnemyRetreat enemy in FindObjectsOfType<EnemyRetreat>())
        {
            Destroy(enemy.gameObject);
        }

        // Сбрасываем количество текущих врагов
        currentEnemies = 0;
        UpdateEnemyCountText();

        // Останавливаем таймер волны, если он запущен
        CancelInvoke("UpdateWaveTimer");

        // Начинаем новую волну
        SpawnWave();

        // Сбрасываем таймер на стандартный интервал
        timeUntilNextWave = waveInterval;
        InvokeRepeating("UpdateWaveTimer", 1.0f, 1.0f);
    }

    void SpawnWave()
    {
        isWaveActive = true; // Устанавливаем флаг, что волна активна

        if (currentWave > 0 && currentWave % 5 == 0)
        {
            if (!isResting)
            {
                isResting = true;
                Invoke("EndRest", restInterval);
                timeUntilNextWave = restInterval;
                return;
            }
        }

        isResting = false;
        currentWave++;
        UpdateWaveCountText();

        enemiesPerWave = initialEnemiesPerWave + (enemyIncrementPerWave * currentWave / 5);

        if (currentWave % 5 == 0)
        {
            SpawnBoss();
        }
        else
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                if (currentEnemies < maxEnemies)
                {
                    TryActivateEnemy(false);
                }
            }
        }

        UpdateWaveTimerText(); // Обновляем текст таймера, чтобы показать "Идет волна"
    }

    void EndRest()
    {
        timeUntilNextWave = waveInterval;
        Invoke("SpawnWave", waveInterval);
    }

    void SpawnBoss()
    {
        foreach (var enemyPrefab in enemyPrefabs)
        {
            if (enemyPrefab.isBoss)
            {
                TryActivateEnemy(true, enemyPrefab.prefab);
                break;
            }
        }
    }

    void TryActivateEnemy(bool isBossWave, GameObject specificEnemyPrefab = null)
    {
        GameObject enemyPrefab;

        if (isBossWave && specificEnemyPrefab != null)
        {
            enemyPrefab = specificEnemyPrefab;
        }
        else
        {
            enemyPrefab = GetRandomNonBossEnemyPrefab();
        }

        if (enemyPrefab == null) return;

        // Создаем новый клон врага
        GameObject enemy = Instantiate(enemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);

        // Включаем клон
        enemy.SetActive(true);

        // Добавляем скрипт отслеживания если его нет
        if (enemy.GetComponent<EnemyTracker>() == null)
        {
            enemy.AddComponent<EnemyTracker>();
        }

        // Добавляем систему отсчета времени до отступления
        EnemyRetreat enemyRetreat = enemy.AddComponent<EnemyRetreat>();
        enemyRetreat.SetSpawner(this); // Передаем ссылку на спаунер для обратного вызова

        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.isClone = true;
            enemyAI.player = player;
        }
        else
        {
            EnemyAI2D enemyAI2D = enemy.GetComponent<EnemyAI2D>();
            if (enemyAI2D != null)
            {
                enemyAI2D.isClone = true;
                enemyAI2D.player = player;
            }
        }

        currentEnemies++; // Увеличиваем количество врагов
        UpdateEnemyCountText(); // Обновляем текст количества врагов
    }

    GameObject GetRandomNonBossEnemyPrefab()
    {
        var nonBossEnemies = new System.Collections.Generic.List<GameObject>();

        foreach (var enemyPrefab in enemyPrefabs)
        {
            if (!enemyPrefab.isBoss)
            {
                nonBossEnemies.Add(enemyPrefab.prefab);
            }
        }

        if (nonBossEnemies.Count > 0)
        {
            return nonBossEnemies[Random.Range(0, nonBossEnemies.Count)];
        }

        return null;
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
        Collider2D collider = Physics2D.OverlapPoint(position, blockLayer);
        return collider != null;
    }

    void UpdateEnemyCountText()
    {
        if (currentEnemies <= 0)
        {
            enemyCountText.text = "Врагов: нет";
        }
        else
        {
            enemyCountText.text = "Врагов: " + currentEnemies.ToString();
        }
    }

    void UpdateWaveCountText()
    {
        waveCountText.text = "Волны: " + currentWave.ToString();
    }

    void UpdateWaveTimerText()
    {
        if (isWaveActive)
        {
            waveTimerText.text = "Идет волна";
        }
        else
        {
            int minutes = Mathf.FloorToInt(timeUntilNextWave / 60f);
            int seconds = Mathf.FloorToInt(timeUntilNextWave % 60f);

            string timeFormatted = string.Format("{0:00}:{1:00}", minutes, seconds);
            waveTimerText.text = "До волны " + timeFormatted;
        }
    }

    public void OnEnemyDestroyed()
    {
        currentEnemies--;
        UpdateEnemyCountText();

        if (currentEnemies <= 0 && !isResting)
        {
            isWaveActive = false; // Если все враги уничтожены, волна завершена
            UpdateWaveTimerText(); // Обновляем текст таймера
        }
    }

    public void OnEnemyRetreat(GameObject enemy)
    {
        Destroy(enemy);
        currentEnemies--;
        UpdateEnemyCountText();

        if (currentEnemies <= 0 && !isResting)
        {
            isWaveActive = false;
            UpdateWaveTimerText();
        }
    }
}

// Новый скрипт EnemyRetreat
public class EnemyRetreat : MonoBehaviour
{
    private EnemySpawner spawner;
    private float retreatTime = 120.0f; // Время до отступления
    private float timeElapsed = 0.0f;

    public void SetSpawner(EnemySpawner spawner)
    {
        this.spawner = spawner;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= retreatTime)
        {
            // Уведомляем спаунер, что враг отступил
            spawner.OnEnemyRetreat(gameObject);
        }
    }
}
