using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 40;
    public float lifeTime = 2f; // Время жизни пули
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector2 direction)
    {
        rb.velocity = direction * speed;
        Destroy(gameObject, lifeTime); // Уничтожаем пулю через определенное время только у клонов
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Enemy enemy = hitInfo.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Уничтожаем пулю только при контакте с врагом
        }
    }
}