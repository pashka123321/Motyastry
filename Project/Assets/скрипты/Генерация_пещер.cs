using UnityEngine;
using System.Collections.Generic;
public class LakeGenerator : MonoBehaviour
{
    [System.Serializable]
    public class LandscapePrefab
    {
        public GameObject prefab;
        public float perlinScaleX = 20f;
        public float perlinScaleY = 20f;
        public float threshold = 0.5f;
        public bool isBorder;
        public float percentage; // Новый параметр для процентного соотношения
    }

    [System.Serializable]
    public class LakePrefab
    {
        public GameObject prefab;
        public string type;
        public GameObject sandPrefab; // Префаб песка для обводки озера
        public int sandBorderThickness = 1; // Толщина обводки песком в блоках
        public float centerLakePercentage = 0.5f; // Процент занимаемой центром озера области
        public GameObject centerLakePrefab; // Префаб для центральной области озера
    }

    public LandscapePrefab[] landscapePrefabs;
    public LakePrefab[] lakePrefabs;
    public int mapWidth = 200;
    public int mapHeight = 200;
    public Vector2 perlinOffset;
    public float biomeScale = 50f;
    public int exclusionRadius = 25;
    public int transitionRadius = 10;
    public float noiseAdjustment = 0.1f; // Parameter for noise adjustment
    public int minLakes = 2;
    public int maxLakes = 4;
    public int lakeRadius = 5;

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

        // Initialize Perlin noise offset using seed for stability
        perlinOffset = new Vector2(seed % 1000, seed % 1000);
        GenerateCaveLandscapes();
        GenerateLakes();
    }

    void GenerateCaveLandscapes()
    {
        // Clear old objects
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;

        // Расчет общих процентных соотношений
        float totalPercentage = 0;
        foreach (var prefab in landscapePrefabs)
        {
            totalPercentage += prefab.percentage;
        }

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
                float cumulativePercentage = 0;
                LandscapePrefab selectedPrefab = null;

                // Выбор префаба на основе процентного соотношения
                foreach (var prefab in landscapePrefabs)
                {
                    cumulativePercentage += prefab.percentage / totalPercentage;
                    if (biomeValue <= cumulativePercentage)
                    {
                        selectedPrefab = prefab;
                        break;
                    }
                }

                if (selectedPrefab != null)
                {
                    float perlinValue = Mathf.PerlinNoise((x + perlinOffset.x) / selectedPrefab.perlinScaleX, (y + perlinOffset.y) / selectedPrefab.perlinScaleY);

                    // Adjust noise value to create more cave-like structures
                    perlinValue += Random.Range(-noiseAdjustment, noiseAdjustment);

                    if (distanceFromCenter <= exclusionRadius + transitionRadius)
                    {
                        perlinValue *= Mathf.InverseLerp(exclusionRadius, exclusionRadius + transitionRadius, distanceFromCenter);
                    }

                    if (perlinValue > selectedPrefab.threshold)
                    {
                        Vector3 position = new Vector3(x, y, 0);
                        Instantiate(selectedPrefab.prefab, position, Quaternion.identity, transform);
                        AddBorderBlocks(x, y);
                    }
                }
            }
        }
    }

    void GenerateLakes()
    {
        // Ensure each lake type spawns at least once
        for (int lakeTypeIndex = 0; lakeTypeIndex < lakePrefabs.Length; lakeTypeIndex++)
        {
            var lakePrefab = lakePrefabs[lakeTypeIndex];

            Vector2 lakeCenter = new Vector2(Random.Range(lakeRadius, mapWidth - lakeRadius), Random.Range(lakeRadius, mapHeight - lakeRadius));

            // Check if the lake center is in a valid position
            if (IsPositionOccupied(lakeCenter))
            {
                continue;
            }

            GenerateLakeShape(lakePrefab, lakeCenter);
        }

        // Generate additional lakes randomly
        int numLakes = Random.Range(minLakes - lakePrefabs.Length, maxLakes - lakePrefabs.Length + 1);

        for (int i = 0; i < numLakes; i++)
        {
            int lakeTypeIndex = Random.Range(0, lakePrefabs.Length);
            var lakePrefab = lakePrefabs[lakeTypeIndex];

            Vector2 lakeCenter = new Vector2(Random.Range(lakeRadius, mapWidth - lakeRadius), Random.Range(lakeRadius, mapHeight - lakeRadius));

            // Check if the lake center is in a valid position
            if (IsPositionOccupied(lakeCenter))
            {
                i--;
                continue;
            }

            GenerateLakeShape(lakePrefab, lakeCenter);
        }
    }

    void GenerateLakeShape(LakePrefab lakePrefab, Vector2 lakeCenter)
    {
        // Determine lake shape (could be elongated, curved, etc.)
        float shapeFactor = Random.Range(0.5f, 1.5f); // Example: elongated lakes
        HashSet<Vector2> lakePositions = new HashSet<Vector2>();

        for (int x = -lakeRadius; x <= lakeRadius; x++)
        {
            for (int y = -lakeRadius; y <= lakeRadius; y++)
            {
                if (x * x + y * y <= lakeRadius * lakeRadius * shapeFactor)
                {
                    Vector2 lakePosition = new Vector2(lakeCenter.x + x, lakeCenter.y + y);

                    // Check if the lake position is in a valid position
                    if (IsPositionOccupied(lakePosition))
                    {
                        continue;
                    }

                    lakePositions.Add(lakePosition);

                    Vector3 position = new Vector3(lakePosition.x, lakePosition.y, 0);

                    // Determine which part of lake to instantiate
                    if (Vector2.Distance(lakePosition, lakeCenter) <= lakeRadius * lakePrefab.centerLakePercentage)
                    {
                        // Instantiate center lake prefab
                        Instantiate(lakePrefab.centerLakePrefab, position, Quaternion.identity, transform);
                    }
                    else
                    {
                        // Instantiate outer lake prefab
                        Instantiate(lakePrefab.prefab, position, Quaternion.identity, transform);
                    }
                }
            }
        }

        // Add sand border around the lake
        foreach (var lakePosition in lakePositions)
        {
            AddSandBorder(lakePrefab.sandPrefab, lakePosition, lakePositions, lakePrefab.sandBorderThickness);
        }
    }

    bool IsPositionOccupied(Vector2 position)
    {
        Collider2D collider = Physics2D.OverlapPoint(position);
        return collider != null;
    }

    void AddSandBorder(GameObject sandPrefab, Vector2 lakePosition, HashSet<Vector2> lakePositions, int sandBorderThickness)
    {
        Vector2[] directions = {
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(-1, 0),
            new Vector2(1, 1),
            new Vector2(-1, 1),
            new Vector2(1, -1),
            new Vector2(-1, -1)
        };

        HashSet<Vector2> borderPositions = new HashSet<Vector2>();
        borderPositions.Add(lakePosition);

        for (int i = 0; i < sandBorderThickness; i++)
        {
            HashSet<Vector2> newBorderPositions = new HashSet<Vector2>();
            foreach (var position in borderPositions)
            {
                foreach (var direction in directions)
                {
                    Vector2 borderPosition = position + direction;
                    if (!lakePositions.Contains(borderPosition) && !borderPositions.Contains(borderPosition) && borderPosition.x >= 0 && borderPosition.x < mapWidth && borderPosition.y >= 0 && borderPosition.y < mapHeight)
                    {
                        Collider2D collider = Physics2D.OverlapPoint(borderPosition);
                        if (collider == null)
                        {
                            Instantiate(sandPrefab, new Vector3(borderPosition.x, borderPosition.y, 0), Quaternion.identity, transform);
                            newBorderPositions.Add(borderPosition);
                        }
                    }
                }
            }
            borderPositions.UnionWith(newBorderPositions);
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
