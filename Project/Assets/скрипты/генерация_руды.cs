using UnityEngine;
using System.Collections.Generic;

public class OreSpawner : MonoBehaviour
{
    [System.Serializable]
    public class OreType
    {
        public GameObject[] orePrefabs;  // Массив префабов для разнообразия
    }

    public OreType[] oreTypes;  // Массив типов руды
    public int mapWidth = 100;
    public int mapHeight = 100;
    public int oreClusterSizeMin = 10;
    public int oreClusterSizeMax = 20;
    public int clusterSpacing = 10;
    public float oreHeightMin = 1f;  // Минимальная высота руды
    public float oreHeightMax = 5f;  // Максимальная высота руды

    private List<Vector2Int> directions = new List<Vector2Int> {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    void Start()
    {
        SpawnOreClusters();
    }

    void SpawnOreClusters()
    {
        List<Vector2Int> usedPositions = new List<Vector2Int>();
        int clusterCount = (mapWidth * mapHeight) / (oreClusterSizeMax * clusterSpacing);

        for (int i = 0; i < clusterCount; i++)
        {
            Vector2Int clusterPosition = new Vector2Int(
                Random.Range(0, mapWidth),
                Random.Range(0, mapHeight)
            );

            if (IsValidClusterPosition(clusterPosition, usedPositions))
            {
                int clusterSize = Random.Range(oreClusterSizeMin, oreClusterSizeMax + 1);
                CreateOreCluster(clusterPosition, clusterSize, usedPositions);
            }
        }
    }

    bool IsValidClusterPosition(Vector2Int position, List<Vector2Int> usedPositions)
    {
        foreach (Vector2Int usedPosition in usedPositions)
        {
            if (Vector2Int.Distance(position, usedPosition) < clusterSpacing)
            {
                return false;
            }
        }
        return true;
    }

    void CreateOreCluster(Vector2Int start, int size, List<Vector2Int> usedPositions)
    {
        // Выбираем тип руды для всего кластера
        OreType oreType = oreTypes[Random.Range(0, oreTypes.Length)];

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        List<Vector2Int> clusterPositions = new List<Vector2Int>();
        queue.Enqueue(start);
        clusterPositions.Add(start);
        usedPositions.Add(start);

        float maxHeight = Random.Range(oreHeightMin, oreHeightMax); // Случайная высота кластера
        InstantiateOre(oreType, new Vector3(start.x, start.y, Random.Range(0, maxHeight)));

        while (queue.Count > 0 && clusterPositions.Count < size)
        {
            Vector2Int current = queue.Dequeue();
            List<Vector2Int> validNeighbors = new List<Vector2Int>();

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;
                if (next.x >= 0 && next.x < mapWidth &&
                    next.y >= 0 && next.y < mapHeight &&
                    !usedPositions.Contains(next))
                {
                    validNeighbors.Add(next);
                }
            }

            if (validNeighbors.Count > 0)
            {
                Vector2Int next = validNeighbors[Random.Range(0, validNeighbors.Count)];
                queue.Enqueue(next);
                clusterPositions.Add(next);
                usedPositions.Add(next);

                // Генерируем случайную высоту для каждого нового блока руды
                float height = Random.Range(0, maxHeight);
                InstantiateOre(oreType, new Vector3(next.x, next.y, height));
            }
        }
    }

    void InstantiateOre(OreType oreType, Vector3 position)
    {
        // Выбираем случайный префаб из списка и создаем его
        GameObject orePrefab = oreType.orePrefabs[Random.Range(0, oreType.orePrefabs.Length)];
        Instantiate(orePrefab, position, Quaternion.identity);
    }
}
