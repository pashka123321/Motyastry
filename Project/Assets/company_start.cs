using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class companystart : MonoBehaviour
{
    public AudioClip[] buttonClickSounds; // Массив аудиоклипов для звука кнопки
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Добавление компонента AudioSource к объекту
    }

    public void LoadGameScene()
    {
        PlayRandomSound(); // Воспроизвести случайный звук
        StartCoroutine(LoadSceneWithDelay(1.0f)); // Загрузить сцену с задержкой в 1 секунду
    }

    private void PlayRandomSound()
    {
        if (buttonClickSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, buttonClickSounds.Length); // Выбрать случайный индекс
            AudioClip randomClip = buttonClickSounds[randomIndex]; // Получить случайный аудиоклип
            audioSource.PlayOneShot(randomClip); // Воспроизвести случайный аудиоклип
        }
    }

    private IEnumerator LoadSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Ожидать заданное количество секунд
        SceneManager.LoadScene("company"); // Загрузить сцену "game"
    }
}
