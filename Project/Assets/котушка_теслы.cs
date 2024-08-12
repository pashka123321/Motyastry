using System.Collections;
using UnityEngine;

public class TeslaCoiltesla : MonoBehaviour
{
    public Transform target;  // Цель, куда будет ударять молния
    public Sprite connectedSprite;  // Спрайт для состояния "подключена"
    public Sprite disconnectedSprite;  // Спрайт для состояния "не подключена"
    public float boltDuration = 0.1f; // Длительность отображения молнии
    public float strikeInterval = 0.1f; // Интервал между ударами
    public int boltSegments = 10; // Количество сегментов молнии
    public float boltOffset = 0.5f; // Случайное смещение сегментов
    public float activationRadius = 10f; // Радиус активации молнии

    private LineRenderer lineRenderer;
    private SpriteRenderer spriteRenderer;
    private bool isActive;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer.positionCount = boltSegments;
        UpdateSprite();
        StartCoroutine(StrikeRoutine());
    }

    IEnumerator StrikeRoutine()
    {
        while (true)
        {
            if (isActive)
            {
                StartCoroutine(GenerateBolt());
            }
            yield return new WaitForSeconds(strikeInterval);
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

    void Update()
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            isActive = distanceToTarget <= activationRadius;
            UpdateSprite(); // Обновление спрайта на каждом кадре
        }
    }

    void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            if (isActive)
            {
                spriteRenderer.sprite = connectedSprite;
            }
            else
            {
                spriteRenderer.sprite = disconnectedSprite;
            }
        }
    }
}
