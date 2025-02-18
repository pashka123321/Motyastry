using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

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

    private BuildModeController buildModeController;
    public Text shootingModeText; // UI элемент для отображения статуса стрельбы
    private PlayerHealth playerHealth; // Ссылка на PlayerHealth

    public List<GameObject> ignoreUIElements = new List<GameObject>();

    private float shootingStartTime = 0f; // Время первого нажатия на кнопку стрельбы
    private float shootingDelay = 0.2f; // Задержка перед началом стрельбы (1 секунда)

    public float recoilAmount = 0.1f;
    public float recoilSpeed = 5f;

    private Vector3 firstGunInitialPos;
    private Vector3 secondGunInitialPos;

    private PlayerSpeedDisplay playerSpeedDisplay;

    private тряска_шейдер shakeShader;

    void Start()
    {
        buildModeController = FindObjectOfType<BuildModeController>();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        playerHealth = GetComponent<PlayerHealth>();

        firstGunInitialPos = firstGun.localPosition;
        secondGunInitialPos = secondGun.localPosition;

        playerSpeedDisplay = FindObjectOfType<PlayerSpeedDisplay>();

        shakeShader = Camera.main.GetComponent<тряска_шейдер>();

        if (shootingModeText != null)
        {
            shootingModeText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (PauseController.IsGamePaused)
        {
            return;
        }

        if (buildModeController != null && buildModeController.IsBuildModeActive)
        {
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
            Vector2 playerVelocity = playerSpeedDisplay.GetPlayerVelocity();
            float playerVelocityInfluence = 0.5f; // Коэффициент влияния скорости игрока
            rb.velocity = bullet.transform.up * bulletSpeed + new Vector3(playerVelocity.x * playerVelocityInfluence, playerVelocity.y * playerVelocityInfluence, 0);
        }
        else
        {
            Debug.LogWarning("Отсутствует компонент Rigidbody2D у пули.");
        }

        Bullet bulletScript = bullet.AddComponent<Bullet>();
        bulletScript.damage = 20;
        bulletScript.damageType = this.damageType;

        if (shakeShader != null)
        {
            shakeShader.ApplyShakeEffect();
        }

        Destroy(bullet, bulletLifeTime);
        PlayShootingSound();
    }

    private void PlayShootingSound()
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

    private void UpdateShootingModeText()
    {
        if (shootingModeText != null)
        {
            shootingModeText.gameObject.SetActive(!shootingModeEnabled);
            shootingModeText.text = shootingModeEnabled ? "" : "Стрельба отключена";
        }
    }

    private bool IsPointerOverUI()
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