using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject[] panels; // Массив панелей
    public Button[] buttons; // Массив кнопок

    private int initialPanelIndex = 0; // Индекс начальной панели

    void Start()
    {
        // Назначаем функции для кнопок
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Локальная копия переменной для замыкания
            buttons[i].onClick.AddListener(() => ShowPanel(index));
        }

        // Показываем начальную панель
        ShowPanel(initialPanelIndex);
    }

    // Функция для показа панели
    void ShowPanel(int index)
    {
        // Скрываем все панели
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }

        // Показываем выбранную панель
        if (index >= 0 && index < panels.Length)
        {
            panels[index].SetActive(true);
        }
    }
}