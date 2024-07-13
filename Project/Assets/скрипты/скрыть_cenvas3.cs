using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private Canvas inputCanvas;
    [SerializeField] private Button playButton;
    [SerializeField] private Canvas menuCanvas; // Добавляем ссылку на Canvas с меню
    [SerializeField] private Vector3 originalPosition;
    private bool isAttachedToCamera = false; // Изначально не привязан к камере
    private bool menuWasActiveBefore = false; // Переменная для хранения состояния Canvas с меню

    void Start()
    {
        // Запоминаем оригинальное положение Canvas
        originalPosition = inputCanvas.transform.position;
        
        // Скрываем Canvas при старте
        inputCanvas.gameObject.SetActive(false);
        
        // Привязываем метод к кнопке "играть"
        playButton.onClick.AddListener(OnPlayButtonClick);
    }

    void Update()
    {
        // Проверяем нажатие клавиши Esc для скрытия Canvas
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isAttachedToCamera)
            {
                DetachCanvasFromCamera();
            }
        }
    }

    void AttachCanvasToCamera()
    {
        // Скрываем Canvas с меню, если он активен
        if (menuCanvas.gameObject.activeSelf)
        {
            menuWasActiveBefore = true;
            menuCanvas.gameObject.SetActive(false);
        }
        
        // Показываем основной Canvas
        inputCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        inputCanvas.worldCamera = Camera.main;
        inputCanvas.planeDistance = 1f;
        inputCanvas.gameObject.SetActive(true); // Активируем Canvas
        isAttachedToCamera = true; // Устанавливаем флаг
    }

    void DetachCanvasFromCamera()
    {
        inputCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        inputCanvas.transform.position = originalPosition;
        inputCanvas.gameObject.SetActive(false); // Скрываем Canvas
        isAttachedToCamera = false; // Сбрасываем флаг
        
        // Показываем Canvas с меню, если он был активен до открытия основного Canvas
        if (menuWasActiveBefore)
        {
            menuCanvas.gameObject.SetActive(true);
            menuWasActiveBefore = false;
        }
    }

    void OnPlayButtonClick()
    {
        if (isAttachedToCamera)
        {
            DetachCanvasFromCamera();
        }
        else
        {
            AttachCanvasToCamera();
        }
    }
}
