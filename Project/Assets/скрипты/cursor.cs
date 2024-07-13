using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D defaultCursor;    // Текстура курсора по умолчанию
    public Texture2D leftClickCursor;  // Текстура курсора при нажатии левой кнопки мыши
    public Texture2D rightClickCursor; // Текстура курсора при нажатии правой кнопки мыши

    private static CursorManager instance;

    private void Awake()
    {
        // Убедимся, что существует только один экземпляр Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Сохраняем объект при переходе между сценами
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши
        {
            Cursor.SetCursor(leftClickCursor, Vector2.zero, CursorMode.Auto);
        }
        else if (Input.GetMouseButtonDown(1)) // Правая кнопка мыши
        {
            Cursor.SetCursor(rightClickCursor, Vector2.zero, CursorMode.Auto);
        }
        else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) // Отпускание любой из кнопок мыши
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    // Метод для изменения курсора из других скриптов
    public static void SetCursor(Texture2D cursorTexture)
    {
        instance.defaultCursor = cursorTexture;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}
