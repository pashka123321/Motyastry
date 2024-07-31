using UnityEngine;

public class LightSpawner : MonoBehaviour
{
    public GameObject lightPrefab; // Префаб света, который нужно спавнить
    public float interval = 10f; // Интервал между спавнами
    public float delay = 0.1f; // Задержка перед началом спавна
    public LayerMask blockLayer; // Слой, где объекты блокируют спавн

    void Start()
    {
        // Запуск корутины, которая выполнит спавн с задержкой
        StartCoroutine(SpawnLightsAfterDelay());
    }

    System.Collections.IEnumerator SpawnLightsAfterDelay()
    {
        // Ожидание указанного времени
        yield return new WaitForSeconds(delay);

        SpawnLights();
    }

    void SpawnLights()
    {
        for (float x = 0; x <= 200; x += interval)
        {
            for (float y = 0; y <= 200; y += interval)
            {
                Vector3 position = new Vector3(x, y, 0);

                // Проверка, не пересекается ли позиция с объектами на слое block
                if (!Physics2D.OverlapPoint(position, blockLayer))
                {
                    // Создаем экземпляр префаба света в позиции (x, y)
                    Instantiate(lightPrefab, position, Quaternion.identity);
                }
            }
        }
    }
}
