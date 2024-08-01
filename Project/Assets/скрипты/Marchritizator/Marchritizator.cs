using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Marchritizator : MonoBehaviour
{
    public Transform[] spawnPoints;    // Массив точек спавна
    public bool[] activeSP;            // Массив активных точек спавна

    private int currentSpawnIndex = 0;
    private List<Collider2D> collidersInTrigger = new List<Collider2D>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MovementController>() != null)
        {
            if (!collidersInTrigger.Contains(collision))
            {
                collidersInTrigger.Add(collision);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<MovementController>() != null)
        {
            if (collidersInTrigger.Contains(collision))
            {
                collidersInTrigger.Remove(collision);
            }
        }
    }

    private void Update()
    {
        if (spawnPoints == null || spawnPoints.Length == 0 || activeSP == null || activeSP.Length == 0)
        {
            return; // Прерывание, если массивы не заданы
        }

        // Найти активные точки спавна
        var activeSpawnPoints = spawnPoints
            .Select((point, index) => new { point, index })
            .Where(x => activeSP[x.index])
            .ToList();

        if (activeSpawnPoints.Count == 0)
        {
            return; // Прерывание, если нет активных точек спавна
        }

        // Обработать ресурсы
        if (collidersInTrigger.Count > 0)
        {
            var collider = collidersInTrigger.First(); // Получаем первый коллайдер из списка

            // Проверяем, что текущий индекс находится в пределах допустимого диапазона
            if (currentSpawnIndex >= activeSpawnPoints.Count)
            {
                currentSpawnIndex = 0; // Сброс индекса если превышает количество активных точек спавна
            }

            // Создаем объект на текущей активной точке спавна
            Instantiate(collider.gameObject, activeSpawnPoints[currentSpawnIndex].point.position, Quaternion.identity);
            Destroy(collider.gameObject);

            collidersInTrigger.Remove(collider); // Удаляем обработанный коллайдер из списка

            // Переключаемся на следующую точку спавна
            currentSpawnIndex++;
            if (currentSpawnIndex >= activeSpawnPoints.Count)
            {
                currentSpawnIndex = 0; // Сброс индекса если превышает количество активных точек спавна
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
