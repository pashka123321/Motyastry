using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    public Text pauseText;
    public static bool IsGamePaused = false;
    private bool isPaused = false;

    void Start()
    {
        pauseText.enabled = false; // Убедитесь, что текст выключен при старте
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // Остановить время
        pauseText.enabled = true; // Показать текст
        isPaused = true;
        IsGamePaused = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // Возобновить время
        pauseText.enabled = false; // Скрыть текст
        isPaused = false;
        IsGamePaused = false;
    }
}
