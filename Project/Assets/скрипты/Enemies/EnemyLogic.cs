using UnityEngine;
using System.Collections;
using System.Linq;

public class EnemyLogic : MonoBehaviour
{
    [SerializeField] private Transform gunTransform;
    public Transform playerTransform;
    [SerializeField] private bool playerNearby;
    private Transform core;

    [SerializeField] private float rotationSpeed = 5f; // скорость поворота
    [SerializeField] private float shootingRange = 10f; // дальность стрельбы
    [SerializeField] private float fireRate = 1f; // частота стрельбы
    [SerializeField] private GameObject bulletPrefab; // префаб пули
    [SerializeField] private Transform bulletSpawnPoint; // точка, где рождается пуля

    [SerializeField] private float blockwallDetectionRange = 10f; // Дальность обнаружения блоков
    [SerializeField] private float blockwallShootingRange = 5f; // Дальность стрельбы по блокам
    [SerializeField] private string blockwallTag = "Blockwall"; // Тег блоков
    [SerializeField] private float coreDetectionRange = 10f; // Дальность обнаружения ядра
    [SerializeField] private float coreShootingRange = 5f; // Дальность стрельбы по ядру
    [SerializeField] private string coreTag = "core"; // Тег ядра
    [SerializeField] private string blockTag = "Block"; // Тег блоков
    [SerializeField] private string enemyTag = "Enemy"; // Тег врагов

    public bool isFriendly; // Флаг, является ли враг дружелюбным

    [SerializeField] private float recoilForce = 0.5f; // Сила отдачи
    [SerializeField] private float recoilSpeed = 10f; // Скорость движения при отдаче
    [SerializeField] private float recoverySpeed = 5f; // Скорость возвращения после отдачи

    [SerializeField] private GameObject[] ignorePrefabs; // Массив префабов объектов, по которым не нужно стрелять

    private Vector3 originalLocalPosition; // Исходное локальное положение пушки
    private bool isRecoiling; // Флаг выполнения отдачи

    private float lastShotTime;

    [SerializeField] private AudioClip shootingSound; // Звук выстрела
    private AudioSource audioSource; // Аудио источник

    private void Start()
    {
        core = GameObject.FindWithTag(coreTag)?.transform;
        originalLocalPosition = gunTransform.localPosition; // Сохраняем начальное локальное положение пушки
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        bool isPlayerInRange = IsPlayerInShootingRange();
        bool isCoreInRange = IsCoreInRange();
        bool isBlockwallInRange = IsBlockwallInRange();
        bool isBlockInRange = IsBlockInRange();
        bool isEnemyInRange = IsEnemyInRange();

        if (isFriendly)
        {
            if (isEnemyInRange)
            {
                Transform closestEnemy = FindClosestEnemy();
                if (closestEnemy != null)
                {
                    RotateTowardsTarget(closestEnemy.position);
                    if (IsAimedAtTarget(closestEnemy.position))
                    {
                        TryShoot();
                    }
                }
            }
        }
        else
        {
            if (playerNearby && isPlayerInRange)
            {
                RotateTowardsTarget(playerTransform.position);
                if (IsAimedAtTarget(playerTransform.position))
                {
                    TryShoot();
                }
            }
            else if (isCoreInRange && core != null)
            {
                RotateTowardsTarget(core.position);
                if (IsAimedAtTarget(core.position))
                {
                    TryShoot();
                }
            }
            else if (isBlockwallInRange)
            {
                Transform closestBlockwall = FindClosestBlockwall();
                if (closestBlockwall != null)
                {
                    RotateTowardsTarget(closestBlockwall.position);
                    if (IsAimedAtTarget(closestBlockwall.position))
                    {
                        TryShoot();
                    }
                }
            }
            else if (isBlockInRange)
            {
                Transform closestBlock = FindClosestBlock();
                if (closestBlock != null)
                {
                    RotateTowardsTarget(closestBlock.position);
                    if (IsAimedAtTarget(closestBlock.position))
                    {
                        TryShoot();
                    }
                }
            }
            else
            {
                if (core != null)
                {
                    RotateTowardsTarget(core.position);
                }
            }
        }
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - gunTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        gunTransform.rotation = Quaternion.RotateTowards(gunTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private bool IsAimedAtTarget(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - gunTransform.position;
        float angleToTarget = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(gunTransform.eulerAngles.z, angleToTarget));
        return angleDifference < 5f; // Допустимая погрешность в 5 градусов
    }

    private bool IsPlayerInShootingRange()
    {
        return Vector3.Distance(gunTransform.position, playerTransform.position) <= shootingRange;
    }

    private bool IsCoreInRange()
    {
        if (core == null)
        {
            return false;
        }
        return Vector3.Distance(gunTransform.position, core.position) <= coreDetectionRange;
    }

    private bool IsBlockwallInRange()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(gunTransform.position, blockwallDetectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(blockwallTag))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsBlockInRange()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(gunTransform.position, blockwallDetectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(blockTag) && !IsIgnoredObject(hitCollider.gameObject))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsEnemyInRange()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(gunTransform.position, blockwallDetectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(enemyTag))
            {
                return true;
            }
        }
        return false;
    }

    private Transform FindClosestBlockwall()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(gunTransform.position, blockwallDetectionRange);
        Transform closestBlockwall = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(blockwallTag))
            {
                float distance = Vector3.Distance(gunTransform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBlockwall = hitCollider.transform;
                }
            }
        }
        return closestBlockwall;
    }

    private Transform FindClosestBlock()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(gunTransform.position, blockwallDetectionRange);
        Transform closestBlock = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(blockTag) && !IsIgnoredObject(hitCollider.gameObject))
            {
                float distance = Vector3.Distance(gunTransform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBlock = hitCollider.transform;
                }
            }
        }
        return closestBlock;
    }

    private Transform FindClosestEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(gunTransform.position, blockwallDetectionRange);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(enemyTag))
            {
                float distance = Vector3.Distance(gunTransform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hitCollider.transform;
                }
            }
        }
        return closestEnemy;
    }

    private void TryShoot()
    {
        if (Time.time - lastShotTime >= fireRate)
        {
            Shoot();
            lastShotTime = Time.time;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, gunTransform.rotation * Quaternion.Euler(0, 0, -90));
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = bullet.transform.up * 15f; // Скорость пули
            }

            if (shootingSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootingSound);
            }

            StartCoroutine(Recoil()); // Запуск эффекта отдачи
        }
    }

    private IEnumerator Recoil()
    {
        if (isRecoiling) yield break; // Предотвращаем множественные вызовы
        isRecoiling = true;

        // Считаем направление отдачи в глобальных координатах
        Vector3 recoilDirection = -gunTransform.up.normalized * recoilForce;

        // Поворачиваем отдачу на 90 градусов
        recoilDirection = Quaternion.Euler(0, 0, -90) * recoilDirection;  // Поворот на 90 градусов относительно направления прицеливания

        // Переводим это смещение в локальные координаты относительно родителя (обычно корпуса)
        Vector3 recoilTargetLocal = gunTransform.localPosition + gunTransform.parent.InverseTransformDirection(recoilDirection);

        // Сдвигаем пушку назад
        while (Vector3.Distance(gunTransform.localPosition, recoilTargetLocal) > 0.01f)
        {
            gunTransform.localPosition = Vector3.MoveTowards(gunTransform.localPosition, recoilTargetLocal, recoilSpeed * Time.deltaTime);
            yield return null;
        }

        // Возвращаем пушку в исходное локальное положение
        while (Vector3.Distance(gunTransform.localPosition, originalLocalPosition) > 0.01f)
        {
            gunTransform.localPosition = Vector3.MoveTowards(gunTransform.localPosition, originalLocalPosition, recoverySpeed * Time.deltaTime);
            yield return null;
        }

        isRecoiling = false; // Завершение эффекта отдачи
    }

    private bool IsIgnoredObject(GameObject obj)
    {
        return ignorePrefabs.Any(prefab => prefab.name == obj.name.Replace("(Clone)", "").Trim());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}
