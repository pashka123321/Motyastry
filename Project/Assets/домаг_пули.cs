using UnityEngine;

public class Bullettank : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifespan = 2f; // Время жизни пули, после которого она уничтожается

    private void Start()
    {
        Destroy(gameObject, lifespan); // Уничтожить пулю через заданное время
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
            Destroy(gameObject); // Уничтожить пулю после попадания
        }
    }
}
