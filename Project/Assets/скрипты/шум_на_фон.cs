using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip musicClip; // Сюда перетащите ваш прифаб музыки
    public float volume = 0.5f; // Громкость от 0.0 (без звука) до 1.0 (максимальная громкость)
    private AudioSource audioSource; // Приватная переменная для AudioSource

    void Awake()
    {
        CreateAudioSource(); // Создаем и настраиваем AudioSource
    }

    void Start()
    {
        audioSource.Play(); // Начинаем воспроизведение музыки при старте сцены
    }

    void CreateAudioSource()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Добавляем компонент AudioSource
        audioSource.clip = musicClip; // Устанавливаем клип
        audioSource.loop = true; // Устанавливаем бесконечное повторение
        audioSource.volume = volume; // Устанавливаем громкость
        audioSource.playOnAwake = false; // Запрещаем автоматическое воспроизведение при запуске
    }

    // Метод для изменения громкости программно
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp(newVolume, 0.0f, 1.0f); // Ограничиваем громкость от 0.0 до 1.0
        if (audioSource != null)
        {
            audioSource.volume = volume; // Обновляем громкость AudioSource
        }
    }

    void FixedUpdate()
    {
        if (audioSource != null && audioSource.volume != volume)
        {
            audioSource.volume = volume; // Постоянно обновляем громкость до заданного уровня
        }
    }
}
