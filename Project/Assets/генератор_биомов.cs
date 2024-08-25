using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    public int width = 100; // Ширина карты
    public int height = 100; // Высота карты
    public float scale = 20f; // Масштаб для шума Перлина
    public int numBiomes = 4; // Количество различных биомов

    public Tilemap tilemap; // Ссылка на Tilemap, которая будет использоваться
    public Tile[] biomeTiles; // Массив тайлов для разных биомов
    public Color[] biomeColors; // Массив постоянных цветов для каждого биома
    public Color[] biomeBorderColors; // Массив цветов границ для каждого биома

    public float[] biomePercentages = new float[] { 0.25f, 0.25f, 0.25f, 0.25f }; // Процент для каждого биома

    private int[,] map; // Массив для хранения типов биомов

    // Перечисление типов биомов для удобства
    private enum BiomeType { Water, Sand, Grass, Stone };

void Start()
{
    // Настройка слоя Tilemap на -1
    tilemap.GetComponent<TilemapRenderer>().sortingOrder = -1;
    
    map = new int[width, height]; // Инициализируем массив карты
    NormalizeBiomePercentages();
    GenerateWorld();
    DrawWorld();
}


    void NormalizeBiomePercentages()
    {
        float total = 0f;
        foreach (float percentage in biomePercentages)
        {
            total += percentage;
        }
        for (int i = 0; i < biomePercentages.Length; i++)
        {
            biomePercentages[i] /= total; // Нормализуем значения, чтобы сумма была равна 1
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
        for (int i = 0; i < biomePercentages.Length; i++)
        {
            cumulative += biomePercentages[i];
            if (value <= cumulative)
            {
                return i;
            }
        }
        return biomePercentages.Length - 1; // Если вдруг value больше, чем cumulative (в теории быть не должно)
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
                if (biomeIndex >= 0 && biomeIndex < biomeTiles.Length)
                {
                    Tile originalTile = biomeTiles[biomeIndex];

                    // Создаем временный тайл для изменения его цвета
                    Tile tileToSet = ScriptableObject.CreateInstance<Tile>();
                    tileToSet.sprite = originalTile.sprite;
                    tileToSet.color = originalTile.color; // Копируем цвет оригинального тайла
                    tileToSet.colliderType = originalTile.colliderType; // Копируем тип коллайдера

                    // Применяем постоянный цвет для биома
                    if (biomeIndex < biomeColors.Length)
                    {
                        tileToSet.color = biomeColors[biomeIndex];
                    }

                    // Если блок находится на границе, изменяем его цвет
                    if (IsBiomeBorder(x, y) && biomeIndex < biomeBorderColors.Length)
                    {
                        tileToSet.color = biomeBorderColors[biomeIndex]; // Применяем цвет границы
                    }

                    tilemap.SetTile(tilePosition, tileToSet); // Устанавливаем тайл на карте
                }
            }
        }
    }
}