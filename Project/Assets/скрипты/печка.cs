using UnityEngine;
using System.Collections.Generic;

public class Furnace : MonoBehaviour
{
    [System.Serializable]
    public class OreToIngotMapping
    {
        public string oreTag;         // Тег руды
        public GameObject ingotPrefab; // Префаб слитка
    }

    public List<OreToIngotMapping> oreToIngotMappings; // Список сопоставлений руды и слитков
    public Transform spawnPoint;    // Точка спавна слитка

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Получаем GameObject, с которым произошло столкновение
        GameObject oreObject = collision.gameObject;

        // Ищем в списке сопоставлений подходящий префаб слитка для данной руды
        foreach (var mapping in oreToIngotMappings)
        {
            // Проверяем по тегу руды
            if (oreObject.CompareTag(mapping.oreTag))
            {
                // Спавним соответствующий слиток в указанной точке с нулевым поворотом
                Instantiate(mapping.ingotPrefab, spawnPoint.position, Quaternion.identity);

                // Удаляем объект руды
                Destroy(oreObject);
                return; // Выходим из метода после успешного спавна слитка
            }
        }
    }
}
