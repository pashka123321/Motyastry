using UnityEngine;

public class Bullet2 : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 40;
    public float lifeTime = 2f; // Время жизни пули
    private Rigidbody2D rb;

    void Start()
    {
        // Поворачиваем текстуру ракеты на 90 градусов
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + -90);
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
        }
        Destroy(gameObject);
    }
}
