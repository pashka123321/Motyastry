using UnityEngine;
using System.Collections;
using Pathfinding;

public class AutoScanScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(StartScanAfterDelay(1.6f));
    }

    IEnumerator StartScanAfterDelay(float delaySeconds)
    {
        // Ждем указанное количество секунд
        yield return new WaitForSeconds(delaySeconds);

        // Получаем экземпляр AstarPath
        AstarPath astar = AstarPath.active;

        // Выполняем проверку на наличие AstarPath
        if (astar != null)
        {
            // Запускаем сканирование
            astar.Scan();
        }
        else
        {
            Debug.LogError("AstarPath не найден в сцене. Убедитесь, что плагин A* Pathfinding Project правильно настроен.");
        }
    }
}
