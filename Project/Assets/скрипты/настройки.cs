using UnityEngine;
using UnityEngine.UI;

public class ShowHiddenObjectOnClick : MonoBehaviour
{
    public GameObject objectToShow; // Ссылка на объект, который нужно показать
    public AudioClip[] audioClips; // Массив звуковых клипов
    private AudioSource audioSource; // Аудиокомпонент для воспроизведения звуков

    // Start is called before the first frame update
    void Start()
    {
        // Проверяем, что ссылка на объект установлена
        if (objectToShow == null)
        {
            Debug.LogError("Object to show is not assigned!");
            return;
        }

        // Скрываем объект при запуске сцены
        objectToShow.SetActive(false);

        // Добавляем аудиокомпонент и проверяем, что массив звуков не пуст
        audioSource = gameObject.AddComponent<AudioSource>();
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogError("Audio clips are not assigned or empty!");
            return;
        }
    }

    // Вызывается при нажатии на кнопку
    public void OnButtonClick()
    {
        // Показываем скрытый объект
        objectToShow.SetActive(true);

        // Воспроизводим случайный звук
        PlayRandomSound();
    }

    // Метод для воспроизведения случайного звука
    private void PlayRandomSound()
    {
        int randomIndex = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[randomIndex];
        audioSource.Play();
    }
}
