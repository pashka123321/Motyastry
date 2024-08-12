using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelWorld : MonoBehaviour
{
    public int worldSize = 100;  // Размер мира
    public GameObject pixelPrefab;
    public GameObject capitalPrefab;
    public float updateInterval = 0.01f; // Время между обновлениями

    private PixelType[,] world;
    private GameObject[,] worldObjects;
    private GameObject[,] capitalObjects;

    public enum PixelType { White, Blue, Red, Green, Orange }

    private int bluePower = 100;
    private int redPower = 100;
    private int greenPower = 100;
    private int orangePower = 100;

    private int maxPower = 10000;
    private int powerGainInterval = 1; // Интервал восстановления силы (секунд)

    private Dictionary<PixelType, Vector2Int> capitals = new Dictionary<PixelType, Vector2Int>();
    private Dictionary<PixelType, Queue<Vector2Int>> frontlines = new Dictionary<PixelType, Queue<Vector2Int>>();

    void Start()
    {
        InitializeWorld();
        StartCoroutine(UpdateWorld());
        StartCoroutine(RegeneratePower());
    }

    void InitializeWorld()
    {
        world = new PixelType[worldSize, worldSize];
        worldObjects = new GameObject[worldSize, worldSize];
        capitalObjects = new GameObject[worldSize, worldSize];

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                GameObject pixelObj = Instantiate(pixelPrefab, new Vector3(x, y, 0), Quaternion.identity);
                pixelObj.GetComponent<SpriteRenderer>().color = Color.white;
                world[x, y] = PixelType.White;
                worldObjects[x, y] = pixelObj;
            }
        }

        // Начальные пиксели и столицы
        InitializeStartingPixel(10, 10, PixelType.Blue, Color.blue);
        InitializeStartingPixel(90, 90, PixelType.Red, Color.red);
        InitializeStartingPixel(10, 90, PixelType.Green, Color.green);
        InitializeStartingPixel(90, 10, PixelType.Orange, new Color(1f, 0.5f, 0f));

        // Устанавливаем столицы и добавляем их в передовую
        SetCapital(10, 10, PixelType.Blue);
        SetCapital(90, 90, PixelType.Red);
        SetCapital(10, 90, PixelType.Green);
        SetCapital(90, 10, PixelType.Orange);
    }

    void InitializeStartingPixel(int x, int y, PixelType type, Color color)
    {
        world[x, y] = type;
        worldObjects[x, y].GetComponent<SpriteRenderer>().color = color;
    }

    void SetCapital(int x, int y, PixelType type)
    {
        capitals[type] = new Vector2Int(x, y);
        GameObject capitalObj = Instantiate(capitalPrefab, new Vector3(x, y, 0), Quaternion.identity);
        capitalObj.GetComponent<SpriteRenderer>().color = Color.black;
        capitalObjects[x, y] = capitalObj;

        // Добавляем столицу в очередь передовой
        frontlines[type] = new Queue<Vector2Int>();
        frontlines[type].Enqueue(new Vector2Int(x, y));
    }

    IEnumerator UpdateWorld()
    {
        while (true)
        {
            foreach (var frontline in frontlines)
            {
                PixelType type = frontline.Key;
                Queue<Vector2Int> queue = frontline.Value;

                int currentQueueSize = queue.Count;

                for (int i = 0; i < currentQueueSize; i++)
                {
                    Vector2Int pos = queue.Dequeue();

                    if (world[pos.x, pos.y] == type)
                    {
                        Spread(pos.x, pos.y, ref queue);
                    }

                    queue.Enqueue(pos); // Добавляем позицию обратно в очередь
                }
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    IEnumerator RegeneratePower()
    {
        while (true)
        {
            yield return new WaitForSeconds(powerGainInterval);

            int blueTerritory = CountTerritory(PixelType.Blue);
            int redTerritory = CountTerritory(PixelType.Red);
            int greenTerritory = CountTerritory(PixelType.Green);
            int orangeTerritory = CountTerritory(PixelType.Orange);

            bluePower = Mathf.Min(maxPower, bluePower + blueTerritory);
            redPower = Mathf.Min(maxPower, redPower + redTerritory);
            greenPower = Mathf.Min(maxPower, greenPower + greenTerritory);
            orangePower = Mathf.Min(maxPower, orangePower + orangeTerritory);
        }
    }

    int CountTerritory(PixelType type)
    {
        int count = 0;
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                if (world[x, y] == type)
                {
                    count++;
                }
            }
        }
        return count;
    }

    int CalculateCaptureCost(PixelType type)
    {
        int territorySize = CountTerritory(type);

        if (territorySize < 100)
        {
            return 1;
        }
        else if (territorySize < 1000)
        {
            return 10;
        }
        else if (territorySize < 10000)
        {
            return 100;
        }
        else
        {
            return 1000;
        }
    }

    void Spread(int x, int y, ref Queue<Vector2Int> queue)
    {
        PixelType type = world[x, y];
        int currentPower = GetPower(type);
        Color color = GetColor(type);

        int captureCost = CalculateCaptureCost(type);

        if (currentPower < captureCost)
        {
            // Недостаточно силы для захвата
            return;
        }

        List<Vector2Int> neighborsToCheck = new List<Vector2Int>();

        // Захват соседних клеток
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                int newX = x + i;
                int newY = y + j;

                if (newX >= 0 && newX < worldSize && newY >= 0 && newY < worldSize)
                {
                    if (world[newX, newY] == PixelType.White)
                    {
                        neighborsToCheck.Add(new Vector2Int(newX, newY));
                    }
                }
            }
        }

        // Перемешиваем соседей для равномерного захвата
        Shuffle(neighborsToCheck);

        foreach (var pos in neighborsToCheck)
        {
            int newX = pos.x;
            int newY = pos.y;

            if (currentPower >= captureCost)
            {
                world[newX, newY] = type;
                worldObjects[newX, newY].GetComponent<SpriteRenderer>().color = color;
                DeductPower(type, captureCost);

                // Добавляем новую клетку в очередь для дальнейшего захвата
                queue.Enqueue(new Vector2Int(newX, newY));
            }
        }

        // Проверяем, не захвачена ли столица
        CheckCapitals();
    }

    void CheckCapitals()
    {
        foreach (var capital in capitals)
        {
            PixelType type = capital.Key;
            Vector2Int capitalPos = capital.Value;

            if (world[capitalPos.x, capitalPos.y] != type)
            {
                // Если столица захвачена другим цветом, уничтожаем старый тип
                Destroy(capitalObjects[capitalPos.x, capitalPos.y]);
                capitalObjects[capitalPos.x, capitalPos.y] = null;

                switch (type)
                {
                    case PixelType.Blue: bluePower = 0; break;
                    case PixelType.Red: redPower = 0; break;
                    case PixelType.Green: greenPower = 0; break;
                    case PixelType.Orange: orangePower = 0; break;
                }
            }
        }
    }

    void DeductPower(PixelType type, int amount)
    {
        switch (type)
        {
            case PixelType.Blue:
                bluePower = Mathf.Clamp(bluePower - amount, 0, maxPower);
                break;
            case PixelType.Red:
                redPower = Mathf.Clamp(redPower - amount, 0, maxPower);
                break;
            case PixelType.Green:
                greenPower = Mathf.Clamp(greenPower - amount, 0, maxPower);
                break;
            case PixelType.Orange:
                orangePower = Mathf.Clamp(orangePower - amount, 0, maxPower);
                break;
        }
    }

    int GetPower(PixelType type)
    {
        switch (type)
        {
            case PixelType.Blue: return bluePower;
            case PixelType.Red: return redPower;
            case PixelType.Green: return greenPower;
            case PixelType.Orange: return orangePower;
            default: return 0;
        }
    }

    Color GetColor(PixelType type)
    {
        switch (type)
        {
            case PixelType.Blue: return Color.blue;
            case PixelType.Red: return Color.red;
            case PixelType.Green: return Color.green;
            case PixelType.Orange: return new Color(1f, 0.5f, 0f);
            default: return Color.white;
        }
    }

    // Метод для перемешивания списка
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
