using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BotAI : MonoBehaviour
{
    public float speed = 5f; // Скорость движения бота
    public Sprite selectionSprite; // Спрайт для индикатора выделения
    public LayerMask obstacleLayers; // Слои препятствий, которые нужно обходить

    private static List<BotAI> selectedBots = new List<BotAI>(); // Список всех выбранных ботов
    private List<Vector2> waypoints = new List<Vector2>(); // Список контрольных точек
    private bool isMoving = false;
    public bool isSelected = false; // Проверка, выбран ли бот
    private LineRenderer lineRenderer;
    private GameObject selectionIndicator;

    private bool isRightMouseHeld = false; // Флаг, отслеживающий зажатие ПКМ
    private float rightMouseHoldTime = 0f; // Время удержания ПКМ
    private const float holdThreshold = 0.2f; // Порог времени, чтобы считать зажатие

void Start()
{
    lineRenderer = gameObject.AddComponent<LineRenderer>();
    lineRenderer.startWidth = 0.1f; // Более тонкая линия
    lineRenderer.endWidth = 0.1f; // Более тонкая линия
    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

    // Полупрозрачный красный цвет
    Color lineColor = new Color(1, 0, 0, 0.3f);
    lineRenderer.startColor = lineColor;
    lineRenderer.endColor = lineColor;

    lineRenderer.positionCount = 0;

    selectionIndicator = new GameObject("SelectionIndicator");
    selectionIndicator.transform.SetParent(transform);
    selectionIndicator.transform.localPosition = Vector3.zero;
    selectionIndicator.transform.localScale = Vector3.one * 4f;

    SpriteRenderer spriteRenderer = selectionIndicator.AddComponent<SpriteRenderer>();
    spriteRenderer.sprite = selectionSprite;
    spriteRenderer.color = new Color(0, 1, 0, 0.5f);
    selectionIndicator.SetActive(false);

    // Запускаем проверку каждую секунду
    StartCoroutine(CheckForObstaclesRoutine());
}

void Update()
{
    HandleSelection();
    HandleMovement();
    AvoidObstacles(); // Проверка препятствий и отход от них
}


    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D collider = Physics2D.OverlapPoint(mousePosition);

            if (collider != null)
            {
                BotAI bot = collider.GetComponent<BotAI>();

                if (bot != null && bot != this)
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        if (bot.isSelected)
                        {
                            bot.Deselect();
                        }
                        else
                        {
                            bot.Select();
                        }
                    }
                    else
                    {
                        DeselectAll();
                        bot.Select();
                    }
                }
            }
            else if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                DeselectAll();
            }
        }
    }

    private void HandleMovement()
    {
        if (isSelected)
        {
            if (Input.GetMouseButtonDown(1))
            {
                isRightMouseHeld = true;
                rightMouseHoldTime = 0f;
            }

            if (Input.GetMouseButton(1))
            {
                rightMouseHoldTime += Time.deltaTime;
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (isRightMouseHeld && rightMouseHoldTime < holdThreshold)
                {
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 destination = new Vector2(mousePosition.x, mousePosition.y);

                    waypoints = CalculatePath(transform.position, destination);
                    UpdateLineRenderer();

                    isMoving = true;
                }

                isRightMouseHeld = false;
            }
        }

        if (isMoving && waypoints.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[0], speed * Time.deltaTime);
            UpdateLineRenderer();

            if (Vector2.Distance(transform.position, waypoints[0]) < 0.1f)
            {
                waypoints.RemoveAt(0);
                if (waypoints.Count == 0)
                {
                    isMoving = false;
                    lineRenderer.positionCount = 0;
                }
            }
        }
    }

private IEnumerator CheckForObstaclesRoutine()
{
    while (true)
    {
        AvoidObstacles();
        yield return new WaitForSeconds(1f); // Интервал проверки в секундах
    }
}


private void AvoidObstacles()
{
    float safeDistance = 2.0f; // Минимальное безопасное расстояние от препятствия
    float moveAwayStep = 0.5f; // Шаг для отхода от препятствия
    int maxChecks = 36; // Количество направлений, которые проверяет бот (10° шаг)
    float checkStepAngle = 360f / maxChecks;

    Collider2D hit = Physics2D.OverlapCircle(transform.position, safeDistance, obstacleLayers);

    if (hit != null)
    {
        // Бот слишком близко к препятствию, ищем свободное направление
        for (int i = 0; i < maxChecks; i++)
        {
            float angle = i * checkStepAngle;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector2 potentialPosition = (Vector2)transform.position + direction * moveAwayStep;

            // Проверяем, свободно ли место для отхода
            if (!Physics2D.OverlapCircle(potentialPosition, 0.5f, obstacleLayers))
            {
                // Если нашли свободное место, двигаемся туда
                transform.position = Vector2.MoveTowards(transform.position, potentialPosition, speed * Time.deltaTime);
                return; // Прекращаем поиск после перемещения
            }
        }

        Debug.LogWarning("BotAI: No safe position found for avoidance.");
    }
}


private List<Vector2> CalculatePath(Vector2 start, Vector2 end)
{
    List<Vector2> path = new List<Vector2> { start };
    Vector2 currentPosition = start;
    int maxIterations = 100; // Лимит итераций для предотвращения зависания
    int iterations = 0;

    float stopDistance = 1.8f; // Увеличено расстояние остановки
    float avoidanceRadius = 1.5f; // Увеличенный радиус проверки препятствий
    float stepDistance = 1.5f; // Увеличенное расстояние шага

    while (Vector2.Distance(currentPosition, end) > stopDistance && iterations < maxIterations)
    {
        iterations++;
        Vector2 direction = (end - currentPosition).normalized;
        Vector2 nextPosition = currentPosition + direction * stepDistance;

        Collider2D hit = Physics2D.OverlapCircle(nextPosition, avoidanceRadius, obstacleLayers);
        if (hit != null && !hit.transform.IsChildOf(transform))
        {
            Vector2 offset = Vector2.Perpendicular(direction) * avoidanceRadius;
            if (!Physics2D.OverlapCircle(currentPosition + offset, avoidanceRadius, obstacleLayers))
            {
                nextPosition = currentPosition + offset;
            }
            else if (!Physics2D.OverlapCircle(currentPosition - offset, avoidanceRadius, obstacleLayers))
            {
                nextPosition = currentPosition - offset;
            }
            else
            {
                break; // Если невозможно обойти, выходим
            }
        }

        path.Add(nextPosition);
        currentPosition = nextPosition;
    }

    if (iterations >= maxIterations)
    {
        Debug.LogWarning("Pathfinding reached iteration limit, path might be incomplete.");
    }

    return path;
}


    public void Select()
    {
        if (!isSelected)
        {
            isSelected = true;
            selectedBots.Add(this);
            selectionIndicator.SetActive(true);
        }
    }

    public void Deselect()
    {
        if (isSelected)
        {
            isSelected = false;
            selectedBots.Remove(this);
            selectionIndicator.SetActive(false);
        }
    }

    private static void DeselectAll()
    {
        foreach (BotAI bot in selectedBots)
        {
            bot.isSelected = false;
            bot.selectionIndicator.SetActive(false);
        }
        selectedBots.Clear();
    }

    private void UpdateLineRenderer()
    {
        if (waypoints.Count > 0)
        {
            lineRenderer.positionCount = waypoints.Count;
            for (int i = 0; i < waypoints.Count; i++)
            {
                lineRenderer.SetPosition(i, waypoints[i]);
            }
        }
    }
}