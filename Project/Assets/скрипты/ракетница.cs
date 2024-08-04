using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    public GameObject rocketPrefab;  // Префаб ракеты
    public Transform launchPoint;    // Точка запуска ракеты
    public float rocketSpeed = 10f;  // Скорость полета ракеты
    public float fireRate = 0.5f;    // Минимальное время между запусками ракет

    private float lastFireTime;      // Время последнего запуска ракеты

    void Update()
    {
        RotateTowardsMouse();

        // Проверяем, зажата ли кнопка и прошло ли достаточно времени с последнего запуска
        if (Input.GetButton("Fire1") && Time.time >= lastFireTime + fireRate)
        {
            LaunchRocket();
        }
    }

    void RotateTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // Поворачиваем ракетницу на 90 градусов
    }

    void LaunchRocket()
    {
        lastFireTime = Time.time;  // Обновляем время последнего запуска

        // Проверяем, что префаб ракеты назначен
        if (rocketPrefab == null)
        {
            Debug.LogError("Rocket Prefab is not assigned!");
            return;
        }

        // Проверяем, что точка запуска назначена
        if (launchPoint == null)
        {
            Debug.LogError("Launch Point is not assigned!");
            return;
        }

        // Создаем ракету и настраиваем её
        GameObject rocket = Instantiate(rocketPrefab, launchPoint.position, launchPoint.rotation);
        rocket.transform.rotation = Quaternion.Euler(new Vector3(0, 0, launchPoint.rotation.eulerAngles.z - 90)); // Поворачиваем ракету на 90 градусов

        // Настраиваем физику ракеты
        Rigidbody2D rb = rocket.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = rocket.transform.right * rocketSpeed;
        }
        else
        {
            Debug.LogError("Rigidbody2D component not found on rocket prefab!");
        }
    }
}
