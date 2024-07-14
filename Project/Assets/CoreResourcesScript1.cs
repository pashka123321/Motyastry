using UnityEngine;

public class CoreResourcesScript : MonoBehaviour
{
    public int lead;
    public int gold;
    public int coal;
    public int copper;
    public int copperigot;
    public int leadigot;
    public int goldigot;

    // Метод Start можно удалить, если он не используется
    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != null)
        {
            switch (collision.tag)
            {
                case "золотая руда":
                    gold++;
                    break;
                case "свинцовая руда":
                    lead++;
                    break;
                case "медная руда":
                    copper++;
                    break;
                case "угольная руда":
                    coal++;
                    break;
                case "слиток меди":
                    copperigot++;
                    break;
                case "слиток свинца":
                    leadigot++;
                    break;
                case "слиток золота":
                    goldigot++;
                    break;
                default:
                    return;
                    break;
            }

            Destroy(collision.gameObject);
        }
        else
        {
            Debug.LogWarning("Collision or collision.gameObject is null.");
        }
    }
}
