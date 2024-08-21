using System.Collections;
using UnityEngine;

public class TeslaCoiltesla : MonoBehaviour
{
    public LayerMask targetLayer;  // Слой, на котором будут находиться цели
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
    private Transform currentTarget; // Текущая цель

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
            if (isActive && currentTarget != null)
            {
                StartCoroutine(GenerateBolt(currentTarget.position));
            }
            yield return new WaitForSeconds(strikeInterval);
        }
    }

    IEnumerator GenerateBolt(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position; // Начальная позиция молнии

        // Установка позиций линии молнии
        for (int i = 0; i < boltSegments; i++)
        {
            float t = i / (float)(boltSegments - 1);
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, t);
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
        UpdateTarget();  // Обновление текущей цели
        isActive = currentTarget != null;
        UpdateSprite();  // Обновление спрайта в зависимости от состояния
    }

    void UpdateTarget()
    {
        if (currentTarget != null)
        {
            // Если цель вышла за радиус активации, сбросить цель
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget > activationRadius)
            {
                currentTarget = null;
            }
        }

        if (currentTarget == null)
        {
            // Поиск ближайшей цели, если текущей цели нет
            currentTarget = FindClosestTarget();
        }
    }

    Transform FindClosestTarget()
    {
        Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, activationRadius, targetLayer);
        Collider2D closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (var target in targetsInRange)
        {
            // Проверка, что цель на том же слое и не является самим собой
            if (target.gameObject != gameObject)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }
        }

        return closestTarget != null ? closestTarget.transform : null;
    }

    void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isActive ? connectedSprite : disconnectedSprite;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Отображение радиуса активации в редакторе
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}
