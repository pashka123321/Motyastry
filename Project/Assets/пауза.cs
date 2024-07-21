using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    public Text pauseText;
    public static bool IsGamePaused = false;
    private bool isPaused = false;
    public GameObject[] interactableObjects; // Все объекты, с которыми можно взаимодействовать
    public AudioSource backgroundMusic; // Источник музыки

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

        foreach (GameObject obj in interactableObjects)
        {
            obj.SetActive(false); // Отключить все взаимодействуемые объекты
        }

        if (backgroundMusic != null)
        {
            backgroundMusic.Pause(); // Остановить музыку
        }
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // Возобновить время
        pauseText.enabled = false; // Скрыть текст
        isPaused = false;
        IsGamePaused = false;

        foreach (GameObject obj in interactableObjects)
        {
            obj.SetActive(true); // Включить все взаимодействуемые объекты
        }

        if (backgroundMusic != null)
        {
            backgroundMusic.UnPause(); // Возобновить музыку
        }
    }
}
