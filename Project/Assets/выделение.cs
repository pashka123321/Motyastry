using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private Vector3 startMousePosition; // Начальная точка для выделения
    private Vector3 endMousePosition;   // Конечная точка для выделения
    private Vector3 currentMousePosition; // Текущая позиция мыши
    private bool isSelecting = false;  // Флаг выделения
    private bool mouseMoved = false;   // Флаг движения мыши
    private Texture2D selectionTexture; // Текстура рамки выделения
    private const float minMouseMovement = 5f; // Минимальное расстояние для начала выделения

    void Start()
    {
        // Создаём текстуру для выделения
        selectionTexture = new Texture2D(1, 1);
        selectionTexture.SetPixel(0, 0, new Color(0, 1, 0, 0.25f)); // Полупрозрачный зелёный
        selectionTexture.Apply();
    }

    void OnGUI()
    {
        // Рисуем рамку выделения, если выделяем
        if (isSelecting)
        {
            Rect rect = GetScreenRect(Camera.main.WorldToScreenPoint(startMousePosition), Camera.main.WorldToScreenPoint(currentMousePosition));
            DrawScreenRect(rect, selectionTexture);
            DrawScreenRectBorder(rect, 2, Color.green);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Начало выделения (ПКМ)
        {
            startMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseMoved = false; // Сбрасываем флаг движения мыши
        }

        if (Input.GetMouseButton(1)) // Удерживание ПКМ
        {
            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Проверяем, двигалась ли мышь достаточно
            if (!mouseMoved && Vector3.Distance(startMousePosition, currentMousePosition) > minMouseMovement)
            {
                mouseMoved = true; // Фиксируем движение
                isSelecting = true; // Начинаем выделение
            }
        }

        if (Input.GetMouseButtonUp(1)) // Завершение выделения
        {
            if (isSelecting)
            {
                endMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SelectBotsInRect(); // Выделяем ботов в зоне
                AddObjectsToDeleteQueue(); // Добавляем объекты в очередь удаления
            }

            isSelecting = false; // Сбрасываем флаг выделения
        }
    }

    // Выделение ботов в зоне
    private void SelectBotsInRect()
    {
        Vector2 min = Vector2.Min(startMousePosition, endMousePosition);
        Vector2 max = Vector2.Max(startMousePosition, endMousePosition);

        Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);

        foreach (var collider in colliders)
        {
            BotAI bot = collider.GetComponent<BotAI>();
            if (bot != null)
            {
                bot.Select(); // Выделяем бота
            }
        }
    }

    // Добавление объектов в очередь удаления
    private void AddObjectsToDeleteQueue()
    {
        Vector2 min = Vector2.Min(startMousePosition, endMousePosition);
        Vector2 max = Vector2.Max(startMousePosition, endMousePosition);

        Collider2D[] colliders = Physics2D.OverlapAreaAll(min, max);

        foreach (var collider in colliders)
        {
            DeleteOnRightClick.Instance.EnqueueObjectForDeletion(collider.gameObject);
        }
    }

    // Создание прямоугольника из двух точек
    private Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        Vector3 topLeft = Vector3.Min(screenPosition1, screenPosition2);
        Vector3 bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    // Рисование прямоугольника
    private void DrawScreenRect(Rect rect, Texture2D texture)
    {
        GUI.DrawTexture(rect, texture);
    }

    // Рисование рамки прямоугольника
    private void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        Color prevColor = GUI.color;
        GUI.color = color;

        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), Texture2D.whiteTexture); // Верх
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture); // Низ
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), Texture2D.whiteTexture); // Лево
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), Texture2D.whiteTexture); // Право

        GUI.color = prevColor;
    }
}
