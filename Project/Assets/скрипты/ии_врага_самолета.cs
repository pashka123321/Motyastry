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

    private float fireTimer; // Таймер для отслеживания частоты стрельбы
    private Rigidbody2D rb;
    private Vector2 currentVelocity; // Текущая скорость врага

    void Start()
    {
        // Получаем компонент Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        // Настройка Rigidbody2D
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        fireTimer = 0.0f; // Инициализируем таймер стрельбы
        currentVelocity = Vector2.zero; // Инициализируем текущую скорость
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
                Vector2 targetVelocity = Vector2.zero;

                if (distanceToPlayer < retreatDistance)
                {
                    // Если игрок слишком близко, отступаем
                    targetVelocity = -direction.normalized * speed;
                }
                else if (distanceToPlayer >= minDesiredDistance && distanceToPlayer <= maxDesiredDistance)
                {
                    // Если игрок находится в зоне, останавливаем движение
                    targetVelocity = Vector2.zero;
                }
                else if (distanceToPlayer < minDesiredDistance)
                {
                    // Если игрок ближе минимального желаемого расстояния, отступаем
                    targetVelocity = -direction.normalized * speed;
                }
                else
                {
                    // Иначе движемся к игроку
                    targetVelocity = direction.normalized * speed;
                }

                // Плавное изменение скорости для создания эффекта скольжения
                currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, (targetVelocity == Vector2.zero ? deceleration : acceleration) * Time.deltaTime);

                // Плавное перемещение врага
                Vector2 newPosition = rb.position + currentVelocity * Time.deltaTime;
                rb.MovePosition(newPosition);

                // Поворот врага в сторону игрока
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float currentAngle = transform.eulerAngles.z;
                float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

                // Стрельба только если враг нацелен на игрока и находится в радиусе 15 юнитов
                float angleDifference = Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle));
                if (angleDifference < shootAngleThreshold && distanceToPlayer <= maxDesiredDistance)
                {
                    fireTimer += Time.deltaTime;
                    if (fireTimer >= 1.0f / fireRate)
                    {
                        Fire();
                        fireTimer = 0.0f;
                    }
                }
            }
        }
    }

    void Fire()
    {
        // Создаем пулю из префаба
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Поворачиваем пулю на 90 градусов
        bulletObj.transform.Rotate(0, 0, -90);

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
            
            // Уничтожаем врага после столкновения с игроком
            Destroy(gameObject);
        }
    }
}