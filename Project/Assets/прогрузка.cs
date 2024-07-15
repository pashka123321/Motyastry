using UnityEngine;

public class SoundPreloader : MonoBehaviour
{
    [System.Serializable]
    public class SoundInfo
    {
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 0f;
    }

    public SoundInfo[] soundInfos;

    void Start()
    {
        foreach (var soundInfo in soundInfos)
        {
            GameObject tempGO = new GameObject("TempAudio"); // Создаем временный GameObject
            AudioSource audioSource = tempGO.AddComponent<AudioSource>(); // Добавляем компонент AudioSource
            audioSource.clip = soundInfo.clip; // Устанавливаем AudioClip
            audioSource.volume = soundInfo.volume; // Устанавливаем громкость

            // Воспроизводим звук в точке (0, 0, 0) с заданной громкостью
            AudioSource.PlayClipAtPoint(soundInfo.clip, Vector3.zero);

            Destroy(tempGO, soundInfo.clip.length); // Удаляем временный GameObject после окончания звука
        }
    }
}
