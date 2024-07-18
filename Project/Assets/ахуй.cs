using UnityEngine;
using UnityEngine.UI;

public class PlaySoundOnClick : MonoBehaviour
{
    public AudioClip audioClip;  // Ссылка на аудиоклип
    public Button button;        // Ссылка на кнопку

    private AudioSource audioSource;

    void Start()
    {
        // Создадим или получим компонент AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        
        // Присвоим аудиоклип
        audioSource.clip = audioClip;

        // Убедимся, что у нас есть ссылка на кнопку и аудиофайл
        if (button != null)
        {
            // Добавим слушателя нажатия на кнопку
            button.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.Play();
        }
    }
}
