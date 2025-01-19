using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    [System.Serializable]
    public class Resource
    {
        public GameObject prefab; // Префаб ресурса
        public int sellPrice;     // Цена продажи
    }

    public Resource[] resources;     // Массив ресурсов, которые можно продать
    public MoneyManager moneyManager; // Ссылка на MoneyManager

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (Resource resource in resources)
        {
            // Если столкновение произошло с ресурсом из массива
            if (collision.gameObject.CompareTag(resource.prefab.tag))
            {
                // Добавляем деньги за ресурс
                moneyManager.AddMoney(resource.sellPrice);

                // Уничтожаем ресурс
                Destroy(collision.gameObject);

                // Выходим из цикла (не нужно проверять дальше)
                break;
            }
        }
    }
}
