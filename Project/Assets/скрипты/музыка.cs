using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager1 : MonoBehaviour
{
    [System.Serializable]
    public class MusicTrack
    {
        public AudioClip clip;
        [Range(0, 100)]
        public float volume = 100f;
        public bool isBattleTrack; // Указывает, является ли трек боевым
    }

    public MusicTrack[] musicTracks;
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;
    private int lastTrackIndex = -1; // Исправлено: добавлена переменная
    private bool isBattleMusicPlaying = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayRandomBackgroundMusic());
    }

public void PlayBattleMusic()
{
    StopCurrentMusic(); // Останавливаем текущую фоновую музыку
    isBattleMusicPlaying = true; // Отмечаем, что началась боевая музыка

    var battleTracks = new List<MusicTrack>();
    foreach (var track in musicTracks)
    {
        if (track.isBattleTrack)
            battleTracks.Add(track);
    }

    if (battleTracks.Count > 0)
    {
        int randomIndex = Random.Range(0, battleTracks.Count);
        StartCoroutine(FadeInMusic(battleTracks[randomIndex]));
    }
}

public void StopBattleMusic()
{
    // Проверяем, если играла боевая музыка, то завершить её
    if (isBattleMusicPlaying)
    {
        StartCoroutine(FadeOutMusic());
        isBattleMusicPlaying = false; // Сбрасываем флаг боевой музыки
    }
    else
    {
        // Если боевой музыки не было, то ничего не делаем, фоновая продолжает играть
        Debug.Log("Фоновая музыка продолжает играть, боевой музыки не было.");
    }
}

    private IEnumerator FadeInMusic(MusicTrack track)
    {
        audioSource.clip = track.clip;
        audioSource.volume = 0f;
        audioSource.Play();

        float targetVolume = track.volume / 100f;
        float fadeDuration = 3f; // Время нарастания громкости
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOutMusic()
    {
        float fadeDuration = 3f; // Время затухания
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 0f;
    }

    private void StopCurrentMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

private IEnumerator PlayRandomBackgroundMusic()
{
    // Начальная пауза перед запуском первого трека
    yield return new WaitForSeconds(Random.Range(15, 60)); 

    while (true)
    {
        if (!audioSource.isPlaying)
        {
            int newTrackIndex = GetRandomBackgroundTrackIndex();
            audioSource.clip = musicTracks[newTrackIndex].clip;
            audioSource.volume = 0f; // Начинаем с нулевой громкости

            audioSource.Play();

            float targetVolume = musicTracks[newTrackIndex].volume / 100f;
            float fadeDuration = 3f; // Время нарастания громкости
            float currentTime = 0f;

            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / fadeDuration);
                yield return null;
            }

            audioSource.volume = targetVolume;

            // Ждём окончания трека и случайную паузу перед следующим
            yield return new WaitForSeconds(audioSource.clip.length + Random.Range(15, 60)); 
        }
        else
        {
            yield return null;
        }
    }
}


    private int GetRandomBackgroundTrackIndex()
    {
        List<int> backgroundTrackIndices = new List<int>();

        for (int i = 0; i < musicTracks.Length; i++)
        {
            if (!musicTracks[i].isBattleTrack) // Исключаем боевые треки
            {
                backgroundTrackIndices.Add(i);
            }
        }

        // Выбираем случайный индекс из фоновых треков
        int newTrackIndex;
        do
        {
            newTrackIndex = backgroundTrackIndices[Random.Range(0, backgroundTrackIndices.Count)];
        } while (newTrackIndex == lastTrackIndex); // Избегаем повтора трека

        lastTrackIndex = newTrackIndex; // Сохраняем индекс последнего трека
        return newTrackIndex;
    }
}
