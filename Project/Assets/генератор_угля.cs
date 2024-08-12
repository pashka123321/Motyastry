using UnityEngine;

public class CoalGenerator : MonoBehaviour
{
    public float energyPerSecond = 5f; // Сколько энергии генерируется в секунду
    public float generationRadius = 15f; // Радиус передачи энергии
    public LayerMask batteryLayer; // Слой, на котором находятся батареи

    public int maxCoalAmount = 10; // Максимальное количество угля, которое может храниться
    public int currentCoalAmount = 0; // Текущее количество угля

    private void Update()
    {
        if (currentCoalAmount > 0)
        {
            GenerateEnergy();
            ConsumeCoal(); // Уменьшение угля по мере генерации энергии
        }
    }

    void GenerateEnergy()
    {
        Collider2D[] batteriesInRange = Physics2D.OverlapCircleAll(transform.position, generationRadius, batteryLayer);

        foreach (Collider2D batteryCollider in batteriesInRange)
        {
            Battery battery = batteryCollider.GetComponent<Battery>();
            if (battery != null)
            {
                battery.StoreEnergy(energyPerSecond * Time.deltaTime);
            }
        }
    }

    void ConsumeCoal()
    {
        // Предполагаем, что 1 единица угля обеспечивает генерацию энергии на 1 секунду
        // Следовательно, в 1 кадре потребляется определенная доля угля
        currentCoalAmount -= Mathf.FloorToInt(energyPerSecond * Time.deltaTime);
        if (currentCoalAmount < 0)
        {
            currentCoalAmount = 0;
        }
    }

    public void AddCoal(int amount)
    {
        currentCoalAmount += amount;
        if (currentCoalAmount > maxCoalAmount)
        {
            currentCoalAmount = maxCoalAmount; // Ограничение по максимальному количеству угля
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, generationRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("угольная руда")) // Проверяем, является ли объект углем
        {
            AddCoal(1); // Добавляем 1 единицу угля
            Destroy(collision.gameObject); // Уничтожаем префаб угля после загрузки
        }
    }
}
