using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class EnemyAI : MonoBehaviour
{
    public Transform player; // Ссылка на объект игрока
    public float speed = 2.0f; // Скорость движения врага
    public float minDesiredDistance = 10.0f; // Минимальное желаемое расстояние между врагом и игроком
    public float maxDesiredDistance = 15.0f; // Максимальное желаемое расстояние между врагом и игроком
    public float retreatDistance = 8.0f; // Расстояние для отступления
    public bool isClone = false; // Флаг для определения, является ли объект клоном
    public int damage = 10; // Урон, который наносит враг при попадании
    public GameObject bulletPrefab; // Префаб пули
    public Transform firePoint; // Точка, откуда выпускаются пули
    public float fireRate = 1.0f; // Частота стрельбы (выстрелов в секунду)
    public float bulletSpeed = 10.0f; // Скорость пули
    public float rotationSpeed = 200.0f; // Скорость поворота врага (градусы в секунду)
    public float shootAngleThreshold = 5.0f; // Максимальный угол отклонения для стрельбы
    public float acceleration = 5.0f; // Ускорение при движении
    public float deceleration = 5.0f; // Замедление при остановке

    public float floatAmplitude = 0.5f; // Амплитуда планирования
    public float floatFrequency = 1.0f; // Частота планирования

    private float fireTimer; // Таймер для отслеживания частоты стрельбы
    private Rigidbody2D rb;
    private Vector2 currentVelocity; // Текущая скорость врага
    private Vector2 recoilForce; // Сила отдачи
    private float recoilDuration = 0.2f; // Длительность отдачи
    private float recoilTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        fireTimer = 0.0f;
        currentVelocity = Vector2.zero;
        recoilForce = Vector2.zero;
        recoilTimer = 0.0f;
    }

void FixedUpdate()
{
    if (isClone && player != null)
    {
        Vector3 direction = player.position - transform.position;
        float distanceToPlayer = direction.magnitude;
        Vector2 targetVelocity = Vector2.zero;

        // Добавляем планирование (плавное движение по синусоиде)
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        if (distanceToPlayer < retreatDistance)
        {
            targetVelocity = -direction.normalized * speed;
        }
        else if (distanceToPlayer >= minDesiredDistance && distanceToPlayer <= maxDesiredDistance)
        {
            targetVelocity = new Vector2(0, floatOffset);
        }
        else if (distanceToPlayer < minDesiredDistance)
        {
            targetVelocity = -direction.normalized * speed;
        }
        else
        {
            targetVelocity = new Vector2(direction.normalized.x, direction.normalized.y) * speed + new Vector2(0, floatOffset);
        }

        // Плавное изменение скорости
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, (targetVelocity == Vector2.zero ? deceleration : acceleration) * Time.fixedDeltaTime);

        // Применение отдачи при попадании
        if (recoilTimer > 0)
        {
            recoilTimer -= Time.fixedDeltaTime;
            rb.velocity = recoilForce; // Применяем силу отдачи
        }
        else
        {
            rb.velocity = currentVelocity;
        }

        // Поворот к игроку
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 0f;
        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(angle);

        // Стрельба
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle));
        if (angleDifference < shootAngleThreshold && distanceToPlayer <= maxDesiredDistance)
        {
            fireTimer += Time.fixedDeltaTime;
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
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bulletObj.transform.Rotate(0, 0, -90);
        Rigidbody2D bulletRB = bulletObj.GetComponent<Rigidbody2D>();
        if (bulletRB != null)
        {
            bulletRB.velocity = firePoint.right * bulletSpeed;
        }
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.damage = damage;
        }
    }

    public void ApplyRecoil(Vector2 force)
    {
        recoilForce = force;
        recoilTimer = recoilDuration;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 repelDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(repelDirection * speed, ForceMode2D.Impulse);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damageAmount, Vector2 impactDirection)
    {
        // Логика получения урона
        ApplyRecoil(impactDirection * 2.0f); // Примерно отталкиваем врага
    }
}
