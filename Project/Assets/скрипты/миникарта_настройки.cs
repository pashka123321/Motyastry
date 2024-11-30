using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Toggle miniMapToggle;

    void Start()
    {
        // Загружаем сохранённое состояние чекбокса
        bool isMiniMapEnabled = PlayerPrefs.GetInt("MiniMapEnabled", 1) == 1;
        miniMapToggle.isOn = isMiniMapEnabled;

        // Подписываемся на событие изменения состояния чекбокса
        miniMapToggle.onValueChanged.AddListener(OnMiniMapToggleChanged);
    }

    void OnMiniMapToggleChanged(bool isOn)
    {
        // Сохраняем состояние чекбокса
        PlayerPrefs.SetInt("MiniMapEnabled", isOn ? 1 : 0);
        PlayerPrefs.Save();

        // Обновляем состояние мини-карты на всех сценах
        UpdateMiniMapState(isOn);
    }

    void UpdateMiniMapState(bool isEnabled)
    {
        // Обновляем состояние всех объектов мини-карты, если они находятся в текущей сцене
        MiniMapController miniMapController = FindObjectOfType<MiniMapController>();
        if (miniMapController != null)
        {
            foreach (GameObject part in miniMapController.miniMapParts)
            {
                part.SetActive(isEnabled);
            }
        }
    }
}
