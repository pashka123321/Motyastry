using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private Canvas inputCanvas;
    [SerializeField] private Button playButton;
    [SerializeField] private Canvas menuCanvas; // Добавляем ссылку на Canvas с меню
    [SerializeField] private Vector3 originalPosition;
    private bool isCanvasActive = false; // Флаг активности Canvas
    private bool menuWasActiveBefore = false; // Переменная для хранения состояния Canvas с меню

    void Start()
    {
        // Запоминаем оригинальное положение Canvas
        originalPosition = inputCanvas.transform.position;
        
        // Убедимся, что Canvas активен по умолчанию
        inputCanvas.gameObject.SetActive(false);
        
        // Привязываем метод к кнопке "играть"
        playButton.onClick.AddListener(OnPlayButtonClick);
    }

    void Update()
    {
        // Проверяем нажатие клавиши Esc для скрытия Canvas
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isCanvasActive)
            {
                HideCanvas();
            }
        }
    }

    void ShowCanvas()
    {
        // Скрываем Canvas с меню, если он активен
        if (menuCanvas.gameObject.activeSelf)
        {
            menuWasActiveBefore = true;
            menuCanvas.gameObject.SetActive(false);
        }
        
        // Показываем основной Canvas
        inputCanvas.gameObject.SetActive(true); // Активируем Canvas
        isCanvasActive = true; // Устанавливаем флаг
    }

    void HideCanvas()
    {
        inputCanvas.gameObject.SetActive(false); // Скрываем Canvas
        isCanvasActive = false; // Сбрасываем флаг
        
        // Показываем Canvas с меню, если он был активен до открытия основного Canvas
        if (menuWasActiveBefore)
        {
            menuCanvas.gameObject.SetActive(true);
            menuWasActiveBefore = false;
        }
    }

    void OnPlayButtonClick()
    {
        if (isCanvasActive)
        {
            HideCanvas();
        }
        else
        {
            ShowCanvas();
        }
    }
}
