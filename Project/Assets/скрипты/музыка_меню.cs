using UnityEngine;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public List<AudioClip> menuMusicList; // Список префабов музыки
    private AudioSource audioSource;

    void Start()
    {
        // Добавляем компонент AudioSource, если его нет
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Настраиваем AudioSource
        audioSource.loop = false; // Отключаем автоматическое повторение одного трека
        audioSource.playOnAwake = false;

        // Запускаем воспроизведение случайной музыки
        PlayRandomMusic();
    }

    void Update()
    {
        // Проверяем, играет ли музыка, если нет - запускаем следующий случайный трек
        if (!audioSource.isPlaying)
        {
            PlayRandomMusic();
        }
    }

    void PlayRandomMusic()
    {
        if (menuMusicList.Count > 0)
        {
            AudioClip randomClip = menuMusicList[Random.Range(0, menuMusicList.Count)];
            audioSource.clip = randomClip;
            audioSource.Play();
        }
    }

    void OnDestroy()
    {
        // Останавливаем музыку при выходе из сцены
        audioSource.Stop();
    }
}
