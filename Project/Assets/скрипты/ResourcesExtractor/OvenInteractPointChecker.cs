using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenInteractPointChecker : MonoBehaviour
{
    [SerializeField] private Furnace spawner;

    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ConveerEnter"))
        {
            spawner.ActivateSpawnPoint((int)(gameObject.name[13]) - 49);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ConveerEnter"))
        {
            spawner.DeactivateSpawnPoint((int)(gameObject.name[13]) - 49);
        }
    }
}
