using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class WorldGenerator : MonoBehaviour
{
    public int width = 100; // Ширина карты
    public int height = 100; // Высота карты
    public float scale = 20f; // Масштаб для шума Перлина
    public int numBiomes = 4; // Количество различных биомов

    public Tilemap tilemap; // Ссылка на Tilemap, которая будет использоваться

    // Массивы для хранения данных о биомах
    public BiomeData[] biomes;

    private int[,] map; // Массив для хранения типов биомов

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

    void Start()
    {
        // Настройка слоя Tilemap на -1
        tilemap.GetComponent<TilemapRenderer>().sortingOrder = -5;

        map = new int[width, height]; // Инициализируем массив карты

        // Проверка, чтобы количество биомов не превышало доступное количество данных
        if (numBiomes > biomes.Length)
        {
            numBiomes = biomes.Length; // Ограничиваем количество биомов количеством доступных данных
        }

        NormalizeBiomePercentages();
        GenerateWorld();
        DrawWorld();
        GenerateFauna(); // Вызываем метод для генерации фауны
    }

    void NormalizeBiomePercentages()
    {
        float total = 0f;
        for (int i = 0; i < numBiomes; i++)
        {
            total += biomes[i].percentage;
        }

        for (int i = 0; i < numBiomes; i++)
        {
            biomes[i].percentage /= total; // Нормализуем значения, чтобы сумма была равна 1
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
        for (int i = 0; i < numBiomes; i++)
        {
            cumulative += biomes[i].percentage;
            if (value <= cumulative)
            {
                return i;
            }
        }
        return numBiomes - 1; // Если вдруг value больше, чем cumulative (в теории быть не должно)
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
                if (biomeIndex >= 0 && biomeIndex < biomes.Length)
                {
                    Tile[] tiles = biomes[biomeIndex].biomeTiles;

                    if (tiles.Length > 0)
                    {
                        Tile originalTile = tiles[Random.Range(0, tiles.Length)];

                        // Создаем временный тайл для изменения его цвета
                        Tile tileToSet = ScriptableObject.CreateInstance<Tile>();
                        tileToSet.sprite = originalTile.sprite;
                        tileToSet.color = originalTile.color; // Копируем цвет оригинального тайла
                        tileToSet.colliderType = originalTile.colliderType; // Копируем тип коллайдера

                        // Применяем постоянный цвет для биома
                        tileToSet.color = biomes[biomeIndex].biomeColor;

                        // Если блок находится на границе, изменяем его цвет
                        if (IsBiomeBorder(x, y))
                        {
                            tileToSet.color = biomes[biomeIndex].borderColor;
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
                if (biomeIndex >= 0 && biomeIndex < biomes.Length)
                {
                    BiomeData biome = biomes[biomeIndex];

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
}
