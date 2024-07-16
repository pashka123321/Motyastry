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
    private List<Collider2D> oresInTrigger = new List<Collider2D>();

    [SerializeField] private GameObject[] zavodEnters;

    [SerializeField] private ObjectStorage storage;

    private void Start()
    {
        CheckAllOresInTrigger();
    }

    private void Update()
    {
        if (storage.currentAmount > 0)
        {
            ProcessOre(storage.currentType);
            storage.currentAmount--;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        oresInTrigger.Add(collision);
        ProcessOre(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        oresInTrigger.Remove(collision);
    }

    private void CheckAllOresInTrigger()
    {
        foreach (var ore in oresInTrigger.ToList())
        {
            ProcessOre(ore);
        }
    }

    private void ProcessOre(Collider2D oreCollider)
    {
        if (oreCollider.GetComponent<ResourcesMovementLogic>() == null)
        {
            return;
        }

        int count = activeSP.Where(c => c).Count();

        if (count == 0)
        {
            if (storage.currentAmount < storage.StorageLimit && storage.currentAmount != 0 && oreCollider == storage.currentType)
            {
                storage.currentAmount++;
            }
            else if (storage.currentAmount == 0)
            {
                Debug.LogError("pON");
                storage.currentType = oreCollider;
                storage.currentAmount++;
            }
            else
            {
                for (int j = 0; j < 4; j++)
                {
                    zavodEnters[j].GetComponent<Collider2D>().enabled = false;
                }

                return;
            }
        }

        if (i == 4)
        {
            i = 0;
        }

        // Получаем GameObject, с которым произошло столкновение
        GameObject oreObject = oreCollider.gameObject;

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
                oresInTrigger.Remove(oreCollider); // Удаляем из списка oresInTrigger
                return; // Выходим из метода после успешного спавна слитка
            }
        }
    }

    public void ActivateSpawnPoint(int spIndex)
    {
        if (activeSP[spIndex] == false)
        {
            activeSP[spIndex] = true;
            CheckAllOresInTrigger(); // Проверка руды при активации спавн-точки

            zavodEnters[spIndex].SetActive(false);
        }
    }

    public void DeactivateSpawnPoint(int spIndex)
    {
        if (activeSP[spIndex] == true)
        {
            activeSP[spIndex] = false;

            zavodEnters[spIndex].SetActive(true);
        }
    }
}
