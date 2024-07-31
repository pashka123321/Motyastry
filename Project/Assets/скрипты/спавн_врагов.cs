using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    public GameObject[] enemyPrefabs; // Array of enemy prefabs
    public Transform player; // Reference to the player object
    public int enemiesPerWaveMin = 10; // Minimum number of enemies per wave
    public int enemiesPerWaveMax = 30; // Maximum number of enemies per wave
    public float waveInterval = 90.0f; // Interval between waves
    public float minDistanceFromPlayer = 20.0f; // Minimum distance from player
    public LayerMask blockLayer; // Layer for blocks where enemies can't spawn
    public Text enemyCountText; // Reference to the UI text component for enemy count

    public int maxEnemies = 50; // Maximum number of enemies in the game

    private int currentEnemies = 0; // Current number of enemies

    void Awake()
    {
        // Set up singleton
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
        // Start spawning enemy waves
        InvokeRepeating("SpawnWave", waveInterval, waveInterval);
        UpdateEnemyCountText();
    }

    void SpawnWave()
    {
        int enemiesToSpawn = Random.Range(enemiesPerWaveMin, enemiesPerWaveMax + 1);
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (currentEnemies < maxEnemies)
            {
                TrySpawnClone();
            }
        }
    }

    void TrySpawnClone()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();

        while (Vector3.Distance(spawnPosition, player.position) < minDistanceFromPlayer ||
               IsPositionOccupiedByBlock(spawnPosition))
        {
            spawnPosition = GetRandomSpawnPosition(); // Generate new position if too close to player or occupied by a block
        }

        GameObject selectedEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject clone = Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity);

        // Add EnemyTracker to the enemy clone
        clone.AddComponent<EnemyTracker>();

        // Set up the enemy (e.g., link to the player)
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
        // Check if the position is within a block collider
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
