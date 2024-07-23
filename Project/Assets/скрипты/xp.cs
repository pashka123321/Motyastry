using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public HealthBar healthBar; // Ссылка на компонент HealthBar для отображения здоровья игрока

    public AudioClip[] hurtSounds; // Массив звуков урона
    public AudioClip[] deathSounds; // Массив звуков при смерти
    private AudioSource audioSource; // Аудиоисточник для проигрывания звуков

    public CameraFollow cameraFollow; // Ссылка на скрипт CameraFollow
    public Transform respawnPoint; // Точка респавна игрока

    private PlayerMovement playerMovement; // Ссылка на скрипт движения игрока

    public bool isAlive { get; private set; } = true; // Добавлено свойство isAlive

    public bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }

        // Получаем компонент AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Найти скрипт CameraFollow, если он не привязан в инспекторе
        if (cameraFollow == null)
        {
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
        }

        // Получаем скрипт PlayerMovement
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible) return;

        if (!isAlive) return; // Добавлена проверка на живого игрока

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }

        // Проигрываем случайный звук урона
        PlayRandomHurtSound();

        // Запускаем тряску камеры при получении урона
        if (cameraFollow != null)
        {
            StartCoroutine(cameraFollow.ShakeCamera(0.2f, 0.3f)); // Параметры тряски можно настроить
        }

        if (currentHealth <= 0f)
        {
            Die(); // Если здоровье игрока меньше или равно 0, вызываем метод "умереть"
            isInvincible = true;
            Invoke(nameof(InvincibleTimer), 1.5f);
        }
        else
        {
            isInvincible = true;
            Invoke(nameof(InvincibleTimer), 0.25f);
        }
    }

    private void InvincibleTimer()
    {
        isInvincible = false;
    }

    void PlayRandomHurtSound()
    {
        if (hurtSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, hurtSounds.Length);
            AudioClip hurtSound = hurtSounds[randomIndex];
            audioSource.PlayOneShot(hurtSound);
        }
    }

    void Die()
    {
        isAlive = false; // Устанавливаем isAlive в false

        // Логика смерти игрока
        Debug.Log("Player died");

        // Play random death sound
        PlayRandomDeathSound();

        if (cameraFollow != null)
        {
            cameraFollow.StopCamera(); // Останавливаем камеру
        }

        if (playerMovement != null)
        {
            playerMovement.canControl = false; // Отключаем управление
        }

        StartCoroutine(DeathAnimation()); // Запускаем корутину смерти
    }

    void PlayRandomDeathSound()
    {
        if (deathSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, deathSounds.Length);
            AudioClip deathSound = deathSounds[randomIndex];
            audioSource.PlayOneShot(deathSound);
        }
    }

    IEnumerator DeathAnimation()
    {
        // Убираем изменение масштаба
        yield return new WaitForSeconds(1f); // Задержка перед респавном

        if (respawnPoint != null)
        {
            // Перемещаем игрока к точке респавна
            transform.position = respawnPoint.position;
            // Восстанавливаем здоровье
            currentHealth = maxHealth;
            if (healthBar != null)
            {
                healthBar.SetHealth(currentHealth, maxHealth);
            }
            isAlive = true; // Восстанавливаем isAlive при респавне
            Debug.Log("Player respawned at " + respawnPoint.position);
        }
        else
        {
            Debug.LogWarning("Respawn point is not set!");
        }

        if (cameraFollow != null)
        {
            cameraFollow.ResumeCamera(); // Возобновляем следование камеры
        }

        if (playerMovement != null)
        {
            playerMovement.canControl = true; // Включаем управление
        }
    }
}
