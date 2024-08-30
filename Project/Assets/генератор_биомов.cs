using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public int width = 100; // Ширина карты
    public int height = 100; // Высота карты
    public float scale = 20f; // Масштаб для шума Перлина

    // Структура для хранения информации о биоме
    [System.Serializable]
    public class Biome
    {
        public string name; // Название биома
        public GameObject prefab; // Префаб биома
        public GameObject borderPrefab; // Префаб границы
        public float percentage; // Процент биома на карте
    }

    public Biome[] biomes; // Массив биомов

    private int[,] map; // Массив для хранения типов биомов

    void Start()
    {
        map = new int[width, height]; // Инициализируем массив карты
        NormalizeBiomePercentages();
        GenerateWorld();
        DrawWorld();
    }

    // Нормализация процентов биомов
    void NormalizeBiomePercentages()
    {
        float total = 0f;
        foreach (Biome biome in biomes)
        {
            total += biome.percentage;
        }
        foreach (Biome biome in biomes)
        {
            biome.percentage /= total; // Нормализуем значения, чтобы сумма была равна 1
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
        for (int i = 0; i < biomes.Length; i++)
        {
            cumulative += biomes[i].percentage;
            if (value <= cumulative)
            {
                return i;
            }
        }
        return biomes.Length - 1; // Если вдруг value больше, чем cumulative (в теории быть не должно)
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
        // Отображение карты мира с использованием префабов
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int biomeIndex = map[x, y];
                if (biomeIndex >= 0 && biomeIndex < biomes.Length)
                {
                    Biome biome = biomes[biomeIndex];
                    Vector3 position = new Vector3(x, y, 0);

                    // Создаем игровой объект из префаба
                    GameObject biomeObject = Instantiate(biome.prefab, position, Quaternion.identity);
                    biomeObject.transform.parent = transform;

                    // Если блок находится на границе, создаем объект для границы
                    if (IsBiomeBorder(x, y) && biome.borderPrefab != null)
                    {
                        GameObject borderObject = Instantiate(biome.borderPrefab, position, Quaternion.identity);
                        borderObject.transform.parent = biomeObject.transform; // Добавляем как дочерний объект
                    }
                }
            }
        }
    }
}
