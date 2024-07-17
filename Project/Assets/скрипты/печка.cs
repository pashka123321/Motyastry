using System.Collections;
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
    public Sprite defaultSprite;       // Обычный спрайт печки
    public Sprite activeSprite;        // Активный спрайт печки

    private int i = 0;
    public bool[] activeSP;
    private List<Collider2D> oresInTrigger = new List<Collider2D>();

    [SerializeField] private float interval;

    [SerializeField] private GameObject[] zavodEnters;
    private bool isProcessing = false; // Флаг, указывающий на процесс переплавки

    private void Start()
    {
        CheckAllOresInTrigger();
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
        if (isProcessing)
        {
            return; // Если уже идет переплавка, выходим из метода
        }

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

                // Запускаем корутину для переплавки
                StartCoroutine(ProcessOreCoroutine(mapping.ingotPrefab, oreObject, oreCollider));
                return; // Выходим из метода после успешного запуска корутины
            }
        }
    }

    private IEnumerator ProcessOreCoroutine(GameObject ingotPrefab, GameObject oreObject, Collider2D oreCollider)
    {
        // Удаляем объект руды
        Destroy(oreObject);

        // Устанавливаем спрайт на активный
        GetComponent<SpriteRenderer>().sprite = activeSprite;
        isProcessing = true;

        for (int j = 0; j < 4; j++)
        {
            zavodEnters[j].GetComponent<BoxCollider2D>().enabled = true;
        }

        // Задержка переплавки
        yield return new WaitForSeconds(interval);

        // Спавним соответствующий слиток в указанной точке с нулевым поворотом
        Instantiate(ingotPrefab, spawnPoints[i].position, Quaternion.identity);
        i++;

        oresInTrigger.Remove(oreCollider); // Удаляем из списка oresInTrigger

        // Возвращаем спрайт на обычный
        GetComponent<SpriteRenderer>().sprite = defaultSprite;
        isProcessing = false;

        for (int j = 0; j < 4; j++)
        {
            zavodEnters[j].GetComponent<BoxCollider2D>().enabled = false;
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
