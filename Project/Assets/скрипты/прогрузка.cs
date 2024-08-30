using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SoundPreloader : MonoBehaviour
{
    public AudioClip[] audioClips;   // Массив для хранения всех звуков и музыки
    public Slider progressBar;       // UI элемент для прогресс бара

    private List<AudioSource> audioSources = new List<AudioSource>(); // Список для хранения аудиоисточников
    private bool shouldPlayAfterLoad = false; // Переменная для управления проигрыванием музыки после загрузки

    void Start()
    {
        DontDestroyOnLoad(this.gameObject); // Чтобы объект SoundPreloader не уничтожался при смене сцен
        StartCoroutine(LoadSounds());
    }

    private IEnumerator LoadSounds()
    {
        int totalClips = audioClips.Length;
        int loadedClips = 0;

        foreach (var clip in audioClips)
        {
            // Создаем объект для звука и сохраняем его
            GameObject soundObject = new GameObject("Audio_" + clip.name);
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();

            // Настройка источника звука
            audioSource.clip = clip;
            audioSource.volume = 0.001f;  // Очень тихий звук
            audioSource.loop = true;      // Зацикливаем для постоянного воспроизведения

            // Сохраняем объект звука между сценами
            DontDestroyOnLoad(soundObject);

            // Проигрываем звук
            audioSource.Play();

            // Сохраняем источник звука в список
            audioSources.Add(audioSource);

            // Ждем секунду, имитируя задержку загрузки звука
            yield return new WaitForSeconds(0.5f);

            // Обновляем прогресс бар
            loadedClips++;
            progressBar.value = (float)loadedClips / totalClips;
        }

        // Останавливаем все звуки перед переходом на новую сцену
        StopAllSounds();

        // Переходим на сцену 'menu'
        SceneManager.LoadScene("menu");
    }

    // Метод для остановки всех звуков
    private void StopAllSounds()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.Stop();
        }
    }

    // Метод для регулировки громкости, если потребуется
    public void SetVolume(float volume)
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.volume = volume;
        }
    }
}
