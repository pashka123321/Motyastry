using UnityEngine;

public class CoreResourcesScript : MonoBehaviour
{
    public int lead;
    public int gold;
    public int coal;
    public int copper;

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
