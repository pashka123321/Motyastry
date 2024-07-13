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
    public Transform spawnPoint;          // Точка спавна блока
    public float spawnInterval = 1f;      // Интервал спавна блока

    private Dictionary<string, GameObject[]> orePrefabsDict; // Словарь для хранения префабов руд
    private float timer;  // Таймер для отслеживания интервалов
    private string currentOreTag; // Тег текущей руды

    void Start()
    {
        timer = 0f;  // Начальное значение таймера
        InitializeOrePrefabsDictionary();
        currentOreTag = null; // Начальное значение текущего тега руды
    }

    void Update()
    {
        timer += Time.deltaTime;  // Увеличиваем таймер каждый кадр

        if (timer >= spawnInterval && currentOreTag != null)
        {
            SpawnBlock(currentOreTag);  // Вызываем метод спавна блока
            timer = 0f;    // Сбрасываем таймер
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

    void SpawnBlock(string oreTag)
    {
        if (orePrefabsDict.ContainsKey(oreTag))
        {
            GameObject[] prefabs = orePrefabsDict[oreTag];
            if (prefabs.Length > 0)
            {
                // Спавним случайный префаб из списка
                GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];
                Instantiate(randomPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }
}
