using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100; // Максимальное здоровье
    private float currentHealth;
    public AudioClip[] deathSounds; // Массив звуков смерти
    public AudioSource customAudioSource; // Пользовательский источник звука для смерти
    private AudioSource defaultAudioSource; // Стандартный источник звука

    public List<GameObject> objectsToDestroy; // Список объектов для удаления

    [SerializeField, Range(0, 1)] private float[] damageResistance;

    public GameObject explosionPrefab; // Префаб взрыва
    public Transform explosionPosition; // Позиция для появления взрыва

    public float cameraShakeDuration = 0.5f; // Длительность тряски камеры
    public float cameraShakeMagnitude = 0.2f; // Сила тряски камеры

    void Start()
    {
        currentHealth = maxHealth; // Устанавливаем текущее здоровье
        defaultAudioSource = GetComponent<AudioSource>(); // Получаем компонент AudioSource
        if (defaultAudioSource == null)
        {
            defaultAudioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage, int damageType)
    {
        currentHealth -= damage * (1f - damageResistance[damageType]);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Выбираем случайный звук из массива deathSounds
        if (deathSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, deathSounds.Length);
            AudioSource audioSourceToUse = customAudioSource != null ? customAudioSource : defaultAudioSource;
            audioSourceToUse.clip = deathSounds[randomIndex];
            audioSourceToUse.Play();
        }

        // Создание эффекта взрыва
        GameObject explosionInstance = null;
        if (explosionPrefab != null && explosionPosition != null)
        {
            explosionInstance = Instantiate(explosionPrefab, explosionPosition.position, explosionPosition.rotation);
            // Удаляем взрыв через 2 секунды
            Destroy(explosionInstance, 2f);
        }

        // Вызов тряски камеры
        if (CameraShakeController.instance != null)
        {
            CameraShakeController.instance.ShakeCamera(cameraShakeDuration, cameraShakeMagnitude);
        }

        // Уничтожаем указанные объекты
        DestroyObjects(objectsToDestroy);

        // Проверяем, является ли объект клоном
        if (IsClone())
        {
            // Если объект является клоном, удаляем его
            Destroy(gameObject);
        }
    }

    void DestroyObjects(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }

    bool IsClone()
    {
        // Проверяем, находится ли объект в активной сцене
        return gameObject.scene.isLoaded;
    }
}
