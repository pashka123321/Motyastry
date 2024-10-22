using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip musicClip; // Сюда перетащите ваш прифаб музыки
    public float volume = 0.5f; // Громкость от 0.0 (без звука) до 1.0 (максимальная громкость)
    public AudioSource audioSource; // Сюда перетащите ваш компонент AudioSource

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Этот объект не будет уничтожен при загрузке новой сцены
    }

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Если AudioSource не назначен, добавляем компонент
        }
        audioSource.clip = musicClip; // Устанавливаем клип
        audioSource.loop = true; // Устанавливаем бесконечное повторение
        audioSource.volume = volume; // Устанавливаем громкость
        audioSource.Play(); // Начинаем воспроизведение
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
