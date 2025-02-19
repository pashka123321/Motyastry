using System.Collections;
using UnityEngine;

public class ЗвукПриСтарте : MonoBehaviour
{
    public AudioSource audioSource; // Аудиоисточник
    public AudioClip audioClip; // Аудиофайл
    public float delay = 1f; // Задержка перед стартом
    public bool плавноеНачало = true; // Флаг плавного старта
    public float fadeDuration = 2f; // Время на увеличение громкости

    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource не назначен!");
            return;
        }

        if (audioClip == null)
        {
            Debug.LogError("AudioClip не назначен!");
            return;
        }

        StartCoroutine(PlaySoundWithDelay());
    }

    IEnumerator PlaySoundWithDelay()
    {
        yield return new WaitForSeconds(delay);

        audioSource.clip = audioClip;
        audioSource.volume = 0f;

        if (плавноеНачало)
        {
            audioSource.Play();
            yield return StartCoroutine(FadeIn(audioSource, fadeDuration));
        }
        else
        {
            audioSource.volume = 1f;
            audioSource.Play();
        }
    }

    IEnumerator FadeIn(AudioSource source, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            source.volume = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        source.volume = 1f; // Гарантируем, что дойдёт до 1
    }
}
