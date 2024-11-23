using UnityEngine;
using System.Collections;

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

[SerializeField] private float recoilForce = 0.5f; // Сила отдачи
[SerializeField] private float recoilSpeed = 10f; // Скорость движения при отдаче
[SerializeField] private float recoverySpeed = 5f; // Скорость возвращения после отдачи

private Vector3 originalLocalPosition; // Исходное локальное положение пушки
private bool isRecoiling; // Флаг выполнения отдачи





    private float lastShotTime;

private void Start()
{
    core = GameObject.FindWithTag(coreTag)?.transform;
    originalLocalPosition = gunTransform.localPosition; // Сохраняем начальное локальное положение пушки
}





    private void Update()
    {
        bool isPlayerInRange = IsPlayerInShootingRange();
        bool isCoreInRange = IsCoreInRange();
        bool isBlockwallInRange = IsBlockwallInRange();

        if (playerNearby && isPlayerInRange)
        {
            RotateTowardsTarget(playerTransform.position);
            TryShootAtPlayer();
        }
        else if (isCoreInRange && core != null)
        {
            RotateTowardsTarget(core.position);
            TryShootAtCore();
        }
        else if (isBlockwallInRange)
        {
            Transform closestBlockwall = FindClosestBlockwall();
            if (closestBlockwall != null)
            {
                RotateTowardsTarget(closestBlockwall.position);
                TryShootAtBlockwall();
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

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - gunTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        gunTransform.rotation = Quaternion.RotateTowards(gunTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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

    private void TryShootAtPlayer()
    {
        if (Time.time - lastShotTime >= fireRate)
        {
            Shoot();
            lastShotTime = Time.time;
        }
    }

    private void TryShootAtBlockwall()
    {
        Transform closestBlockwall = FindClosestBlockwall();
        if (closestBlockwall != null && Vector3.Distance(gunTransform.position, closestBlockwall.position) <= blockwallShootingRange)
        {
            if (Time.time - lastShotTime >= fireRate)
            {
                Shoot();
                lastShotTime = Time.time;
            }
        }
    }

    private void TryShootAtCore()
    {
        if (core != null && Vector3.Distance(gunTransform.position, core.position) <= coreShootingRange)
        {
            if (Time.time - lastShotTime >= fireRate)
            {
                Shoot();
                lastShotTime = Time.time;
            }
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
