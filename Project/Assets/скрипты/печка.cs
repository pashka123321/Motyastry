using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    [System.Serializable]
    public class OreToIngotMapping
    {
        public string oreTag;         // Тег руды
        public GameObject ingotPrefab; // Префаб слитка
    }

    public List<OreToIngotMapping> oreToIngotMappings; // Список сопоставлений руды и слитков
    public Transform[] spawnPoints;    // Точка спавна слитка

    private int i = 0;

    public bool[] activeSP;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int count = activeSP.Where(c => c).Count();

        if (count == 0)
        {
            return;
        }

        if (i == 4)
        {
            i = 0;
        }

        // Получаем GameObject, с которым произошло столкновение
        GameObject oreObject = collision.gameObject;

        // Ищем в списке сопоставлений подходящий префаб слитка для данной руды
        foreach (var mapping in oreToIngotMappings)
        {
            // Проверяем по тегу руды
            if (oreObject.CompareTag(mapping.oreTag))
            {
                while (activeSP[i] == false)
                {
                    i++;
                    if (i == 4)
                    {
                        i = 0;
                    }
                }

                // Спавним соответствующий слиток в указанной точке с нулевым поворотом
                Instantiate(mapping.ingotPrefab, spawnPoints[i].position, Quaternion.identity);
                i++;

                // Удаляем объект руды
                Destroy(oreObject);
                return; // Выходим из метода после успешного спавна слитка
            }
        }
    }

    public void ActivateSpawnPoint(int spIndex)
    {
        if (activeSP[spIndex] == false)
        {
            activeSP[spIndex] = true;
        }
    }

    public void DeactivateSpawnPoint(int spIndex)
    {
        if (activeSP[spIndex] == true)
        {
            activeSP[spIndex] = false;
        }
    }
}
