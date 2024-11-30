using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsCanvasPrefab; // Префаб Canvas с настройками
    private GameObject settingsCanvasInstance; // Ссылка на инстанс Canvas в сцене

    // Метод для открытия Canvas с настройками
    public void ToggleSettings()
    {
        if (settingsCanvasInstance == null)
        {
            // Создаем экземпляр Canvas с настройками из префаба
            settingsCanvasInstance = Instantiate(settingsCanvasPrefab);
            
            // Устанавливаем Canvas в режим Screen Space - Overlay
            Canvas canvas = settingsCanvasInstance.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            
            settingsCanvasInstance.SetActive(true);
        }
        else
        {
            // Переключаем состояние Canvas
            settingsCanvasInstance.SetActive(!settingsCanvasInstance.activeSelf);
        }
    }
}
