using UnityEngine;

public class BulletSpider : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifespan = 2f; // Время жизни пули, после которого она уничтожается
    [SerializeField] private float minRotationSpeed = 100f; // Минимальная скорость вращения
    [SerializeField] private float maxRotationSpeed = 300f; // Максимальная скорость вращения
    [SerializeField] private float bulletSpeed = 20f; // Скорость движения пули

    private float rotationSpeed; // Скорость вращения
    private int rotationDirection; // Направление вращения (1 = по часовой, -1 = против часовой)

    private void Start()
    {
        // Уничтожить пулю через заданное время
        Destroy(gameObject, lifespan);

        // Инициализируем скорость вращения и направление
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        rotationDirection = Random.Range(0, 2) * 2 - 1;

        // Устанавливаем начальную скорость пули
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = transform.up * bulletSpeed;
        }
    }

    private void Update()
    {
        // Вращаем пулю вокруг своей оси Z
        transform.Rotate(Vector3.forward * rotationDirection * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // Уничтожить пулю после попадания
        }
        else if (collision.CompareTag("Block"))
        {
            BlockHealth blockHealth = collision.GetComponent<BlockHealth>();
            if (blockHealth != null)
            {
                blockHealth.TakeDamage(damage);
            }
            // Пуля не уничтожается сразу при попадании в Block, поэтому удаляем эту строку
        }
        else if (collision.CompareTag("Conveer"))
        {
            BlockHealth blockHealth = collision.GetComponent<BlockHealth>();
            if (blockHealth != null)
            {
                blockHealth.TakeDamage(damage);
            }
            // Пуля не уничтожается сразу при попадании в Conveer, поэтому удаляем эту строку
        }
        else if (collision.CompareTag("Blockwall"))
        {
            Destroy(gameObject); // Уничтожить пулю при попадании в Blockwall
        }
    }
}
