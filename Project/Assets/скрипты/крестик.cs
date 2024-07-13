using UnityEngine;

public class HideObjectOnButtonClick : MonoBehaviour
{
    public GameObject objectToHide; // Ссылка на объект, который нужно скрыть
    public AudioClip[] audioClips; // Массив звуковых клипов
    public AudioSource audioSource; // Внешний аудиокомпонент для воспроизведения звуков

    // Start is called before the first frame update
    void Start()
    {
        // Проверяем, что ссылка на объект установлена
        if (objectToHide == null)
        {
            Debug.LogError("Object to hide is not assigned!");
            return;
        }

        // Проверяем, что массив звуков не пуст
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogError("Audio clips are not assigned or empty!");
            return;
        }

        // Проверяем, что аудиокомпонент установлен
        if (audioSource == null)
        {
            Debug.LogError("Audio source is not assigned!");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Проверяем, нажата ли клавиша Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Проверяем, что ссылка на объект установлена
            if (objectToHide == null)
            {
                Debug.LogError("Object to hide is not assigned!");
                return;
            }

            // Скрываем объект
            objectToHide.SetActive(false);

            // Воспроизводим случайный звук
            PlayRandomSound();
        }
    }

    // Вызывается при нажатии на кнопку
    public void OnButtonClick()
    {
        // Проверяем, что ссылка на объект установлена
        if (objectToHide == null)
        {
            Debug.LogError("Object to hide is not assigned!");
            return;
        }

        // Скрываем объект
        objectToHide.SetActive(false);

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
