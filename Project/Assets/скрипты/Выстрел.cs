using UnityEngine;
using System.Collections.Generic;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject secondBulletPrefab;

    public Transform firstGun; // Трансформ первого ствола
    public Transform secondGun; // Трансформ второго ствола

    [Header("Точки спауна пуль")]
    public List<Transform> firstGunBulletSpawnPoints; // Точки спауна для первого ствола
    public List<Transform> secondGunBulletSpawnPoints; // Точки спауна для второго ствола

    [Range(0, 10)] public int damageType;
    public float bulletSpeed = 10f;
    public int bulletCount = 1;
    public float bulletSpacing = 0.2f;
    public float fireRate = 0.1f;
    public float bulletLifeTime = 2f;
    public AudioClip shootingSound;
    private AudioSource audioSource;
    private float nextFireTime = 0f;
    private bool alternateShoot = false;
    private bool shootingModeEnabled = true;
    private bool isShooting = false;
    private bool wasShootingInitially = false;
    private float shootingStartTime = 0f;
    private float shootingDelay = 0.2f;

    public float recoilAmount = 0.1f;
    public float recoilSpeed = 5f;

    private Vector3 firstGunInitialPos;
    private Vector3 secondGunInitialPos;

    private PlayerHealth playerHealth; // Ссылка на PlayerHealth

    private void Start()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        // Запоминаем исходные позиции стволов для анимации отдачи
        firstGunInitialPos = firstGun.localPosition;
        secondGunInitialPos = secondGun.localPosition;

        // Получаем ссылку на PlayerHealth
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        // Проверка, жив ли игрок
        if (playerHealth != null && !playerHealth.isAlive)
        {
            return; // Блокируем стрельбу, если игрок мертв
        }

        if (shootingModeEnabled && Time.time > nextFireTime)
        {
            if (Input.GetButton("Fire1"))
            {
                if (!wasShootingInitially)
                {
                    shootingStartTime = Time.time;
                    wasShootingInitially = true;
                }

                if (Time.time - shootingStartTime >= shootingDelay)
                {
                    isShooting = true;
                }
            }
            else
            {
                isShooting = false;
                wasShootingInitially = false;
            }

            if (isShooting)
            {
                nextFireTime = Time.time + fireRate;

                if (alternateShoot)
                {
                    ShootFromSecondGun();
                    ApplyRecoil(secondGun, secondGunInitialPos);
                }
                else
                {
                    ShootFromFirstGun();
                    ApplyRecoil(firstGun, firstGunInitialPos);
                }

                alternateShoot = !alternateShoot;
            }
        }

        // Возвращаем стволы в исходное положение
        firstGun.localPosition = Vector3.Lerp(firstGun.localPosition, firstGunInitialPos, recoilSpeed * Time.deltaTime);
        secondGun.localPosition = Vector3.Lerp(secondGun.localPosition, secondGunInitialPos, recoilSpeed * Time.deltaTime);
    }

    private void ApplyRecoil(Transform gun, Vector3 initialPosition)
    {
        float recoilDirection = transform.localScale.x > 0 ? -1 : 1;
        gun.localPosition = initialPosition + new Vector3(recoilAmount * recoilDirection, 0, 0);
    }

    private void ShootFromFirstGun()
    {
        foreach (var spawnPoint in firstGunBulletSpawnPoints)
        {
            CreateBullet(bulletPrefab, spawnPoint.position);
        }
    }

    private void ShootFromSecondGun()
    {
        foreach (var spawnPoint in secondGunBulletSpawnPoints)
        {
            CreateBullet(secondBulletPrefab, spawnPoint.position);
        }
    }

    private void CreateBullet(GameObject bulletPrefab, Vector3 spawnPosition)
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, transform.rotation);
        bullet.transform.Rotate(0, 0, -90);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = bullet.transform.up * bulletSpeed;
        }
        
        // Добавляем компонент Bullet и задаем его параметры
        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.damage = 20; // Устанавливаем урон пули
        bulletScript.damageType = this.damageType; // Устанавливаем тип урона

        Destroy(bullet, bulletLifeTime);
        PlayShootingSound();
    }

    void PlayShootingSound()
    {
        if (shootingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootingSound);
        }
        else
        {
            Debug.LogWarning("Отсутствует аудиоклип для выстрела или AudioSource не инициализирован.");
        }
    }
}

public class Bullet : MonoBehaviour
{
    private Vector3 startPosition;
    public float maxDistance = 100f;
    public int damage = 20; // Урон, который наносит пуля

    public int damageType;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Проверяем, прошло ли пуля более maxDistance единиц
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, столкнулась ли пуля с врагом
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, damageType); // Наносим урон врагу
            Destroy(gameObject); // Уничтожаем пулю
        }
    }
}