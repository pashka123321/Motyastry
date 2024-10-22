using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror;
using System.Collections.Generic;

public class PlayerShootingm : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public GameObject secondBulletPrefab;
    public float bulletSpeed = 10f;
    public int bulletCount = 1;
    public float bulletSpacing = 0.2f;
    public float fireRate = 0.1f;
    public float bulletLifeTime = 2f;
    public Vector2 firstBulletSpawnOffset = new Vector2(-0.1f, 0.1f);
    public Vector2 secondBulletSpawnOffset = new Vector2(0.1f, 0.1f);

    public AudioClip shootingSound;
    private AudioSource audioSource;
    private float nextFireTime = 0f;
    private bool alternateShoot = false;
    private bool shootingModeEnabled = true;
    private bool isShooting = false;
    private bool wasShootingInitially = false;

    private BuildModeController buildModeController; // Reference to build mode controller

    public Text shootingModeText; // Reference to the UI Text component

    private PlayerHealth playerHealth; // Reference to PlayerHealth script

    // List of UI elements to ignore for pointer detection
    public List<GameObject> ignoreUIElements = new List<GameObject>();

    private float shootingStartTime = 0f; // Time when the shooting button was first pressed
    private float shootingDelay = 0.2f; // Delay before shooting starts (1 second)

    void Start()
    {
        buildModeController = FindObjectOfType<BuildModeController>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        playerHealth = GetComponent<PlayerHealth>(); // Get reference to PlayerHealth script

        // Initially hide the shooting mode text
        if (shootingModeText != null)
        {
            shootingModeText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return; // Проверка, является ли объект локальным игроком

        if (PauseController.IsGamePaused)
        {
            // Game is paused, do not shoot
            return;
        }

        if (buildModeController != null && buildModeController.IsBuildModeActive)
        {
            // Build mode active, do not shoot
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            shootingModeEnabled = !shootingModeEnabled;
            UpdateShootingModeText();
        }

        if (shootingModeEnabled && Time.time > nextFireTime && playerHealth.isAlive)
        {
            bool isPointerOverUI = IsPointerOverUI();
            bool fireButtonPressed = Input.GetButton("Fire1");

            if (fireButtonPressed && !isPointerOverUI)
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

                Vector3 spawnPosition = alternateShoot ? transform.position + transform.TransformDirection(secondBulletSpawnOffset) : transform.position + transform.TransformDirection(firstBulletSpawnOffset);
                Quaternion rotation = transform.rotation;

                // Локальное создание пули для игрока
                CreateLocalBullet(spawnPosition, rotation, alternateShoot);

                // Серверное создание пули для остальных
                CmdShoot(spawnPosition, rotation, alternateShoot);

                alternateShoot = !alternateShoot;
            }
        }
    }

    void CreateLocalBullet(Vector3 spawnPosition, Quaternion rotation, bool isSecondGun)
    {
        // Создаем пулю локально
        GameObject bullet = Instantiate(isSecondGun ? secondBulletPrefab : bulletPrefab, spawnPosition, rotation);
        bullet.transform.Rotate(0, 0, -90);
        bullet.transform.position = new Vector3(bullet.transform.position.x, bullet.transform.position.y, 1);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = bullet.transform.up * bulletSpeed;
        }
        bullet.AddComponent<Bullet>().maxDistance = 100f;
        Destroy(bullet, bulletLifeTime);

        // Проигрываем звук выстрела
        PlayShootingSound();
    }

    [Command]
    void CmdShoot(Vector3 spawnPosition, Quaternion rotation, bool isSecondGun)
    {
        // Создаем пулю на сервере
        GameObject bullet = Instantiate(isSecondGun ? secondBulletPrefab : bulletPrefab, spawnPosition, rotation);
        bullet.transform.Rotate(0, 0, -90);
        bullet.transform.position = new Vector3(bullet.transform.position.x, bullet.transform.position.y, 1);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = bullet.transform.up * bulletSpeed;
        }
        bullet.AddComponent<Bullet>().maxDistance = 100f;
        Destroy(bullet, bulletLifeTime);

        // Синхронизируем выстрел со всеми клиентами
        RpcShoot(spawnPosition, rotation, isSecondGun);
    }

    [ClientRpc]
    void RpcShoot(Vector3 spawnPosition, Quaternion rotation, bool isSecondGun)
    {
        if (isLocalPlayer) return;

        // Создаем пулю на клиенте
        GameObject bullet = Instantiate(isSecondGun ? secondBulletPrefab : bulletPrefab, spawnPosition, rotation);
        bullet.transform.Rotate(0, 0, -90);
        bullet.transform.position = new Vector3(bullet.transform.position.x, bullet.transform.position.y, 1);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = bullet.transform.up * bulletSpeed;
        }
        bullet.AddComponent<Bullet>().maxDistance = 100f;
        Destroy(bullet, bulletLifeTime);

        // Проигрываем звук выстрела на клиенте
        PlayShootingSound();
    }

    void PlayShootingSound()
    {
        if (shootingSound != null)
        {
            audioSource.PlayOneShot(shootingSound);
        }
    }

    void UpdateShootingModeText()
    {
        if (shootingModeText != null)
        {
            shootingModeText.gameObject.SetActive(!shootingModeEnabled);
            shootingModeText.text = shootingModeEnabled ? "" : "Стрельба отключена";
        }
    }

    bool IsPointerOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (!ignoreUIElements.Contains(result.gameObject))
            {
                return true;
            }
        }

        return false;
    }
}

public class Bulletm : MonoBehaviour
{
    private Vector3 startPosition;
    public float maxDistance = 100f;
    public int damage = 20; // Урон, который наносит пуля

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
            enemy.TakeDamage(damage); // Наносим урон врагу
            Destroy(gameObject); // Уничтожаем пулю
        }
    }
}
