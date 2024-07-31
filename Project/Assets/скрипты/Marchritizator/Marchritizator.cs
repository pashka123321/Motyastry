using System.Linq;
using UnityEngine;

public class Marchritizator : MonoBehaviour
{
    public Transform[] spawnPoints;    // “очки спавна предметов
    public bool[] activeSP;

    private int i = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MovementController>() == null) return;

        if (spawnPoints == null || spawnPoints.Length == 0 || activeSP == null || activeSP.Length == 0)
        {
            return; // ѕредотвращаем выполнение если точки спавна или активные точки не заданы
        }

        int count = activeSP.Where(c => c).Count();

        if (count == 0)
        {
            return; // ≈сли нет активных точек спавна, выходим
        }

        if (i >= spawnPoints.Length)
        {
            i = 0; // —брасываем индекс если он выходит за пределы массива точек спавна
        }

        while (activeSP[i] == false)
        {
            i++;
            if (i >= spawnPoints.Length)
            {
                i = 0; // —брасываем индекс если он выходит за пределы массива точек спавна
            }
        }

        Instantiate(collision.gameObject, spawnPoints[i].position, Quaternion.identity);

        Destroy(collision.gameObject);
        i++;
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
