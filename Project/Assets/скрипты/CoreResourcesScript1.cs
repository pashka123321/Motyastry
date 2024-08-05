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

    public GameObject resourceUIPrefab;
    public Transform resourceUIParent;

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

    public bool ConsumeResources(Dictionary<string, int> requiredResources)
    {
        foreach (var resource in requiredResources)
        {
            if (!HasResource(resource.Key, resource.Value))
            {
                Debug.Log("Not enough resources: " + resource.Key);
                return false;
            }
        }

        foreach (var resource in requiredResources)
        {
            DeductResource(resource.Key, resource.Value);
        }

        UpdateUI();
        return true;
    }

    private bool HasResource(string resourceName, int amount)
    {
        switch (resourceName)
        {
            case "золотая руда":
                return gold >= amount;
            case "свинцовая руда":
                return lead >= amount;
            case "медная руда":
                return copper >= amount;
            case "угольная руда":
                return coal >= amount;
            case "слиток меди":
                return copperigot >= amount;
            case "слиток свинца":
                return leadigot >= amount;
            case "слиток золота":
                return goldigot >= amount;
            default:
                return false;
        }
    }

    private void DeductResource(string resourceName, int amount)
    {
        switch (resourceName)
        {
            case "золотая руда":
                gold -= amount;
                break;
            case "свинцовая руда":
                lead -= amount;
                break;
            case "медная руда":
                copper -= amount;
                break;
            case "угольная руда":
                coal -= amount;
                break;
            case "слиток меди":
                copperigot -= amount;
                break;
            case "слиток свинца":
                leadigot -= amount;
                break;
            case "слиток золота":
                goldigot -= amount;
                break;
        }
    }
}
