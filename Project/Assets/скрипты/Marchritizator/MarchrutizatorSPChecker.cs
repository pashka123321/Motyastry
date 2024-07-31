using UnityEngine;

public class MarchrutizatorSPChecker : MonoBehaviour
{
    [SerializeField] private Marchritizator marchritizator;

    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ConveerEnter"))
        {
            marchritizator.ActivateSpawnPoint((int)(gameObject.name[10]) - 49);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ConveerEnter"))
        {
            marchritizator.DeactivateSpawnPoint((int)(gameObject.name[10]) - 49);
        }
    }
}
