using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject bushPrefab;
    public int bushCount;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;

    // Создайте переменные для слоев
    public LayerMask blockedLayers;

    void Start()
    {
        StartCoroutine(SpawnObjectsWithDelay(0.5f));
    }

    IEnumerator SpawnObjectsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnObjects(bushPrefab, bushCount);
    }

    void SpawnObjects(GameObject prefab, int count)
    {
        int spawned = 0;
        while (spawned < count)
        {
            Vector2 spawnPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            if (!IsPositionBlocked(spawnPosition))
            {
                Instantiate(prefab, spawnPosition, Quaternion.identity);
                spawned++;
                Debug.Log($"Spawned at {spawnPosition}");
            }
            else
            {
                Debug.Log($"Position {spawnPosition} is blocked by Ore or Block");
            }
        }
    }

    bool IsPositionBlocked(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f, blockedLayers);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.layer == 3 || collider.gameObject.layer == 7 || collider.gameObject.layer == 16)
            {
                Debug.Log($"Position {position} blocked by {collider.tag}");
                return true;
            }
        }
        return false;
    }
}
