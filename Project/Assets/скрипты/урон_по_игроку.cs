using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float damageAmount = 10f;
    public float damageInterval = 0.25f; // Интервал между нанесением урона

    private float lastDamageTime; // Время последнего нанесения урона

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем столкновение с игроком
        if (other.CompareTag("Player"))
        {
            // Проверяем интервал между нанесениями урона
            if (Time.time >= lastDamageTime + damageInterval)
            {
                // Наносим урон игроку
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount);
                    Debug.Log("Enemy dealt " + damageAmount + " damage to the player."); // Отладочное сообщение
                }
                lastDamageTime = Time.time; // Обновляем время последнего нанесения урона
            }
        }
    }
}
