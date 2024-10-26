using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject[] panels; // Массив панелей
    public Button[] buttons; // Массив кнопок
    public GameObject highlightPrefab; // Префаб выделяющего объекта
    private GameObject currentHighlight; // Текущий выделяющий объект

    private int initialPanelIndex = 0; // Индекс начальной панели

    void Start()
    {
        // Создаем объект выделения
        currentHighlight = Instantiate(highlightPrefab, transform);

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

        // Перемещаем выделяющий объект на кнопку
        TeleportHighlightToButton(buttons[index]);
    }

    // Функция для перемещения объекта выделения на кнопку
    private void TeleportHighlightToButton(Button button)
    {
        if (button != null)
        {
            RectTransform buttonTransform = button.GetComponent<RectTransform>();
            currentHighlight.transform.position = buttonTransform.position; // Устанавливаем позицию выделяющего объекта
        }
    }
}
