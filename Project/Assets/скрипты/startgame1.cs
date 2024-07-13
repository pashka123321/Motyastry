using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartGame1 : MonoBehaviour
{
    public AudioClip[] buttonSounds; // Массив аудиоклипов для звука кнопки
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Использовать существующий AudioSource на этом же объекте
        if (audioSource == null)
        {
            Debug.LogError("AudioSource не найден на этом объекте.");
        }

        audioSource.volume = 1.0f; // Проверьте уровень громкости
        audioSource.spatialBlend = 0.0f; // Установите пространственную смесь на 2D
    }

    public void LoadGameScene()
    {
        PlayRandomSound(); // Воспроизвести случайный звук
        StartCoroutine(LoadSceneWithDelay(1.0f)); // Загрузить сцену с задержкой в 1 секунду
    }

    private void PlayRandomSound()
    {
        if (buttonSounds != null && buttonSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, buttonSounds.Length); // Выбрать случайный индекс
            AudioClip randomClip = buttonSounds[randomIndex]; // Получить случайный аудиоклип
            audioSource.PlayOneShot(randomClip); // Воспроизвести случайный аудиоклип
        }
        else
        {
            Debug.LogWarning("Массив buttonSounds пустой или отсутствуют аудиоклипы.");
        }
    }

    private IEnumerator LoadSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Ожидать заданное количество секунд
        SceneManager.LoadScene("menu"); // Загрузить сцену "menu"
    }
}
