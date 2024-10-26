using UnityEngine;

public class WindTurbine : MonoBehaviour
{
    public float baseEnergyPerSecond = 10f; // Базовая энергия, производимая ветровым генератором при максимальной скорости ветра
    public float windSpeed = 5f; // Текущая скорость ветра
    public float generationRadius = 15f; // Радиус передачи энергии
    public LayerMask batteryLayer; // Слой, на котором находятся батареи

    private float currentEnergy = 0f; // Энергия, накопленная ветровым генератором

    private void Update()
    {
        GenerateEnergy();
    }

    void GenerateEnergy()
    {
        // Вычисление количества энергии в зависимости от скорости ветра
        float energyProduced = baseEnergyPerSecond * windSpeed;

        // Получаем все объекты с компонентом Battery в заданном радиусе
        Collider2D[] batteriesInRange = Physics2D.OverlapCircleAll(transform.position, generationRadius, batteryLayer);

        foreach (Collider2D batteryCollider in batteriesInRange)
        {
            Battery battery = batteryCollider.GetComponent<Battery>();
            if (battery != null)
            {
                battery.StoreEnergy(energyProduced * Time.deltaTime); // Передача энергии в батарею
            }
        }
    }

    // Метод для приема энергии от других источников, например от Tesla Coil
    public void ReceiveEnergy(float amount)
    {
        currentEnergy += amount;
        // Здесь можно добавить логику использования или накопления этой энергии
    }

    private void OnDrawGizmosSelected()
    {
        // Для визуализации радиуса генерации энергии в редакторе Unity
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, generationRadius);
    }
}
