using UnityEngine;

public class DrillSpawnPointChecker : MonoBehaviour
{
    [SerializeField] private BlockSpawner spawner;

    public bool ConatinsOre;

    void Start()
    {
        ConatinsOre = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ConveerEnter"))
        {
            spawner.ActivateSpawnPoint((int)(gameObject.name[12]) - 49);
        }
        else if (collision.GetComponent<ResourcesMovementLogic>() != null)
        {
            ConatinsOre = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ConveerEnter"))
        {
            spawner.DeactivateSpawnPoint((int)(gameObject.name[12]) - 49);
        }
        else if (collision.GetComponent<ResourcesMovementLogic>() != null)
        {
            ConatinsOre = false;
        }
    }
}
