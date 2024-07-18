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
    public Transform[] spawnPoints; // Массив точек спавна
    public float spawnInterval = 1f; // Интервал спавна блока

    private Dictionary<string, GameObject[]> orePrefabsDict; // Словарь для хранения префабов руд
    private float timer; // Таймер для отслеживания интервалов
    private string currentOreTag; // Тег текущей руды
    private List<Transform> activeSpawnPoints; // Список активных точек спавна

    void Start()
    {
        timer = 0f; // Начальное значение таймера
        InitializeOrePrefabsDictionary();
        currentOreTag = null; // Начальное значение текущего тега руды
        activeSpawnPoints = new List<Transform>(); // Инициализация списка активных точек спавна
    }

    void Update()
    {
        timer += Time.deltaTime; // Увеличиваем таймер каждый кадр

        if (timer >= spawnInterval && currentOreTag != null && activeSpawnPoints.Count > 0)
        {
            SpawnBlock(currentOreTag); // Вызываем метод спавна блока
            timer = 0f; // Сбрасываем таймер
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

        if (other.CompareTag("Conveer"))
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (other.bounds.Intersects(spawnPoints[i].GetComponent<Collider2D>().bounds) && !activeSpawnPoints.Contains(spawnPoints[i]))
                {
                    activeSpawnPoints.Add(spawnPoints[i]); // Добавляем точку спавна в список активных
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        string blockTag = other.tag;

        if (blockTag == currentOreTag)
        {
            currentOreTag = null; // Сбрасываем текущий тег руды
        }

        if (other.CompareTag("Conveer"))
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (other.bounds.Intersects(spawnPoints[i].GetComponent<Collider2D>().bounds))
                {
                    activeSpawnPoints.Remove(spawnPoints[i]); // Удаляем точку спавна из списка активных
                }
            }
        }
    }

    void SpawnBlock(string oreTag)
    {
        if (orePrefabsDict.ContainsKey(oreTag))
        {
            GameObject[] prefabs = orePrefabsDict[oreTag];
            if (prefabs.Length > 0 && activeSpawnPoints.Count > 0)
            {
                // Спавним случайный префаб из списка в случайной активной точке спавна
                GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];
                Transform randomSpawnPoint = activeSpawnPoints[Random.Range(0, activeSpawnPoints.Count)];
                Instantiate(randomPrefab, randomSpawnPoint.position, Quaternion.identity);
            }
        }
    }
}
