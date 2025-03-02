using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class WorldGenerator : MonoBehaviour
{
    public int width = 100; // Ширина карты
    public int height = 100; // Высота карты
    public float scale = 20f; // Масштаб для шума Перлина

    public Tilemap tilemap; // Ссылка на Tilemap, которая будет использоваться

    // Массивы для хранения данных о типах миров и биомах
    public WorldTypeData[] worldTypes;

    private int[,] map; // Массив для хранения типов биомов
    private BiomeData[] selectedBiomes; // Выбранные биомы для текущего типа мира

    // Перечисление типов биомов для удобства
    private enum BiomeType { Water, Sand, Grass, Stone };

    [System.Serializable]
    public class FaunaData
    {
        public GameObject faunaPrefab; // Префаб фауны
        public float spawnChance; // Шанс появления данного префаба
    }

    [System.Serializable]
    public class BiomeData
    {
        public string name; // Название биома
        public Color biomeColor; // Цвет биома
        public Color borderColor; // Цвет границы биома
        public Tile[] biomeTiles; // Блоки, связанные с биомом
        public FaunaData[] faunaPrefabs; // Префабы, представляющие фауну с шансами появления
        public float percentage; // Процент присутствия биома
    }

    [System.Serializable]
    public class WorldTypeData
    {
        public string name; // Название типа мира
        public BiomeData[] biomes; // Биомы, связанные с этим типом мира
        public LandscapePrefab[] landscapePrefabs; // Префабы для ландшафта
    }

    [System.Serializable]
    public class LandscapePrefab // это скалы
    {
        public GameObject prefab;
        public float perlinScaleX = 20f;
        public float perlinScaleY = 20f;
        public float threshold = 0.5f;
        public bool isBorder;
        public float noiseLayer1Scale = 10f; // Новый параметр для первого слоя шума
        public float noiseLayer2Scale = 30f; // Новый параметр для второго слоя шума
        public float noiseLayer1Weight; // Вес первого слоя шума
        public float noiseLayer2Weight; // Вес второго слоя шума
    }

    public int mapWidth = 200;
    public int mapHeight = 200;
    public Vector2 perlinOffset;
    public float biomeScale = 50f;
    public int exclusionRadius = 25;
    public int transitionRadius = 10;

    private int seed;

    void Start()
    {
        // Инициализация значений noiseLayer1Weight и noiseLayer2Weight
        foreach (var worldType in worldTypes)
        {
            foreach (var landscapePrefab in worldType.landscapePrefabs)
            {
                landscapePrefab.noiseLayer1Weight = Random.Range(0.4f, 0.5f);
                landscapePrefab.noiseLayer2Weight = Random.Range(0.4f, 0.5f);
            }
        }

        // Настройка слоя Tilemap на -1
        tilemap.GetComponent<TilemapRenderer>().sortingOrder = -5;

        map = new int[width, height]; // Инициализируем массив карты

        // Выбираем случайный тип мира
        WorldTypeData selectedWorldType = worldTypes[Random.Range(0, worldTypes.Length)];
        selectedBiomes = selectedWorldType.biomes;

        // Передаем префабы ландшафта в WallGenerator
        LandscapePrefab[] landscapePrefabs = selectedWorldType.landscapePrefabs;

        NormalizeBiomePercentages();
        GenerateWorld();
        DrawWorld();
        GenerateFauna(); // Вызываем метод для генерации фауны

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
        GenerateLandscapes(landscapePrefabs);
    }

    void NormalizeBiomePercentages()
    {
        float total = 0f;
        for (int i = 0; i < selectedBiomes.Length; i++)
        {
            total += selectedBiomes[i].percentage;
        }

        for (int i = 0; i < selectedBiomes.Length; i++)
        {
            selectedBiomes[i].percentage /= total; // Нормализуем значения, чтобы сумма была равна 1
        }
    }

    void GenerateWorld()
    {
        float[,] noiseMap = GeneratePerlinNoiseMap(width, height, scale);

        // Генерация карты на основе значений шума Перлина
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float value = noiseMap[x, y];

                // Определение типа биома на основе значения шума
                int biomeIndex = GetBiomeIndex(value);
                map[x, y] = biomeIndex;
            }
        }
    }

    // Генерация карты шума Перлина
    float[,] GeneratePerlinNoiseMap(int width, int height, float scale)
    {
        float[,] noiseMap = new float[width, height];
        Vector2 offset = new Vector2(Random.Range(0f, 10000f), Random.Range(0f, 10000f));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sampleX = (x + offset.x) / scale;
                float sampleY = (y + offset.y) / scale;
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }

    // Определение типа биома на основе значения шума и процентов
    int GetBiomeIndex(float value)
    {
        float cumulative = 0f;
        for (int i = 0; i < selectedBiomes.Length; i++)
        {
            cumulative += selectedBiomes[i].percentage;
            if (value <= cumulative)
            {
                return i;
            }
        }
        return selectedBiomes.Length - 1; // Если вдруг value больше, чем cumulative (в теории быть не должно)
    }

    // Проверка, является ли текущий блок границей между биомами
    bool IsBiomeBorder(int x, int y)
    {
        int currentBiome = map[x, y];

        // Проверка соседних блоков
        if (x > 0 && map[x - 1, y] != currentBiome) return true;
        if (x < width - 1 && map[x + 1, y] != currentBiome) return true;
        if (y > 0 && map[x, y - 1] != currentBiome) return true;
        if (y < height - 1 && map[x, y + 1] != currentBiome) return true;

        return false;
    }

    void DrawWorld()
    {
        // Отображение карты мира с использованием Tilemap
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                int biomeIndex = map[x, y];
                if (biomeIndex >= 0 && biomeIndex < selectedBiomes.Length)
                {
                    Tile[] tiles = selectedBiomes[biomeIndex].biomeTiles;

                    if (tiles.Length > 0)
                    {
                        Tile originalTile = tiles[Random.Range(0, tiles.Length)];

                        // Создаем временный тайл для изменения его цвета
                        Tile tileToSet = ScriptableObject.CreateInstance<Tile>();
                        tileToSet.sprite = originalTile.sprite;
                        tileToSet.color = originalTile.color; // Копируем цвет оригинального тайла
                        tileToSet.colliderType = originalTile.colliderType; // Копируем тип коллайдера

                        // Применяем постоянный цвет для биома
                        tileToSet.color = selectedBiomes[biomeIndex].biomeColor;

                        // Если блок находится на границе, изменяем его цвет
                        if (IsBiomeBorder(x, y))
                        {
                            tileToSet.color = selectedBiomes[biomeIndex].borderColor;
                        }

                        tilemap.SetTile(tilePosition, tileToSet); // Устанавливаем тайл на карте
                    }
                }
            }
        }
    }

    // Метод для генерации объектов фауны
    void GenerateFauna()
    {
        List<Vector3> spawnedFaunaPositions = new List<Vector3>();
        float minDistance = 5f; // Минимальное расстояние между объектами фауны

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int biomeIndex = map[x, y];
                if (biomeIndex >= 0 && biomeIndex < selectedBiomes.Length)
                {
                    BiomeData biome = selectedBiomes[biomeIndex];

                    foreach (FaunaData faunaData in biome.faunaPrefabs)
                    {
                        if (Random.value <= faunaData.spawnChance)
                        {
                            Vector3 tileCenter = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + tilemap.cellSize / 2f;

                            // Проверяем расстояние до всех уже созданных объектов фауны
                            bool tooClose = false;
                            foreach (Vector3 pos in spawnedFaunaPositions)
                            {
                                if (Vector3.Distance(pos, tileCenter) < minDistance)
                                {
                                    tooClose = true;
                                    break;
                                }
                            }

                            // Если нет объектов слишком близко, создаем новый объект фауны
                            if (!tooClose)
                            {
                                Instantiate(faunaData.faunaPrefab, tileCenter, Quaternion.identity);
                                spawnedFaunaPositions.Add(tileCenter);
                            }
                        }
                    }
                }
            }
        }
    }

    void GenerateLandscapes(LandscapePrefab[] landscapePrefabs)
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
                float perlinValue1 = Mathf.PerlinNoise((x + perlinOffset.x) / landscapePrefab.noiseLayer1Scale, (y + perlinOffset.y) / landscapePrefab.noiseLayer1Scale);
                float perlinValue2 = Mathf.PerlinNoise((x + perlinOffset.x) / landscapePrefab.noiseLayer2Scale, (y + perlinOffset.y) / landscapePrefab.noiseLayer2Scale);

                float combinedPerlinValue = perlinValue1 * landscapePrefab.noiseLayer1Weight + perlinValue2 * landscapePrefab.noiseLayer2Weight;

                if (distanceFromCenter <= exclusionRadius + transitionRadius)
                {
                    combinedPerlinValue *= Mathf.InverseLerp(exclusionRadius, exclusionRadius + transitionRadius, distanceFromCenter);
                }

                if (combinedPerlinValue > landscapePrefab.threshold)
                {
                    Vector3 position = new Vector3(x, y, 0);
                    GameObject block = Instantiate(landscapePrefab.prefab, position, Quaternion.identity, transform);
                    block.name = $"Wall_{x}_{y}";

                    AddBorderBlocks(x, y, landscapePrefabs);
                }
            }
        }
    }

    void AddBorderBlocks(int x, int y, LandscapePrefab[] landscapePrefabs)
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

    LandscapePrefab GetLandscapePrefabForPosition(int x, int y, LandscapePrefab[] landscapePrefabs)
    {
        float biomeValue = Mathf.PerlinNoise((x + perlinOffset.x) / biomeScale, (y + perlinOffset.y) / biomeScale);
        int biomeIndex = Mathf.FloorToInt(biomeValue * landscapePrefabs.Length);
        biomeIndex = Mathf.Clamp(biomeIndex, 0, landscapePrefabs.Length - 1);

        return landscapePrefabs[biomeIndex];
    }
}
