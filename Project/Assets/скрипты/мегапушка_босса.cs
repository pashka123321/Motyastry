using UnityEngine;

public class Shooting : MonoBehaviour
{
    // Префаб пули
    public GameObject bulletPrefab;

    // Префаб эффекта огня
    public GameObject muzzleFlashPrefab;

    // Две точки выстрела
    public Transform firePoint1;
    public Transform firePoint2;

    // Точки для эффекта огня
    public Transform muzzleFlashPoint1;
    public Transform muzzleFlashPoint2;

    // Скорость пули
    public float bulletSpeed = 20f;

    // Радиус для поиска цели
    public float detectionRadius = 20f;

    // Интервал между выстрелами
    public float fireRate = 1f; // Один выстрел в секунду

    // Смещение для имитации отдачи
    public Vector2 recoilOffset = new Vector2(-0.1f, 0f);

    // Время для отдачи и восстановления
    public float recoilTime = 0.05f;
    public float recoveryTime = 0.3f;

    // Звук выстрела
    public AudioClip shootSound;

    private AudioSource audioSource;

    private float fireCooldown = 0f; // Время до следующего выстрела

    // Индекс текущего дула (0 - первое дуло, 1 - второе дуло)
    private int currentFirePointIndex = 0;

    private bool isRecoiling = false;

    void Start()
    {
        // Инициализируем AudioSource компонент
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = shootSound;

        // Устанавливаем громкость на 50%
        audioSource.volume = 0.5f;
    }

    void Update()
    {
        // Ищем цель с тегом "Player"
        GameObject target = FindTarget();

        if (target != null)
        {
            // Проверяем время до следующего выстрела
            if (fireCooldown <= 0f)
            {
                // Стреляем по очереди из каждого дула
                ShootAtTarget(target);

                // Сбрасываем кулдаун
                fireCooldown = 1f / fireRate;

                // Запускаем корутину для имитации отдачи
                StartCoroutine(ApplyRecoil());

                // Проигрываем звук выстрела
                audioSource.Play();
            }
        }

        // Уменьшаем кулдаун по времени
        fireCooldown -= Time.deltaTime;
    }

    GameObject FindTarget()
    {
        // Находим все объекты с тегом "Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Проходим через всех игроков и проверяем их расстояние до этого объекта
        foreach (GameObject player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            // Если игрок находится в радиусе обнаружения, возвращаем его как цель
            if (distance <= detectionRadius)
            {
                return player;
            }
        }

        // Если целей нет, возвращаем null
        return null;
    }

    void ShootAtTarget(GameObject target)
    {
        // Определяем текущее дуло и точку для огня
        Transform currentFirePoint = currentFirePointIndex == 0 ? firePoint1 : firePoint2;
        Transform currentMuzzleFlashPoint = currentFirePointIndex == 0 ? muzzleFlashPoint1 : muzzleFlashPoint2;

        // Создаем пулю в текущей точке огня
        GameObject bullet = Instantiate(bulletPrefab, currentFirePoint.position, currentFirePoint.rotation);

        // Задаем скорость пули в направлении цели
        Vector2 direction = (target.transform.position - currentFirePoint.position).normalized;
        bullet.transform.Translate(direction * bulletSpeed * Time.deltaTime, Space.World);

        // Создаем эффект огня
        CreateMuzzleFlash(currentMuzzleFlashPoint);

        // Переключаемся на следующее дуло для следующего выстрела
        currentFirePointIndex = (currentFirePointIndex + 1) % 2;
    }

    void CreateMuzzleFlash(Transform muzzleFlashPoint)
    {
        // Создаем префаб огня в точке выстрела
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzleFlashPoint.position, muzzleFlashPoint.rotation);

        // Опционально можно настроить масштаб и другие параметры эффекта
        muzzleFlash.transform.SetParent(muzzleFlashPoint);

        // Уничтожаем объект эффекта через время жизни эффекта
        Destroy(muzzleFlash, 0.5f); // Время жизни эффекта (полсекунды)
    }

    System.Collections.IEnumerator ApplyRecoil()
    {
        if (isRecoiling) yield break;

        isRecoiling = true;

        // Исходное положение
        Vector3 originalPosition = transform.localPosition;

        // Учитываем поворот объекта для расчета смещения
        Vector3 recoilDirection = transform.TransformDirection(recoilOffset);

        // Целевая позиция для отдачи
        Vector3 recoilTargetPosition = originalPosition + recoilDirection;

        float elapsedTime = 0f;

        // Быстрое движение к позиции отдачи
        while (elapsedTime < recoilTime)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, recoilTargetPosition, (elapsedTime / recoilTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Обеспечиваем, чтобы модель оказалась в позиции отдачи точно
        transform.localPosition = recoilTargetPosition;

        elapsedTime = 0f;

        // Медленное движение назад к исходной позиции
        while (elapsedTime < recoveryTime)
        {
            transform.localPosition = Vector3.Lerp(recoilTargetPosition, originalPosition, (elapsedTime / recoveryTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        isRecoiling = false;
    }
}
