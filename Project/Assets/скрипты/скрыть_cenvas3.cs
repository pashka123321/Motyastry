using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private Canvas inputCanvas;
    [SerializeField] private Button playButton;
    [SerializeField] private Button[] hideButtons; // Массив кнопок для скрытия Canvas
    [SerializeField] private Vector3 originalPosition;
    private bool isCanvasActive = false;

    void Start()
    {
        originalPosition = inputCanvas.transform.position;
        inputCanvas.gameObject.SetActive(false); // Убедимся, что Canvas скрыт по умолчанию

        // Привязываем метод к кнопке "играть"
        playButton.onClick.AddListener(OnPlayButtonClick);

        // Привязываем метод для скрытия к каждой кнопке из массива
        foreach (Button button in hideButtons)
        {
            button.onClick.AddListener(OnHideButtonClick); // Привязываем метод для скрытия к кнопке
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isCanvasActive)
        {
            HideCanvas();
        }
    }

    void ShowCanvas()
    {
        inputCanvas.gameObject.SetActive(true); // Показываем Canvas
        isCanvasActive = true;
    }

    void HideCanvas()
    {
        inputCanvas.gameObject.SetActive(false); // Скрываем Canvas
        isCanvasActive = false;
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

    // Метод для скрытия Canvas, вызываемый кнопками из массива
    void OnHideButtonClick()
    {
        if (isCanvasActive)
        {
            HideCanvas();
        }
    }
}
