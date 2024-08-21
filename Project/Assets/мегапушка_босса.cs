using UnityEngine;

public class Shooting : MonoBehaviour
{
    // Префаб пули
    public GameObject bulletPrefab;

    // Две точки выстрела
    public Transform firePoint1;
    public Transform firePoint2;

    // Скорость пули
    public float bulletSpeed = 20f;

    // Радиус для поиска цели
    public float detectionRadius = 20f;

    // Интервал между выстрелами
    public float fireRate = 1f; // Один выстрел в секунду

    private float fireCooldown = 0f; // Время до следующего выстрела

    // Индекс текущего дула (0 - первое дуло, 1 - второе дуло)
    private int currentFirePointIndex = 0;

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
        // Определяем текущее дуло
        Transform currentFirePoint = currentFirePointIndex == 0 ? firePoint1 : firePoint2;

        // Создаем пулю в текущей точке огня
        GameObject bullet = Instantiate(bulletPrefab, currentFirePoint.position, currentFirePoint.rotation);

        // Получаем компонент Rigidbody2D пули
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        // Рассчитываем направление полета пули
        Vector2 direction = (target.transform.position - currentFirePoint.position).normalized;

        // Задаем скорость пули в направлении цели
        rb.velocity = direction * bulletSpeed;

        // Переключаемся на следующее дуло для следующего выстрела
        currentFirePointIndex = (currentFirePointIndex + 1) % 2;
    }
}
