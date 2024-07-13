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
    public AudioClip[] shootSounds; // Массив звуков выстрела
    private float fireCountdown = 0f;
    private AudioSource audioSource;

    private GameObject currentTarget; // Текущая цель турели

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (currentTarget == null)
        {
            currentTarget = FindRandomEnemyWithinRadius();
        }
        else
        {
            Vector3 direction = currentTarget.transform.position - turretObject.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
            turretObject.transform.rotation = Quaternion.RotateTowards(turretObject.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }

            fireCountdown -= Time.deltaTime;

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
        }

        if (shootSounds.Length > 0 && audioSource != null)
        {
            AudioClip randomShootSound = shootSounds[Random.Range(0, shootSounds.Length)];
            audioSource.PlayOneShot(randomShootSound);
        }
    }
}
