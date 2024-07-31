using UnityEngine;
using System.Collections.Generic;

public class TurretController : MonoBehaviour
{
    public GameObject turretObject; // Префаб или объект, к которому нужно поворачивать турель
    public GameObject Bullet1; // Префаб пули
    public Transform firePoint; // Точка, откуда вылетают пули
    public float fireRate = 1f; // Скорость стрельбы (выстрелов в секунду)
    public float detectionRadius = 20f; // Радиус обнаружения врагов
    public float rotationSpeed = 5f; // Скорость поворота турели к врагу
    public float aimThreshold = 5f; // Порог для проверки, нацелена ли турель
    public AudioClip[] shootSounds; // Массив звуков выстрела
    public int leadIngotCount = 10; // Количество доступных слитков свинца
    public int maxLeadIngots = 50; // Максимальное количество слитков свинца, которое может храниться
    public int ammoRefillAmount = 10; // Количество слитков свинца, добавляемое при пополнении
    public string ammoTag = "слиток свинца"; // Тег для объектов пополнения боеприпасов
    public bool requiresAmmo = true; // Флаг, указывающий, требуется ли для стрельбы боеприпасы

    private float fireCountdown = 0f;
    private AudioSource audioSource;
    private GameObject currentTarget; // Текущая цель турели

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
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

            float angle = Quaternion.Angle(turretObject.transform.rotation, targetRotation);
            if (angle < aimThreshold)
            {
                if (fireCountdown <= 0f)
                {
                    if (CanShoot()) // Проверяем возможность стрельбы
                    {
                        Shoot();
                        fireCountdown = 1f / fireRate;
                    }
                    else
                    {
                        Debug.Log("No lead ingots left!");
                    }
                }
                fireCountdown -= Time.deltaTime;
            }

            // Проверяем, жива ли текущая цель
            if (!IsTargetAlive(currentTarget))
            {
                currentTarget = null; // Сбрасываем текущую цель, чтобы найти новую на следующем кадре
            }
        }
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

    void Shoot()
    {
        if (Bullet1 != null && firePoint != null)
        {
            GameObject bullet = Instantiate(Bullet1, firePoint.position, firePoint.rotation);
            Bullet1 bulletScript = bullet.GetComponent<Bullet1>();
            if (bulletScript != null)
            {
                bulletScript.SetDirection(firePoint.up); // Предполагаем, что направление пули - вверх от точки выстрела firePoint
            }

            if (requiresAmmo)
            {
                leadIngotCount--; // Уменьшаем количество слитков свинца после каждого выстрела
            }
        }

        if (shootSounds.Length > 0 && audioSource != null)
        {
            AudioClip randomShootSound = shootSounds[Random.Range(0, shootSounds.Length)];
            audioSource.PlayOneShot(randomShootSound, 0.3f); // Устанавливаем громкость на 30%
        }
    }

    bool CanShoot()
    {
        return !requiresAmmo || leadIngotCount > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ammoTag))
        {
            RefillAmmo();
            Destroy(other.gameObject); // Уничтожаем объект "Ammo" после пополнения
        }
    }

    void RefillAmmo()
    {
        leadIngotCount = Mathf.Min(leadIngotCount + ammoRefillAmount, maxLeadIngots);
        Debug.Log("Ammo refilled! Current lead ingots: " + leadIngotCount);
    }
}
