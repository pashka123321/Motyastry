using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserTurretController : MonoBehaviour
{
    public GameObject turretObject; // Префаб или объект, к которому нужно поворачивать турель
    public GameObject Bullet1; // Префаб пули
    public Transform firePoint; // Точка, откуда вылетают пули
    public float fireRate = 1f; // Скорость стрельбы (выстрелов в секунду)
    public float detectionRadius = 20f; // Радиус обнаружения врагов
    public float rotationSpeed = 5f; // Скорость поворота турели к врагу
    public AudioClip[] shootSounds; // Массив звуков выстрела
    public int leadIngotCount = 10; // Количество доступных слитков свинца
    public int maxLeadIngots = 50; // Максимальное количество слитков свинца, которое может храниться
    public string ammoTag = "слиток свинца"; // Тег для объектов пополнения боеприпасов
    public bool requiresAmmo = true; // Флаг, указывающий, требуется ли для стрельбы боеприпасы
    public bool isManualMode = false; // Ручное управление
    public LineRenderer lineRenderer; // LineRenderer для отображения молнии
    public float lightningDamage = 10f; // Урон от молнии
    public float lightningDuration = 0.1f; // Длительность отображения молнии

    private float fireCountdown = 0f;
    private AudioSource audioSource;
    private GameObject currentTarget; // Текущая цель турели

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isManualMode)
        {
            ManualControl(); // Ручное управление
        }
        else
        {
            AutomaticControl(); // Автоматическое управление
        }
    }

void ManualControl()
{
    // Поворот турели в сторону мышки
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    mousePos.z = 0; // Устанавливаем Z в 0, чтобы корректно поворачивать объект в 2D

    Vector3 direction = mousePos - turretObject.transform.position;
    Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
    turretObject.transform.rotation = Quaternion.RotateTowards(turretObject.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    // Проверка, достигла ли турель целевого угла поворота
    float angleDifference = Quaternion.Angle(turretObject.transform.rotation, targetRotation);
    if (angleDifference < 1f) // Порог, например, 1 градус
    {
        // Стрельба при нажатии ЛКМ
        if (Input.GetMouseButton(0) && fireCountdown <= 0f)
        {
            if (CanShoot())
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
        }
    }

    fireCountdown -= Time.deltaTime;
}


void AutomaticControl()
{
    if (requiresAmmo && leadIngotCount <= 0)
    {
        return; // Турель не реагирует на врагов, если включен режим боеприпасов и их нет в наличии
    }

    if (currentTarget == null)
    {
        currentTarget = FindRandomEnemyWithinRadius();
    }
    else
    {
        Vector3 direction = currentTarget.transform.position - turretObject.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
        turretObject.transform.rotation = Quaternion.RotateTowards(turretObject.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Проверка, достигла ли турель целевого угла поворота
        float angleDifference = Quaternion.Angle(turretObject.transform.rotation, targetRotation);
        if (angleDifference < 1f) // Порог, например, 1 градус
        {
            if (fireCountdown <= 0f)
            {
                if (CanShoot())
                {
                    Shoot();
                    fireCountdown = 1f / fireRate;
                }
            }
        }

        fireCountdown -= Time.deltaTime;

        // Проверяем, жива ли текущая цель
        if (!IsTargetAlive(currentTarget))
        {
            currentTarget = null; // Сбрасываем текущую цель, чтобы найти новую на следующем кадре
        }
    }
}


    void Shoot()
    {
        if (Bullet1 != null && firePoint != null)
        {
            GameObject bullet = Instantiate(Bullet1, firePoint.position, firePoint.rotation);
            Bullet1 bulletScript = bullet.GetComponent<Bullet1>();
            if (bulletScript != null)
            {
                bulletScript.SetDirection(firePoint.up); // Направление пули вверх от firePoint
            }

            if (requiresAmmo)
            {
                leadIngotCount--; // Уменьшаем количество боеприпасов
            }
        }

        if (shootSounds.Length > 0 && audioSource != null)
        {
            AudioClip randomShootSound = shootSounds[Random.Range(0, shootSounds.Length)];
            audioSource.PlayOneShot(randomShootSound, 0.3f); // Устанавливаем громкость на 30%
        }

        if (currentTarget != null)
        {
            StartCoroutine(ShootLightning(currentTarget));
        }
    }

    IEnumerator ShootLightning(GameObject target)
    {
        lineRenderer.enabled = true;
        Vector3[] positions = GenerateLightningPositions(firePoint.position, target.transform.position, 10);
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);

        // Наносим урон цели
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage((int)lightningDamage);
        }

        yield return new WaitForSeconds(lightningDuration);
        lineRenderer.enabled = false;
    }

    Vector3[] GenerateLightningPositions(Vector3 start, Vector3 end, int segments)
    {
        Vector3[] positions = new Vector3[segments + 1];
        positions[0] = start;
        positions[segments] = end;

        for (int i = 1; i < segments; i++)
        {
            float t = (float)i / segments;
            Vector3 point = Vector3.Lerp(start, end, t);
            point.x += Random.Range(-0.5f, 0.5f);
            point.y += Random.Range(-0.5f, 0.5f);
            positions[i] = point;
        }

        return positions;
    }

    bool CanShoot()
    {
        return !requiresAmmo || leadIngotCount > 0;
    }

    bool IsTargetAlive(GameObject target)
    {
        return target != null && target.activeSelf;
    }

    GameObject FindRandomEnemyWithinRadius()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        List<GameObject> enemies = new List<GameObject>();

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                enemies.Add(collider.gameObject);
            }
        }

        if (enemies.Count > 0)
        {
            int randomIndex = Random.Range(0, enemies.Count);
            return enemies[randomIndex];
        }

        return null;
    }
}
