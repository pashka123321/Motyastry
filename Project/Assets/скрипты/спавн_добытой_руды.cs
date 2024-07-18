using UnityEngine;
using System.Collections.Generic;

public class BlockSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct OrePrefabs
    {
        public string oreTag;
        public GameObject[] prefabs;
    }

    public OrePrefabs[] orePrefabsArray;
    public Transform[] spawnPoints;  // Точки спавна блоков 
    public bool[] activeSP; 

    public float spawnInterval = 1f;  // Интервал спавна блока

    private Dictionary<string, GameObject[]> orePrefabsDict; // Словарь для хранения префабов руд
    private float timer;  // Таймер для отслеживания интервалов
    private string currentOreTag; // Тег текущей руды

    private int i; // Текущая точка спавна

    [SerializeField] private DrillSpawnPointChecker[] drillSpawnPointCheckers;

    void Start()
    {
        timer = 0f;  // Начальное значение таймера
        InitializeOrePrefabsDictionary();
        currentOreTag = null; // Начальное значение текущего тега руды

        i = -1;
    }

    public void ActivateSpawnPoint(int spIndex)
    {
        if (activeSP[spIndex] == false)
        {
            activeSP[spIndex] = true;
        }
    }

    public void DeactivateSpawnPoint(int spIndex)
    {
        if (activeSP[spIndex] == true)
        {
            activeSP[spIndex] = false;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;  // Увеличиваем таймер каждый кадр

        if (i == 3)
        {
            i = -1;
        }

        if (timer >= spawnInterval && currentOreTag != null && spawnPoints.Length != 0)
        {
            i++;

            bool res = SpawnBlock(currentOreTag, i);  // Вызываем метод спавна блока

            if (res)
            {
                timer = 0f;  // Сбрасываем таймер
            }
            else
            {
                timer = spawnInterval;
            }
        }
    }

    void InitializeOrePrefabsDictionary()
    {
        orePrefabsDict = new Dictionary<string, GameObject[]>();

        foreach (var orePrefabs in orePrefabsArray)
        {
            orePrefabsDict.Add(orePrefabs.oreTag, orePrefabs.prefabs);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        string blockTag = other.tag;

        if (orePrefabsDict.ContainsKey(blockTag))
        {
            currentOreTag = blockTag; // Устанавливаем текущий тег руды
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        string blockTag = other.tag;

        if (blockTag == currentOreTag)
        {
            currentOreTag = null; // Сбрасываем текущий тег руды
        }
    }

    bool SpawnBlock(string oreTag, int i)
    {
        if (drillSpawnPointCheckers[i].ConatinsOre == true)
        {
            return false;
        }

        bool spawnRes = false;

        if (orePrefabsDict.ContainsKey(oreTag))
        {
            GameObject[] prefabs = orePrefabsDict[oreTag];
            if (prefabs.Length > 0)
            {
                // Спавним случайный префаб из списка в случайной активной точке спавна
                GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];

                if (activeSP[i] == true)
                {
                    Instantiate(randomPrefab, spawnPoints[i].position, Quaternion.identity);
                    spawnRes = true;
                }
            }
        }

        return spawnRes;
    }
}
