using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CoreResourcesScript : MonoBehaviour
{
    public int lead;
    public int gold;
    public int coal;
    public int copper;
    public int copperigot;
    public int leadigot;
    public int goldigot;

    public GameObject resourceUIPrefab;  // Префаб для отображения ресурса
    public Transform resourceUIParent;   // Родительский объект для UI ресурсов

    public GameObject goldPrefab;
    public GameObject leadPrefab;
    public GameObject coalPrefab;
    public GameObject copperPrefab;
    public GameObject copperIgnotPrefab;
    public GameObject leadIgnotPrefab;
    public GameObject goldIgnotPrefab;

    private Dictionary<string, GameObject> resourceUIDictionary = new Dictionary<string, GameObject>();

    void Start()
    {
        // Изначально обновляем UI
        UpdateUI();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != null)
        {
            bool resourceCollected = false;
            switch (collision.tag)
            {
                case "золотая руда":
                    gold++;
                    resourceCollected = true;
                    break;
                case "свинцовая руда":
                    lead++;
                    resourceCollected = true;
                    break;
                case "медная руда":
                    copper++;
                    resourceCollected = true;
                    break;
                case "угольная руда":
                    coal++;
                    resourceCollected = true;
                    break;
                case "слиток меди":
                    copperigot++;
                    resourceCollected = true;
                    break;
                case "слиток свинца":
                    leadigot++;
                    resourceCollected = true;
                    break;
                case "слиток золота":
                    goldigot++;
                    resourceCollected = true;
                    break;
                default:
                    return;
            }

            if (resourceCollected)
            {
                Destroy(collision.gameObject);
                UpdateUI();
            }
        }
        else
        {
            Debug.LogWarning("Collision or collision.gameObject is null.");
        }
    }

    void UpdateUI()
    {
        UpdateResourceUI("золотая руда", gold, goldPrefab);
        UpdateResourceUI("свинцовая руда", lead, leadPrefab);
        UpdateResourceUI("медная руда", copper, copperPrefab);
        UpdateResourceUI("угольная руда", coal, coalPrefab);
        UpdateResourceUI("слиток меди", copperigot, copperIgnotPrefab);
        UpdateResourceUI("слиток свинца", leadigot, leadIgnotPrefab);
        UpdateResourceUI("слиток золота", goldigot, goldIgnotPrefab);
    }

    void UpdateResourceUI(string resourceName, int amount, GameObject prefab)
    {
        if (amount > 0)
        {
            if (!resourceUIDictionary.ContainsKey(resourceName))
            {
                GameObject resourceUI = Instantiate(resourceUIPrefab, resourceUIParent);
                resourceUIDictionary[resourceName] = resourceUI;
            }

            GameObject uiElement = resourceUIDictionary[resourceName];
            uiElement.GetComponentInChildren<Text>().text = amount.ToString();

            Sprite icon = prefab.GetComponent<SpriteRenderer>().sprite;
            uiElement.GetComponentInChildren<Image>().sprite = icon;

            // Установка цвета, если префаб ресурса содержит компонент SpriteRenderer с цветом
            if (prefab.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
            {
                uiElement.GetComponentInChildren<Image>().color = spriteRenderer.color;
            }
        }
        else
        {
            if (resourceUIDictionary.ContainsKey(resourceName))
            {
                Destroy(resourceUIDictionary[resourceName]);
                resourceUIDictionary.Remove(resourceName);
            }
        }
    }
}
