using UnityEngine;

public class DrillSpawnPointChecker : MonoBehaviour
{
    [SerializeField] private BlockSpawner spawner;

    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Conveer"))
        {
            spawner.ActivateSpawnPoint((int)(gameObject.name[12]) - 49);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Conveer"))
        {
            spawner.DeactivateSpawnPoint((int)(gameObject.name[12]) - 49);
        }
    }
}
