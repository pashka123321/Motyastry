using UnityEngine;

public class Turretspider : MonoBehaviour
{
    public float detectionRadius = 15f; // Радиус обнаружения
    public float rotationSpeed = 5f; // Скорость поворота турели
    public Transform turretHead; // Ссылка на объект турели (ее "голову" или ствол)
    public GameObject bulletPrefab; // Префаб пули
    public Transform firePoint; // Точка, из которой будет вылетать пуля
    public float fireRate = 1f; // Скорость стрельбы (выстрелы в секунду)
    public float bulletSpeed = 10f; // Скорость пули
    public AudioClip fireSound; // Звук выстрела
    public AudioSource audioSource; // Источник звука

    private Transform target; // Текущая цель
    private float fireCooldown = 0f; // Таймер до следующего выстрела

    void Update()
    {
        FindTarget(); // Ищем цель в пределах радиуса
        AimAtTarget(); // Наводим турель на цель
        ShootAtTarget(); // Стреляем, если есть цель
    }

    void FindTarget()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius); // Ищем все объекты в радиусе

        float closestDistance = detectionRadius; // Изначально считаем, что ближайшая цель на краю радиуса
        target = null; // Сбрасываем цель

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player") || collider.CompareTag("Blockwall")) // Проверяем тег
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position); // Вычисляем дистанцию до цели

                if (distance < closestDistance) // Если цель ближе, чем предыдущая
                {
                    closestDistance = distance;
                    target = collider.transform; // Назначаем новую цель
                }
            }
        }
    }

    void AimAtTarget()
    {
        if (target != null) // Если есть цель
        {
            Vector2 direction = target.position - turretHead.position; // Вычисляем направление на цель
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Вычисляем угол
            targetAngle -= 90f; // Поворачиваем угол на 90 градусов

            float currentAngle = turretHead.rotation.eulerAngles.z; // Текущий угол поворота турели
            float angle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed); // Интерполяция угла для плавного вращения

            turretHead.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Поворачиваем турель
        }
    }

    void ShootAtTarget()
    {
        if (target != null && fireCooldown <= 0f) // Если есть цель и можно стрелять
        {
            // Создаем пулю в точке огня
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Добавляем пуле скорость
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = firePoint.up * bulletSpeed;
            }

            // Воспроизводим звук выстрела
            if (audioSource != null && fireSound != null)
            {
                audioSource.PlayOneShot(fireSound);
            }

            // Сбрасываем таймер до следующего выстрела
            fireCooldown = 1f / fireRate;
        }

        // Обновляем таймер
        fireCooldown -= Time.deltaTime;
    }

    void OnDrawGizmosSelected()
    {
        // Визуализация радиуса обнаружения в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
