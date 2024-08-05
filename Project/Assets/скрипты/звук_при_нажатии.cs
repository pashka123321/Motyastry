using UnityEngine;

public class StartGame123 : MonoBehaviour
{
    public AudioClip[] buttonClickSounds; // Массив аудиоклипов для звука кнопки
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Добавление компонента AudioSource к объекту
    }

    public void PlayButtonClickSound()
    {
        PlayRandomSound(); // Воспроизвести случайный звук
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
}
