using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100; // Максимальное здоровье
    private int currentHealth;
    public AudioClip[] deathSounds; // Массив звуков смерти
    private AudioSource audioSource;

    public List<GameObject> objectsToDestroy; // Список объектов для удаления

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
        currentHealth -= damage; // Уменьшаем здоровье на величину урона

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

        // Удаляем текущий объект
        Destroy(gameObject);
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
}
