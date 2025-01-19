using UnityEngine;
using System.Collections;

public class SpiralFlight : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(0, 40, 0); // Начальная позиция (из космоса)
    public Vector3 targetPosition = Vector3.zero; // Конечная позиция
    public float duration = 2f; // Длительность анимации
    public float spiralIntensity = 2.8f; // Интенсивность закручивания
    public float rotationSpeed = 370f; // Скорость вращения вокруг своей оси
    public GameObject landingParticlePrefab; // Префаб партиклов для приземления
    public AudioClip landingSound; // Звук при приземлении (открытие ящика)
    public AudioClip explosionSound; // Звук взрыва

    public Material fireMaterial; // Огненный материал (шейдер)
    private Material originalMaterial; // Оригинальный материал объекта

    private AudioSource audioSource; // Источник звука
    private Renderer objectRenderer; // Рендер объекта
    private bool isAnimating = false; // Флаг, чтобы предотвратить повторный запуск анимации

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material; // Сохраняем оригинальный материал
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isAnimating)
        {
            StartCoroutine(AnimateSpiralFlight());
        }
    }

    private IEnumerator AnimateSpiralFlight()
    {
        isAnimating = true;
        float timeElapsed = 0f;

        Vector3 initialPosition = startPosition;
        transform.position = initialPosition;

        // Заменяем материал на огненный в начале анимации
        objectRenderer.material = fireMaterial;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            Vector3 position = Vector3.Lerp(initialPosition, targetPosition, Mathf.SmoothStep(0, 1, t));

            float angle = t * Mathf.PI * 2 * spiralIntensity;
            float radius = Mathf.Lerp(2f, 0f, t);

            position.x += Mathf.Cos(angle) * radius;
            position.y += Mathf.Sin(angle) * radius;

            transform.position = position;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, 360f, t * rotationSpeed / 360f));

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        // Устанавливаем позицию и поворот в точку приземления
        transform.position = targetPosition;
        transform.rotation = Quaternion.identity;

        // Возвращаем оригинальный материал с плавным исчезновением огненного материала
        StartCoroutine(FadeOutFireMaterial());

        if (landingSound != null)
        {
            StartCoroutine(PlaySoundWithFadeOut(landingSound));
        }

        if (explosionSound != null)
        {
            StartCoroutine(PlaySoundWithFadeOut(explosionSound));
        }

        if (landingParticlePrefab != null)
        {
            GameObject particleInstance = Instantiate(landingParticlePrefab, targetPosition, Quaternion.identity);
            Destroy(particleInstance, 3f);
        }

        isAnimating = false;
    }

    // Новый метод для плавного исчезновения огненного материала
    private IEnumerator FadeOutFireMaterial()
    {
        float fadeDuration = 1f;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            float t = timeElapsed / fadeDuration;
            objectRenderer.material.Lerp(fireMaterial, originalMaterial, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        objectRenderer.material = originalMaterial;
    }

    private IEnumerator PlaySoundWithFadeOut(AudioClip clip)
    {
        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.volume = 0.5f;
        tempSource.Play();

        float duration = clip.length;
        float fadeDuration = 1f;
        float fadeStart = duration - fadeDuration;

        while (tempSource.isPlaying)
        {
            if (tempSource.time >= fadeStart)
            {
                float t = (tempSource.time - fadeStart) / fadeDuration;
                tempSource.volume = Mathf.Lerp(0.5f, 0f, t);
            }
            yield return null;
        }

        Destroy(tempSource);
    }
}
