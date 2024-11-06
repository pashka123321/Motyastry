using System.Collections;
using UnityEngine;
using Mirror;

public class mEnemySpawner : NetworkBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs; // Список префабов врагов
    [SerializeField] private float spawnInterval = 60f; // Интервал спавна в секундах
    [SerializeField] private Vector2 spawnRangeX = new Vector2(0, 200); // Диапазон по X
    [SerializeField] private Vector2 spawnRangeY = new Vector2(0, 200); // Диапазон по Y

    public override void OnStartServer()
    {
        // Запускаем повторяющийся вызов спавна
        InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
    }

    [Server]
    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        // Выбираем случайный префаб врага
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // Генерируем случайную позицию для спавна
        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnRangeX.x, spawnRangeX.y),
            Random.Range(spawnRangeY.x, spawnRangeY.y),
            0
        );

        // Создаем врага на сервере
        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(enemyInstance); // Спавн объекта на всех клиентах
    }
}
