using UnityEngine;
using UnityEngine.UI;  // Если используете обычный Text
// using TMPro;        // Если используете TextMeshPro

public class TimerScript : MonoBehaviour
{
    public float maxTime = 120f;  // Максимальное время в секундах
    private float currentTime;    // Текущее время
    private bool countingDown;    // Флаг для отсчета времени

    public Text timerText;  // Ссылка на UI текстовый элемент

    void Start()
    {
        currentTime = maxTime;
        countingDown = true;
    }

    void Update()
    {
        if (countingDown)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = maxTime;
            }

            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        // Форматируем время в минуты и секунды
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        // Обновляем текст UI элемента
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
