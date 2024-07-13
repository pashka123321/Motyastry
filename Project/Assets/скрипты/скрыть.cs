using UnityEngine;

public class HideObjectOnStart : MonoBehaviour
{
    // Префаб кнопки настройки
    public GameObject settingsButtonPrefab;

    // Префаб камеры
    public Camera targetCamera;

    // Ссылка на канвас
    public Canvas settingsCanvas;

    // Ссылка на кнопку настройки
    private GameObject settingsButton;

    void Start()
    {
        // Скрыть канвас при запуске сцены
        settingsCanvas.gameObject.SetActive(false);

        // Найти кнопку настройки
        settingsButton = Instantiate(settingsButtonPrefab, transform);

        // Добавить слушатель нажатия на кнопку настройки
        settingsButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ShowSettingsCanvas);
    }

    // Восстановить канвас (показать его снова и привязать к камере)
    public void ShowSettingsCanvas()
    {
        // Активировать канвас
        settingsCanvas.gameObject.SetActive(true);

        // Привязать канвас к камере
        settingsCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        settingsCanvas.worldCamera = targetCamera;

        // Установить канвас выше остальных
        settingsCanvas.sortingOrder = 1004;
    }
}
