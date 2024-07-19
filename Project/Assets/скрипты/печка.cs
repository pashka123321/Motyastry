using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    [System.Serializable]
    public class OreToIngotMapping
    {
        public string oreTag;         // Тег руды
        public GameObject ingotPrefab; // Префаб слитка
    }

    public List<OreToIngotMapping> oreToIngotMappings; // Список сопоставлений руды и слитков
    public Transform[] spawnPoints;    // Точка спавна слитка
    public bool[] activeSP;

    private int i = 0;

    // Метод обработки входа объекта в триггер
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcessCollision(collision);
    }

    // Метод обработки постоянного нахождения объекта в триггере
    private void OnTriggerStay2D(Collider2D collision)
    {
        ProcessCollision(collision);
    }

    // Обработка столкновения руды с печью
    private void ProcessCollision(Collider2D collision)
    {
        if (spawnPoints == null || spawnPoints.Length == 0 || activeSP == null || activeSP.Length == 0)
        {
            return; // Предотвращаем выполнение если точки спавна или активные точки не заданы
        }

        int count = activeSP.Where(c => c).Count();

        if (count == 0)
        {
            return; // Если нет активных точек спавна, выходим
        }

        if (i >= spawnPoints.Length)
        {
            i = 0; // Сбрасываем индекс если он выходит за пределы массива точек спавна
        }

        // Получаем GameObject, с которым произошло столкновение
        GameObject oreObject = collision.gameObject;

        // Ищем в списке сопоставлений подходящий префаб слитка для данной руды
        foreach (var mapping in oreToIngotMappings)
        {
            // Проверяем по тегу руды
            if (oreObject.CompareTag(mapping.oreTag))
            {
                while (activeSP[i] == false)
                {
                    i++;
                    if (i >= spawnPoints.Length)
                    {
                        i = 0; // Сбрасываем индекс если он выходит за пределы массива точек спавна
                    }
                }

                // Спавним соответствующий слиток в указанной точке с нулевым поворотом
                Instantiate(mapping.ingotPrefab, spawnPoints[i].position, Quaternion.identity);
                i++;

                // Удаляем объект руды
                Destroy(oreObject);
                return; // Выходим из метода после успешного спавна слитка
            }
        }
    }

    public void ActivateSpawnPoint(int spIndex)
    {
        if (spIndex >= 0 && spIndex < activeSP.Length)
        {
            activeSP[spIndex] = true;
        }
    }

    public void DeactivateSpawnPoint(int spIndex)
    {
        if (spIndex >= 0 && spIndex < activeSP.Length)
        {
            activeSP[spIndex] = false;
        }
    }
}
