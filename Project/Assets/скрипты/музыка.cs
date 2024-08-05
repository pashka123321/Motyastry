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
    }

    public MusicTrack[] musicTracks;
    private AudioSource audioSource;
    private int lastTrackIndex = -1;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayRandomMusicWithInterval());
    }

    private IEnumerator PlayRandomMusicWithInterval()
    {
        while (true)
        {
            if (!audioSource.isPlaying)
            {
                int newTrackIndex = GetRandomTrackIndex();
                audioSource.clip = musicTracks[newTrackIndex].clip;
                audioSource.volume = 0f; // Начинаем с нулевой громкости

                audioSource.Play();

                // Постепенно наращиваем громкость
                float targetVolume = musicTracks[newTrackIndex].volume / 100f;
                float fadeDuration = 3f; // Время, за которое громкость достигнет максимума (в секундах)
                float currentTime = 0f;

                while (currentTime < fadeDuration)
                {
                    currentTime += Time.deltaTime;
                    audioSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / fadeDuration);
                    yield return null;
                }

                audioSource.volume = targetVolume;

                // Ждем, пока трек не закончится, плюс интервал 1-2 минуты
                float trackDuration = audioSource.clip.length;
                float interval = Random.Range(15, 30); // 1-2 минуты в секундах
                yield return new WaitForSeconds(trackDuration + interval);
            }
            else
            {
                yield return null;
            }
        }
    }

    private int GetRandomTrackIndex()
    {
        int newTrackIndex;

        // Выбираем случайный трек, отличный от предыдущего
        do
        {
            newTrackIndex = Random.Range(0, musicTracks.Length);
        } while (newTrackIndex == lastTrackIndex);

        lastTrackIndex = newTrackIndex;
        return newTrackIndex;
    }
}
