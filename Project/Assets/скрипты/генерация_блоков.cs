using UnityEngine;
using System.Collections;

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
        public GameObject shadowPrefab; // Префаб для тени
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

        perlinOffset = new Vector2(seed % 1000, seed % 1000);
        GenerateLandscapes();
        UpdateShadowLayers(); // Обновляем слои теней после генерации всех блоков
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
                    GameObject block = Instantiate(landscapePrefab.prefab, position, Quaternion.identity, transform);
                    block.name = $"Wall_{x}_{y}";

                    AddShadow(block, x, y, distanceFromCenter, centerX, centerY);
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
                            GameObject borderBlock = Instantiate(prefab.prefab, position, Quaternion.identity, transform);
                            borderBlock.name = $"Border_{newX}_{newY}";
                            break;
                        }
                    }
                }
            }
        }
    }

    void AddShadow(GameObject block, int x, int y, float distanceFromCenter, int centerX, int centerY)
    {
        var landscapePrefab = GetLandscapePrefabForPosition(x, y);
        if (landscapePrefab.shadowPrefab != null)
        {
            Vector3 position = block.transform.position;
            GameObject shadow = Instantiate(landscapePrefab.shadowPrefab, position, Quaternion.identity, block.transform);
            shadow.name = $"Shadow_{x}_{y}";

            // Настройка слоя и сортировки теней
            SpriteRenderer shadowRenderer = shadow.GetComponent<SpriteRenderer>();
            shadowRenderer.sortingOrder = 0; // Начальное значение, будет изменено позже

            // Изначально скрываем тень
            shadowRenderer.enabled = false;

            // Запускаем корутину для появления тени
            StartCoroutine(ShowShadowAfterDelay(shadowRenderer, 1.6f));

            // Расчет коэффициента для затемнения тени
            float maxDistance = Mathf.Max(centerX, centerY);
            float shadowDarkness = Mathf.Lerp(0.5f, 1f, distanceFromCenter / maxDistance); // Чем ближе к центру, тем темнее
            Color shadowColor = shadowRenderer.color;
            shadowColor.a = shadowDarkness;
            shadowRenderer.color = shadowColor;
        }
    }

    IEnumerator ShowShadowAfterDelay(SpriteRenderer shadowRenderer, float delay)
    {
        yield return new WaitForSeconds(delay);
        shadowRenderer.enabled = true;
    }

    void UpdateShadowLayers()
    {
        // Проходим по всем дочерним объектам, чтобы найти и обновить слои теней
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Wall_"))
            {
                string[] coordinates = child.name.Split('_');
                int x = int.Parse(coordinates[1]);
                int y = int.Parse(coordinates[2]);

                Transform shadowTransform = child.Find($"Shadow_{x}_{y}");
                if (shadowTransform != null && IsDeepInsideWall(x, y))
                {
                    SpriteRenderer shadowRenderer = shadowTransform.GetComponent<SpriteRenderer>();
                    shadowRenderer.sortingOrder = 3; // Тень будет отображаться поверх блока

                    // Изменение масштаба для теней внутри блоков
                    shadowTransform.localScale = new Vector3(0.25f, 0.25f, 1f);
                }
            }
        }
    }

    bool IsDeepInsideWall(int x, int y)
    {
        Vector2[] directions = {
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(-1, 0)
        };

        // Проверяем, есть ли минимум два блока между текущим блоком и внешними пустыми блоками
        int outerWallThickness = 2;
        foreach (var direction in directions)
        {
            int distance = 1;
            while (distance <= outerWallThickness)
            {
                int newX = x + (int)direction.x * distance;
                int newY = y + (int)direction.y * distance;

                if (newX < 0 || newX >= mapWidth || newY < 0 || newY >= mapHeight)
                    return false;

                Collider2D collider = Physics2D.OverlapPoint(new Vector2(newX, newY));
                if (collider == null) // Если достигли пустого пространства
                    return false;

                distance++;
            }
        }

        return true;
    }

    LandscapePrefab GetLandscapePrefabForPosition(int x, int y)
    {
        float biomeValue = Mathf.PerlinNoise((x + perlinOffset.x) / biomeScale, (y + perlinOffset.y) / biomeScale);
        int biomeIndex = Mathf.FloorToInt(biomeValue * landscapePrefabs.Length);
        biomeIndex = Mathf.Clamp(biomeIndex, 0, landscapePrefabs.Length - 1);

        return landscapePrefabs[biomeIndex];
    }
}
