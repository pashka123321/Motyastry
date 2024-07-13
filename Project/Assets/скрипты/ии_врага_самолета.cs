using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class EnemyAI : MonoBehaviour
{
    public Transform player; // Ссылка на объект игрока
    public float speed = 2.0f; // Скорость движения врага
    public float desiredDistance = 15.0f; // Желаемое расстояние между врагом и игроком
    public bool isClone = false; // Флаг для определения, является ли объект клоном
    public int damage = 10; // Урон, который наносит враг при попадании

    public GameObject bulletPrefab; // Префаб пули
    public Transform firePoint; // Точка, откуда выпускаются пули
    public float fireRate = 1.0f; // Частота стрельбы (выстрелов в секунду)
    public float bulletSpeed = 10.0f; // Скорость пули

    private float fireTimer; // Таймер для отслеживания частоты стрельбы
    private Rigidbody2D rb;

    void Start()
    {
        // Получаем компонент Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        // Настройка Rigidbody2D
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        fireTimer = 0.0f; // Инициализируем таймер стрельбы
    }

    void Update()
    {
        // Проверяем, если объект является клоном, тогда двигаемся и стреляем
        if (isClone)
        {
            // Проверяем, задан ли игрок
            if (player != null)
            {
                // Направление к игроку
                Vector3 direction = player.position - transform.position;
                float distanceToPlayer = direction.magnitude;

                // Проверяем текущее расстояние и корректируем движение, если необходимо
                if (distanceToPlayer < desiredDistance)
                {
                    // Нужно двигаться назад от игрока
                    direction *= -1;
                }
                direction.Normalize();

                // Перемещение врага с использованием Rigidbody2D
                rb.velocity = direction * speed;

                // Поворот врага в сторону игрока
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

                // Стрельба
                fireTimer += Time.deltaTime;
                if (fireTimer >= 1.0f / fireRate)
                {
                    Fire();
                    fireTimer = 0.0f;
                }
            }
        }
    }

void Fire()
{
    // Создаем пулю из префаба
    GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

    // Настроим скорость пули
    Rigidbody2D bulletRB = bulletObj.GetComponent<Rigidbody2D>();
    if (bulletRB != null)
    {
        bulletRB.velocity = firePoint.right * bulletSpeed; // Устанавливаем скорость пули в направлении firePoint.right
    }

    // Наносим урон игроку, если пуля попала в него
    Bullet bullet = bulletObj.GetComponent<Bullet>();
    if (bullet != null)
    {
        bullet.damage = damage; // Устанавливаем урон пули
    }
}

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем, если столкновение произошло с другим врагом
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Отталкиваемся от другого врага
            Vector2 repelDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(repelDirection * speed, ForceMode2D.Impulse);

            // Отладочная информация
            Debug.Log("Collision with another enemy");
            Debug.DrawRay(transform.position, repelDirection, Color.red, 1.0f);
        }

        // Проверяем, если столкновение произошло с игроком
        if (collision.gameObject.CompareTag("Player"))
        {
            // Получаем компонент игрока и наносим урон
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            
            // Уничтожаем пулю после попадания в игрока
            Destroy(gameObject);
        }
    }
}
