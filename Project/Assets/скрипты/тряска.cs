using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Camera[] cameras; // Префабы камер
    [SerializeField] private float delayBeforeShake = 1.7f; // Задержка перед началом тряски
    [SerializeField] private float shakeDuration = 0.2f; // Длительность тряски
    [SerializeField] private float shakeMagnitude = 0.1f; // Амплитуда тряски
    [SerializeField] private float dampingSpeed = 1.0f; // Скорость затухания тряски
    [SerializeField] private AudioClip[] shakeSounds; // Массив звуков тряски

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayRandomSoundAndShake());
    }

    private IEnumerator PlayRandomSoundAndShake()
    {
        // Воспроизводим случайный звук из массива
        yield return new WaitForSeconds(delayBeforeShake);
        if (audioSource != null && shakeSounds != null && shakeSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, shakeSounds.Length);
            audioSource.PlayOneShot(shakeSounds[randomIndex]);
        }

        // Начинаем трясти камеры
        foreach (Camera cam in cameras)
        {
            StartCoroutine(Shake(cam));
        }
    }

    private IEnumerator Shake(Camera cam)
    {
        Vector3 originalPosition = cam.transform.position;
        float elapsed = 0.0f;
        float currentMagnitude = shakeMagnitude;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * currentMagnitude;
            float y = Random.Range(-1f, 1f) * currentMagnitude;

            cam.transform.position = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            currentMagnitude = Mathf.Lerp(shakeMagnitude, 0f, elapsed / shakeDuration); // Плавное затухание тряски

            yield return null;
        }

        cam.transform.position = originalPosition;
    }
}
