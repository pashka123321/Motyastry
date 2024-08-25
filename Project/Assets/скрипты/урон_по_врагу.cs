using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100; // Максимальное здоровье
    private float currentHealth;
    public AudioClip[] deathSounds; // Массив звуков смерти
    private AudioSource audioSource;

    public List<GameObject> objectsToDestroy; // Список объектов для удаления

    [SerializeField, Range(0, 1)] private float[] damageResistance;

    void Start()
    {
        currentHealth = maxHealth; // Устанавливаем текущее здоровье
        audioSource = GetComponent<AudioSource>(); // Получаем компонент AudioSource
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
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
            audioSource.clip = deathSounds[randomIndex];
            audioSource.Play();
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
