using UnityEngine;

public class HideObjectOnStart : MonoBehaviour
{
    // Префаб камеры
    public Camera targetCamera;

    // Ссылка на канвас
    public Canvas settingsCanvas;

    void Start()
    {
        // Скрыть канвас при запуске сцены
        settingsCanvas.gameObject.SetActive(false);

        // Найти кнопку настройки по имени
        GameObject settingsButton = GameObject.Find("SettingsButton");

        // Проверить, что кнопка настройки найдена
        if (settingsButton != null)
        {
            // Добавить слушатель нажатия на кнопку настройки
            settingsButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ShowSettingsCanvas);
        }
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
        settingsCanvas.sortingOrder = 2111;
    }
}
