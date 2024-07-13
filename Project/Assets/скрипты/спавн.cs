using UnityEngine;

public class DynamicSpawn : MonoBehaviour
{
    public GameObject playerPrefab; // Префаб игрока
    public LayerMask blockLayerMask; // Слой блоков
    public LayerMask avoidBlockLayerMask; // Слой блоков, которые нужно избегать
    public float spawnRadius = 10f; // Радиус поиска свободного места
    public int maxAttempts = 10; // Максимальное количество попыток спавна

    void Start()
    {
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        int attempt = 0;
        Vector2 spawnPosition = Vector2.zero;

        while (attempt < maxAttempts)
        {
            // Генерируем случайную точку в радиусе spawnRadius от текущей позиции
            spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

            // Проверяем, что в этой точке нет блоков и не попадаем на блоки, которые нужно избегать
            Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 0.5f, blockLayerMask);
            bool avoidBlockFound = Physics2D.OverlapCircle(spawnPosition, 0.5f, avoidBlockLayerMask);

            if (colliders.Length == 0 && !avoidBlockFound)
            {
                Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                return; // Выходим из метода, так как игрок заспавнен
            }

            attempt++;
        }

        Debug.LogWarning("Не удалось найти свободное место для спавна игрока.");
    }
}
