using UnityEngine;
using System.Collections; // Добавляем директиву для использования System.Collections

public class ExitGame : MonoBehaviour
{
    public AudioClip[] exitSounds; // Массив звуков для выхода
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ExitGameOnClick()
    {
        // Выбираем случайный звук из массива
        if (exitSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, exitSounds.Length);
            audioSource.PlayOneShot(exitSounds[randomIndex]);
        }

        // Запускаем корутину, которая через секунду вызовет метод выхода из игры
        StartCoroutine(ExitAfterDelay());
    }

    IEnumerator ExitAfterDelay()
    {
        yield return new WaitForSeconds(1f); // Ждем 1 секунду
        Debug.Log("Выход из игры.");
        Application.Quit();
    }
}
