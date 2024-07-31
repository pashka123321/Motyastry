using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [System.Serializable]
    public class LandscapePrefab
    {
        public GameObject prefab;
        public float perlinScaleX = 20f;
        public float perlinScaleY = 20f;
        public float threshold = 0.5f;
        public bool isBorder;
    }

    public LandscapePrefab[] landscapePrefabs;
    public int mapWidth = 200;
    public int mapHeight = 200;
    public Vector2 perlinOffset;
    public float biomeScale = 50f;
    public int exclusionRadius = 25;
    public int transitionRadius = 10;

    private int seed;

    void Start()
    {
        if (PlayerPrefs.HasKey("Seed"))
        {
            seed = PlayerPrefs.GetInt("Seed");
            Random.InitState(seed);
        }
        else
        {
            Debug.LogError("No seed found!");
            seed = Random.Range(0, 1000000);
            Random.InitState(seed);
        }

        // Для стабильности значений Perlin noise, инициализируем смещение с использованием seed
        perlinOffset = new Vector2(seed % 1000, seed % 1000);
        GenerateLandscapes();
    }

    void GenerateLandscapes()
    {
        // Очищаем старые объекты
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float distanceFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));

                if (distanceFromCenter <= exclusionRadius)
                {
                    continue;
                }

                float biomeValue = Mathf.PerlinNoise((x + perlinOffset.x) / biomeScale, (y + perlinOffset.y) / biomeScale);
                int biomeIndex = Mathf.FloorToInt(biomeValue * landscapePrefabs.Length);

                // Обеспечиваем, что индекс всегда в пределах допустимых значений
                biomeIndex = Mathf.Clamp(biomeIndex, 0, landscapePrefabs.Length - 1);

                var landscapePrefab = landscapePrefabs[biomeIndex];
                float perlinValue = Mathf.PerlinNoise((x + perlinOffset.x) / landscapePrefab.perlinScaleX, (y + perlinOffset.y) / landscapePrefab.perlinScaleY);

                if (distanceFromCenter <= exclusionRadius + transitionRadius)
                {
                    perlinValue *= Mathf.InverseLerp(exclusionRadius, exclusionRadius + transitionRadius, distanceFromCenter);
                }

                if (perlinValue > landscapePrefab.threshold)
                {
                    Vector3 position = new Vector3(x, y, 0);
                    Instantiate(landscapePrefab.prefab, position, Quaternion.identity, transform);
                    AddBorderBlocks(x, y);
                }
            }
        }
    }

    void AddBorderBlocks(int x, int y)
    {
        Vector2[] directions = {
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(-1, 0)
        };

        foreach (var direction in directions)
        {
            int newX = x + (int)direction.x;
            int newY = y + (int)direction.y;

            if (newX >= 0 && newX < mapWidth && newY >= 0 && newY < mapHeight)
            {
                Collider2D collider = Physics2D.OverlapPoint(new Vector2(newX, newY));
                if (collider == null)
                {
                    foreach (var prefab in landscapePrefabs)
                    {
                        if (prefab.isBorder)
                        {
                            Vector3 position = new Vector3(newX, newY, 0);
                            Instantiate(prefab.prefab, position, Quaternion.identity, transform);
                            break;
                        }
                    }
                }
            }
        }
    }
}