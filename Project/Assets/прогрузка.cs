using UnityEngine;
using System.Collections;
public class SoundPreloader : MonoBehaviour
{
    public AudioClip[] audioClips;  // Массив для хранения всех звуков и музыки, которые нужно загрузить

    void Start()
    {
        // Проходимся по массиву и загружаем все звуки и музыку
        foreach (var clip in audioClips)
        {
            // Создаем временный объект для звука
            GameObject soundObject = new GameObject("TempAudio");
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();

            // Устанавливаем звук и громкость
            audioSource.clip = clip;
            audioSource.volume = 0.001f; // Устанавливаем громкость в 0.1%

            // Проигрываем звук
            audioSource.Play();

            // Запускаем корутину для остановки звука через 1 секунду
            StartCoroutine(StopMusicAfterDelay(soundObject, 1f));
        }
    }

    private IEnumerator StopMusicAfterDelay(GameObject soundObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        AudioSource audioSource = soundObject.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // Уничтожаем объект после остановки звука
        Destroy(soundObject);
    }
}
