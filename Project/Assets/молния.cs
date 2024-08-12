using System.Collections;
using UnityEngine;

public class TeslaCoil : MonoBehaviour
{
    public Transform target;  // Цель, куда будет ударять молния
    public float boltDuration = 0.1f; // Длительность отображения молнии
    public float strikeInterval = 5f; // Интервал между ударами
    public int boltSegments = 10; // Количество сегментов молнии
    public float boltOffset = 0.5f; // Случайное смещение сегментов

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = boltSegments;
        StartCoroutine(StrikeRoutine());
    }

    IEnumerator StrikeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(strikeInterval);
            StartCoroutine(GenerateBolt());
        }
    }

    IEnumerator GenerateBolt()
    {
        Vector3 startPosition = transform.position; // Начальная позиция молнии
        Vector3 endPosition = target.position; // Конечная позиция молнии

        // Установка позиций линии молнии
        for (int i = 0; i < boltSegments; i++)
        {
            float t = i / (float)(boltSegments - 1);
            Vector3 position = Vector3.Lerp(startPosition, endPosition, t);
            position.x += Random.Range(-boltOffset, boltOffset);
            position.y += Random.Range(-boltOffset, boltOffset);
            lineRenderer.SetPosition(i, position);
        }

        // Отображение молнии
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(boltDuration);
        lineRenderer.enabled = false;
    }
}
